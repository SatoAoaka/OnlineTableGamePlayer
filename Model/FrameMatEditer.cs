using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Collections.ObjectModel;

namespace OnlineTableGamePlayer.Model
{
    class FrameMatEditer
    {
        Collection<Point2f> orignal_positions;
        Collection<Point2f> edited_positions ;
        FrameMatEditer(Point2f[] origin,Point2f[] edited)
        {
            for (int i = 0; i < 4; i++)
            {
                orignal_positions.Add(origin[i]);
                edited_positions.Add(edited[i]);
            }
        }
        internal Mat EditFrame(Mat frame)
        {
            Mat m =  Cv2.GetPerspectiveTransform(orignal_positions,
                                                 edited_positions);
            Mat output = new Mat();
            var size = new Size();
            size.Height = 600;
            size.Width = 1200;
            output=frame.WarpPerspective(m,size);
            return output;
        }
    }
}
