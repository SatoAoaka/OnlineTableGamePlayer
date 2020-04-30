using System.Windows;
using System.Windows.Controls;

namespace OnlineTableGamePlayer.Model
{
    public class MyUtils
    {
        public static float[] ConvertPoint(Point point, Image image)
        {
            float x = (float)(point.X * image.Source.Width / image.ActualWidth);
            float y = (float)(point.Y * image.Source.Height / image.ActualHeight);
            float[] converted_point ={ x,y };
            return converted_point;
        }
    }
}
