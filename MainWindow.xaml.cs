using OnlineTableGamePlayer.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using OnlineTableGamePlayer.Model;
using System.Windows.Ink;

namespace OnlineTableGamePlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private bool myAreaDragBool=false;
        private Point start_position;
        private Path square = new Path() { Stroke = System.Windows.Media.Brushes.Red , StrokeThickness = 1 };

        public MainWindow()
        {
            InitializeComponent();
            
        }

        private Int32Rect SetRect(Image img, Point end_position)
        {
            var start_point_converted = MyUtils.ConvertPoint(start_position, img);
            var end_point_converted = MyUtils.ConvertPoint(end_position, img);
            int x, y, width, height;
            if (start_point_converted[0] < end_point_converted[0])
            {
                x = (int)start_point_converted[0];
                width = (int)(end_point_converted[0] - start_point_converted[0]);
            }
            else
            {
                x = (int)end_point_converted[0];
                width = (int)(start_point_converted[0] - end_point_converted[0]);
            }
            if (start_point_converted[1] < end_point_converted[1])
            {
                y = (int)start_point_converted[1];
                height = (int)(end_point_converted[1] - start_point_converted[1]);
            }
            else
            {
                y = (int)end_point_converted[1];
                height = (int)(start_point_converted[1] - end_point_converted[1]);
            }
            return new Int32Rect(x, y, width, height);
        }


        private void SetMyAreaPathData(Point p)
        {
            var rect = new Rect(start_position, p);
            rect.X += (int)((myAreaCanvas.ActualWidth-myAreaView.ActualWidth)/2);
            rect.Y += (int)((myAreaCanvas.ActualHeight - myAreaView.ActualHeight) / 2);
            square.Data = new RectangleGeometry(rect);
        }

        private void myAreaView_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            start_position = e.GetPosition(myAreaView);
            myAreaDragBool = true;
            myAreaView.CaptureMouse();
            square.Data = new RectangleGeometry();
            myAreaCanvas.Children.Add(square);
        }

        private void myAreaView_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (myAreaDragBool)
            {
                myAreaDragBool = false;
                var end_position = e.GetPosition(myAreaView);
                myAreaView.ReleaseMouseCapture();
                myAreaCanvas.Children.Remove(square);
                var bitmap = myAreaView.Source.Clone() as InteropBitmap;
                var rect = SetRect(myAreaView, end_position);
                try
                {
                    forcusImageArea.Source = new CroppedBitmap(bitmap, rect);
                }
                catch
                {
                   //Out of range for trimming the Image,but it do no action.
                }
            }
        }

       
        private void myAreaView_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (myAreaDragBool)
            {
                var p = e.GetPosition(myAreaView);
                SetMyAreaPathData(p);
            }
        }
    }
}
