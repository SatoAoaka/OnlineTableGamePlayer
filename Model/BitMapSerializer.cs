using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace OnlineTableGamePlayer.Model
{
     public sealed class BitMapSerializer : ITcpSerializer<Bitmap>
    {
        static int HEAD_LENGTH = 28;

        // シリアライズ
        public void Serialize(Stream stream, Bitmap message)
        {
            MemoryStream ms = new MemoryStream();
            message.Save(ms,ImageFormat.Png);
            byte[] img = ms.GetBuffer();
            ms.Close();
            
            // データ長の書き込み
            byte[] data_length = BitConverter.GetBytes(img.Length);
            stream.Write(data_length, 0, 4);

            // データの書き込み
            stream.Write(img, 0, img.Length);
           

        }
       
        // デシリアライズ
        public Bitmap Deserialize(Stream stream)
        {            
            // データ長の読み込み
            var len_byte = new Byte[4];
            stream.Read(len_byte, 0, 4);
            var len = BitConverter.ToInt32(len_byte);
            // データの読み込み
            var bytes_all = ReadDataFromNetwork(stream, len);
  
            MemoryStream ms = new MemoryStream();
            ms.Write(bytes_all, 0, bytes_all.Length);
            var bitmap =new Bitmap(ms);
            ms.Close();
            return bitmap;
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
