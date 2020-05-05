using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace OnlineTableGamePlayer.Model
{
     public sealed class BitMapImageSerializer : ITcpSerializer<BitmapSource>
    {
        static int HEAD_LENGTH = 28;
        
        // シリアライズ
        public void Serialize(Stream stream, BitmapSource message)
        {
            var bytes = GetHeader(message);
            var rawStride = (message.PixelWidth * PixelFormats.Bgr32.BitsPerPixel + 7) / 8;
            byte[] data = new byte[rawStride * message.PixelHeight];
            message.CopyPixels(data, rawStride, 0);

            // データ長の書き込み
            byte[] data_length = BitConverter.GetBytes(HEAD_LENGTH + data.Length);
             stream.Write(data_length,0,4);

            // データの書き込み
            stream.Write(bytes, 0, bytes.Length);
            stream.Write(data, 0, data.Length);
            
           
        }

        private byte[] GetHeader(BitmapSource img)
        {
            var width_b = BitConverter.GetBytes(img.PixelWidth);
            var height_b = BitConverter.GetBytes(img.PixelHeight);
            var dpiX_b = BitConverter.GetBytes(img.DpiX);
            var dpiY_b = BitConverter.GetBytes(img.DpiY);
            var rawStride = BitConverter.GetBytes((img.PixelWidth * PixelFormats.Bgr32.BitsPerPixel + 7) / 8);
            var format =img.Format;
            byte[] dst = new byte[HEAD_LENGTH];
            Array.Copy(width_b, dst, 4);
            Array.Copy(height_b, 0, dst, 4, 4);
            Array.Copy(dpiX_b, 0, dst, 8, 8);
            Array.Copy(dpiY_b, 0, dst, 16, 8);
            Array.Copy(rawStride, 0, dst, 24, 4);
            return dst;
        }

        // デシリアライズ
        public BitmapSource Deserialize(Stream stream)
        {
            // データ長の読み込み
            var len_byte = new Byte[4];
            stream.Read(len_byte, 0, 4);
            var len = BitConverter.ToInt32(len_byte);
            // データの読み込み
            var bytes_all = ReadDataFromNetwork(stream, len);
            var dst4 = new Byte[4];
            var dst8 = new Byte[8];
            Buffer.BlockCopy(bytes_all, 0, dst4, 0, 4);
            var width = BitConverter.ToInt32(dst4);
            Buffer.BlockCopy(bytes_all, 4, dst4, 0, 4);
            var height = BitConverter.ToInt32(dst4);
            Buffer.BlockCopy(bytes_all, 8, dst8, 0, 8);
            var dpiX = BitConverter.ToDouble(dst8);
            Buffer.BlockCopy(bytes_all, 16, dst8, 0, 8);
            var dpiY = BitConverter.ToDouble(dst8);
            Buffer.BlockCopy(bytes_all, 24, dst4, 0, 4);
            var rawStride = BitConverter.ToInt32(dst4);
            var data = new Byte[len - HEAD_LENGTH];
            Buffer.BlockCopy(bytes_all, HEAD_LENGTH, data, 0, len - HEAD_LENGTH);

            return BitmapSource.Create(width,height,dpiX,dpiY, PixelFormats.Bgr32, null, data, rawStride); 
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
