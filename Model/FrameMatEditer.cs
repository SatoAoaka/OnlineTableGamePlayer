using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Collections.ObjectModel;

namespace OnlineTableGamePlayer.Model
{
    internal class FrameMatEditer
    {
        Collection<Point2f> orignal_positions = new Collection<Point2f>();
        Collection<Point2f> edited_positions =new Collection<Point2f>();
        internal FrameMatEditer(Point2f[] origin)
        {
            for (int i = 0; i < 4; i++)
            {
                orignal_positions.Add(origin[i]);
            }
        }

        internal FrameMatEditer(Collection<Point2f> origin)
        {
            orignal_positions = origin;
        }
        internal Mat EditFrame(Mat frame)
        {
            if (edited_positions.Count < 4)
            {
                var width = frame.Width;
                var height = frame.Height;
                edited_positions.Add(new Point2f(0, 0));
                edited_positions.Add(new Point2f(0, height));
                edited_positions.Add(new Point2f(width, height));
                edited_positions.Add(new Point2f(width, 0));
            }
            Mat m =  Cv2.GetPerspectiveTransform(orignal_positions,
                                                 edited_positions);
            Mat output = new Mat();
            var size = frame.Size();
            output=frame.WarpPerspective(m,size);
            return output;
        }
    }
}
