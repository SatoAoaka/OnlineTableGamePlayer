using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OnlineTableGamePlayer
{
    /// <summary>
    /// FrameSettingWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class FrameSettingWindow : System.Windows.Window
    {
        private Collection<Point2f> positions;
        public FrameSettingWindow(ImageSource img, ref Collection<Point2f> positions_org)
        {
            InitializeComponent();
            myImageArea.Source = img;
            positions = positions_org;
        }

        private void myImageArea_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var point= e.GetPosition(myImageArea);
            float x = (float)(point.X * myImageArea.Source.Width / myImageArea.ActualWidth);
            float y = (float)(point.Y * myImageArea.Source.Height / myImageArea.ActualHeight);
            positions.Add(new Point2f(x, y));
            if (positions.Count >= 4)
            {
                this.Close();
            }
        }
    }
}
