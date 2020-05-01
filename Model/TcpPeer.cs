using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OnlineTableGamePlayer.Model
{
    public sealed class TcpPeer<TMessage> where TMessage : class
    {
        public event Action<TMessage> Sended;
        public event Action<TMessage> Recved;

        private Socket _socket;

        private ITcpSerializer<TMessage> _serializer;

        private SynchronizationContext _sc = SynchronizationContext.Current;

        private ConcurrentQueue<TMessage> _sendQueue = new ConcurrentQueue<TMessage>();

        // コンストラクタ（接続待機側で、相手からの接続要求Socketに対して使用）
        public TcpPeer(Socket socket, ITcpSerializer<TMessage> serializer)
        {
            _socket = socket;
            _serializer = serializer;
        }

        // コンストラクタ（接続する側で使用）
        public TcpPeer(ITcpSerializer<TMessage> serializer)
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _serializer = serializer;
        }

        // 指定のIPアドレス・ポートに接続する
        public async Task ConnectAsync(string ipAddr, int port)
        {
            await Task.Factory.StartNew(() => _socket.Connect(IPAddress.Parse(ipAddr), port));
        }

        // 非同期送受信を開始する
        public async Task StartMessagingAsync()
        {
            try
            {
                await Task.WhenAll(
                    Task.Factory.StartNew(DoSendMain, TaskCreationOptions.LongRunning),
                    Task.Factory.StartNew(DoRecvMain, TaskCreationOptions.LongRunning));
            }
            finally
            {
                _socket.Close();
            }
        }

        // メッセージ送信
        public void Send(TMessage message)
        {
            _sendQueue.Enqueue(message);
        }

        // 送信キュー内の全てのメッセージが送信された後、送受信を終了させる
        public void Terminate()
        {
            _sendQueue.Enqueue(null);
        }

        // 送信メイン
        private void DoSendMain()
        {
            try
            {
                using (var stream = new NetworkStream(_socket, FileAccess.Write, false))
                {
                    while (_socket.Connected)
                    {
                        if (_sendQueue.TryDequeue(out var message))
                        {
                            if (message == null)
                            {
                                break;
                            }

                            _serializer.Serialize(stream, message);
                            stream.Flush();
                            _sc.Post(_ => Sended?.Invoke(message), null);
                        }
                        else
                        {
                            // CPU占有回避（本来はManualResetEventを使ってキューに追加された時だけ処理する方がいい）
                            Thread.Sleep(1);
                        }
                    }
                }
            }
            finally
            {
                _socket.Shutdown(SocketShutdown.Send);
            }
        }

        // 受信メイン
        private void DoRecvMain()
        {
            try
            {
                using (var stream = new NetworkStream(_socket, FileAccess.Read, false))
                {
                    var peekBuff = new byte[1];
                    while (_socket.Receive(peekBuff, SocketFlags.Peek) > 0)
                    {
                        var message = _serializer.Deserialize(stream);
                        _sc.Post(_ => Recved?.Invoke(message), null);
                    }
                }
            }
            finally
            {
                Terminate();
            }
        }
    }
}
