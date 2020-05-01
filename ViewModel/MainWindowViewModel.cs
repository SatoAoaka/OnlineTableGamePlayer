using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using OnlineTableGamePlayer.Command;
using OnlineTableGamePlayer.Model;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace OnlineTableGamePlayer.ViewModel
{
    internal class MainWindowViewModel : ViewModelBase
    {
        private int cameraIndex = Settings.Default.cameraIndex;

        private ImageSource _myAreaImage;
        private ImageSource myAreaGetter;

        private ImageSource _yourAreaImage;

        private ImageSource _focusedImage;

        private string _chatLog;

        private FrameMatEditer _matEditer;
        private DispatcherTimer timer;

        private TcpPeer<string> _peer;


        public ICommand FrameSeetingWindowOpenCommand { get; }
        public ICommand SettingMenuOpenCommand { get; }
        public ICommand RefreshCommand { get; }

        public ICommand StartWaitConmmand { get; }
        public ICommand StartConnectCommand { get; }

        public ICommand SendButtonCommand { get; }


        #region プロパティ
        public ImageSource MyAreaImage
        {
            get { return _myAreaImage; }
            set
            {
                this._myAreaImage = value;
                this.OnPropertyChanged(nameof(MyAreaImage));
            }
        }

        public ImageSource YourAreaImage
        {
            get { return _yourAreaImage; }
            set
            {
                this._yourAreaImage = value;
                this.OnPropertyChanged(nameof(YourAreaImage));
            }
        }

        //public Image myAreaView;
        public ImageSource FocusedImage
        {
            get { return _focusedImage; }
            set
            {
                this._focusedImage = value;
                this.OnPropertyChanged(nameof(FocusedImage));
            }
        }

        public String ChatLog
        {
            get { return _chatLog; }
            set
            {
                this._chatLog = value;
                this.OnPropertyChanged(nameof(ChatLog));
            }
        }

        #endregion

        public MainWindowViewModel()
        {
            AutoUpdate();
            SettingMenuOpenCommand = new OnlyWindowCommand(OpenSettingMenu.OpenSettingMenuWindow);
            RefreshCommand = new OnlyWindowCommand(Refresh);
            FrameSeetingWindowOpenCommand = new OnlyWindowCommand(MatEditerMake);
            StartConnectCommand = new AnytimeReadyCommand(StartConnect);
            StartWaitConmmand = new AnytimeReadyCommand(WaitConnect);
            SendButtonCommand = new AnytimeReadyCommand(SendButton);
        }

        private void AutoUpdate()
        {
            timer = new DispatcherTimer(DispatcherPriority.Normal)
            {
                Interval = TimeSpan.FromSeconds(0.03),
            };

            System.Threading.SemaphoreSlim semaphore
              = new System.Threading.SemaphoreSlim(1, 1);

            timer.Tick += async(s, e) =>
             {
                 if(!await semaphore.WaitAsync(0)) {
                     return;
                 }
                 try
                 {
                     await Task.Run(() => Update());
                     
                     MyAreaImage = myAreaGetter;
                 }
                 finally
                 {
                     semaphore.Release();
                 }
             };

            timer.Start();

        }

        private void Update()
        {
            var frame = CameraInput.CaptureImage(cameraIndex);
            if (frame != null)
            {
                Bitmap bitmap;
                if (_matEditer != null)
                {
                    bitmap = (_matEditer.EditFrame(frame)).ToBitmap();
                }
                else
                {
                    bitmap = frame.ToBitmap();
                }
                myAreaGetter = Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                myAreaGetter.Freeze();
                bitmap.Dispose();
            }
        }

        private void Refresh()
        {
            timer.Stop();
            MyAreaImage = null;
            cameraIndex = Settings.Default.cameraIndex;
            timer.Start();
        }

        private void MatEditerMake()
        {
            _matEditer = null;
            timer.Stop();
            Update();
            MyAreaImage = myAreaGetter;
            Collection<Point2f> positions = new Collection<Point2f>();
            var window = new FrameSettingWindow(MyAreaImage , ref positions);
            Nullable<bool> dialogResult = window.ShowDialog();
            if (positions.Count == 4)
            {
                _matEditer = new FrameMatEditer(positions);
            }
            timer.Start();

        }


        private async void ChatProc()
        {
            // メッセージを送受信した際のイベント処理
            _peer.Sended += message => ChatLog+= $"> 自分: {message}\n";
            _peer.Recved += message => ChatLog += $"> 相手: {message}\n";

            ChatLog+="チャット開始\n";
            await _peer.StartMessagingAsync();
            ChatLog+="チャット終了\n";
        }
        private async void StartConnect()
        {
            _peer = new TcpPeer<string>(new StringSerializer());
            await _peer.ConnectAsync("127.0.0.1", 10000);

            ChatProc();
        }

        private async void WaitConnect()
        {
            var listener = new TcpListener(new IPEndPoint(IPAddress.Any, 10000));
            listener.Start();
            var socket = await listener.AcceptSocketAsync();
            listener.Stop();
            _peer = new TcpPeer<string>(socket, new StringSerializer());
            ChatProc();
        }

        private void SendButton()
        {
            _peer.Send("テスト送信した！");
        }
    }
}
