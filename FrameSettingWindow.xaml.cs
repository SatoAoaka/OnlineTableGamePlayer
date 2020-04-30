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
using OnlineTableGamePlayer.Model;

namespace OnlineTableGamePlayer
{
    /// <summary>
    /// FrameSettingWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class FrameSettingWindow : System.Windows.Window
    {
        private Collection<Point2f> positions;
        private String[] operations = { "切り取りたい範囲の左上をクリック！","切り取りたい範囲の左下をクリック！" , "切り取りたい範囲の右下をクリック！" , "切り取りたい範囲の右上をクリック！" };
        public FrameSettingWindow(ImageSource img, ref Collection<Point2f> positions_org)
        {
            InitializeComponent();
            myImageArea.Source = img;
            positions = positions_org;
        }

        private void myImageArea_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var point= e.GetPosition(myImageArea);
            var point_float=MyUtils.ConvertPoint(point,myImageArea);
            positions.Add(new Point2f(point_float[0], point_float[1]));
            if (positions.Count < 4)
            {
                operation.Content = operations[positions.Count];
            }
            else 
            {
                this.Close();
            }
        }
    }
}
