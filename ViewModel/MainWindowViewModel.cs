using System;
using System.Collections.Generic;
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



namespace OnlineTableGamePlayer.ViewModel
{
    internal class MainWindowViewModel : ViewModelBase
    {
        private int cameraIndex = Settings.Default.cameraIndex;

        private ImageSource _myAreaImage;
        private ImageSource myAreaGetter;
       
        public ICommand UpdateImageCommand { get; }
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
            SettingMenuOpenCommand = new SettingMenuOpenCommand(OpenSettingMenu.OpenSettingMenuWindow);
            RefreshCommand = new RefreshCommand(Refresh);
        }

        private void AutoUpdate()
        {
            var timer = new DispatcherTimer(DispatcherPriority.Normal)
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
            var bitmap = CameraInput.CaptureImage(cameraIndex);
            if (bitmap != null)
            {
                myAreaGetter = Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                myAreaGetter.Freeze();
            }
        }

        private void Refresh()
        {
            cameraIndex = Settings.Default.cameraIndex;
        }
    }
}
