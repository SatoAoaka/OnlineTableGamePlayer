using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using OnlineTableGamePlayer.Command;
using OnlineTableGamePlayer.Model;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace OnlineTableGamePlayer.ViewModel
{
    internal class MainWindowViewModel : ViewModelBase
    {
        private int cameraIndex = Settings.Default.cameraIndex;

        private ImageSource _myAreaImage;
        private ImageSource myAreaGetter;

        private FrameMatEditer _matEditer;
        private DispatcherTimer timer;


        public bool FrameSeetingMode = false;

        public ICommand FrameSeetingWindowOpenCommand { get; }
        public ICommand SettingMenuOpenCommand { get; }
        public ICommand RefreshCommand { get; }

        #region プロパティ
        public ImageSource MyAreaImage
        {
            get { return _myAreaImage; }
            set
            {
                this._myAreaImage = value;
                this.OnPropertyChanged(nameof(MyAreaImage));
            }
        }
        #endregion

        public MainWindowViewModel()
        {
            AutoUpdate();
            SettingMenuOpenCommand = new OnlyWindowCommand(OpenSettingMenu.OpenSettingMenuWindow);
            RefreshCommand = new OnlyWindowCommand(Refresh);
            FrameSeetingWindowOpenCommand = new OnlyWindowCommand(MatEditerMake);

          //  Point2f[] ori = new Point2f[]{ new Point2f(0, 0), new Point2f(480, 80), new Point2f(160, 600), new Point2f(480, 240) };            
          //  _matEditer = new FrameMatEditer(ori);
        }

        private void AutoUpdate()
        {
            timer = new DispatcherTimer(DispatcherPriority.Normal)
            {
                Interval = TimeSpan.FromSeconds(0.03),
            };

            System.Threading.SemaphoreSlim semaphore
              = new System.Threading.SemaphoreSlim(1, 1);

            timer.Tick += async(s, e) =>
             {
                 if(!await semaphore.WaitAsync(0)) {
                     return;
                 }
                 try
                 {
                     await Task.Run(() => Update());
                     
                     MyAreaImage = myAreaGetter;
                 }
                 finally
                 {
                     semaphore.Release();
                 }
             };

            timer.Start();

        }

        private void Update()
        {
            var frame = CameraInput.CaptureImage(cameraIndex);
            if (frame != null)
            {
                Bitmap bitmap;
                if (_matEditer != null)
                {
                    bitmap = (_matEditer.EditFrame(frame)).ToBitmap();
                }
                else
                {
                    bitmap = frame.ToBitmap();
                }
                myAreaGetter = Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                myAreaGetter.Freeze();
            }
        }

        private void Refresh()
        {
            cameraIndex = Settings.Default.cameraIndex;
        }

        private void MatEditerMake()
        {
            _matEditer = null;
            timer.Stop();
            Update();
            MyAreaImage = myAreaGetter;
            Collection<Point2f> positions = new Collection<Point2f>();
            var window = new FrameSettingWindow(MyAreaImage , ref positions);
            Nullable<bool> dialogResult = window.ShowDialog();
            _matEditer = new FrameMatEditer(positions);
            timer.Start();

        }
    }
}
