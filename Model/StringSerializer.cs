using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OnlineTableGamePlayer.Model
{
    public sealed class StringSerializer : ITcpSerializer<string>
    {
        // シリアライズ
        public void Serialize(Stream stream, string message)
        {
            var bytes = Encoding.UTF8.GetBytes(message);
            // データ長の書き込み
            stream.WriteByte((byte)bytes.Length);
            // データの書き込み
            stream.Write(bytes, 0, bytes.Length);
        }

        // デシリアライズ
        public string Deserialize(Stream stream)
        {
            // データ長の読み込み
            var len = stream.ReadByte();
            // データの読み込み
            return Encoding.UTF8.GetString(ReadDataFromNetwork(stream, len));
        }

        // ネットワークストリームからバイト列を読み込む
        private static byte[] ReadDataFromNetwork(Stream stream, int readLength)
        {
            // ネットワークの場合、一度に読み込めるとは限らないのでループする
            var buffer = new byte[readLength];
            var readTotal = 0;
            while (readTotal < readLength)
            {
                readTotal += stream.Read(buffer, readTotal, readLength - readTotal);
            }
            return buffer;
        }
    }
}
