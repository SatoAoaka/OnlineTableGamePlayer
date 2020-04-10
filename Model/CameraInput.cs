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
        internal static Bitmap CaptureImage(int cameraIndex)
        {
          Mat frame;
          using(VideoCapture capture = new VideoCapture(cameraIndex))
            {
                frame = new Mat(capture.FrameHeight, capture.FrameWidth, MatType.CV_8UC3);
                capture.Grab();
                NativeMethods.videoio_VideoCapture_operatorRightShift_Mat(capture.CvPtr, frame.CvPtr);
            }
            if (!frame.Empty())
            {
                return frame.ToBitmap();
            }
            else
            {
                return null;
            }
        }

    }
}
