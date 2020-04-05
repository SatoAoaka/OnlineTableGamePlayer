using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Media.Imaging;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace OnlineTableGamePlayer.Model
{
    class CameraInput
    {
        internal static Bitmap CaptureImage()
        {
          Mat frame;
          using(VideoCapture capture = new VideoCapture(1))
            {
                frame = new Mat(capture.FrameHeight, capture.FrameWidth, MatType.CV_8UC3);
                capture.Grab();
                NativeMethods.videoio_VideoCapture_operatorRightShift_Mat(capture.CvPtr, frame.CvPtr);
            }

            return frame.ToBitmap();
        }

    }
}
