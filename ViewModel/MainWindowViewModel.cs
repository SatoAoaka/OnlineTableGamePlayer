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
using WrapperClass;

namespace OnlineTableGamePlayer.ViewModel
{
    internal class MainWindowViewModel : ViewModelBase
    {
        private YWrapper cameraOutput;

        private int cameraIndex = Settings.Default.cameraIndex;

        private ImageSource _myAreaImage;
        private ImageSource myAreaGetter;

        private ImageSource _focusedImage;

        private FrameMatEditer _matEditer;
        private DispatcherTimer timer;

       

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

        //public Image myAreaView;
        public ImageSource FocusedImage
        {
            get { return _focusedImage; }
            set
            {
                this._focusedImage = value;
                this.OnPropertyChanged(nameof(FocusedImage));
            }
        }


        #endregion

        public MainWindowViewModel()
        {
            AutoUpdate();
            SettingMenuOpenCommand = new OnlyWindowCommand(OpenSettingMenu.OpenSettingMenuWindow);
            RefreshCommand = new OnlyWindowCommand(Refresh);
            FrameSeetingWindowOpenCommand = new OnlyWindowCommand(MatEditerMake);
            cameraOutput = new YWrapper();

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
                cameraOutput.Send(bitmap);
                myAreaGetter = Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                myAreaGetter.Freeze();
                bitmap.Dispose();
            }
        }

        private void Refresh()
        {
            timer.Stop();
            MyAreaImage = null;
            cameraIndex = Settings.Default.cameraIndex;
            timer.Start();
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
            if (positions.Count == 4)
            {
                _matEditer = new FrameMatEditer(positions);
            }
            timer.Start();

        }

       
    }
}
