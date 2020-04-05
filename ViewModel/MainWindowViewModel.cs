using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OnlineTableGamePlayer.Command;
using OnlineTableGamePlayer.Model;


namespace OnlineTableGamePlayer.ViewModel
{
    internal class MainWindowViewModel : ViewModelBase
    {
        private ImageSource _myAreaImage;

        public ICommand UpdateImageCommand { get; }

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
            UpdateImageCommand = new UpdateImageCommand(Update);
        }

        private void Update()
        {
            MyAreaImage = Imaging.CreateBitmapSourceFromHBitmap(CameraInput.CaptureImage().GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }
    }
}
