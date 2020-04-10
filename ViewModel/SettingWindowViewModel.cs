using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using OnlineTableGamePlayer.Command;

namespace OnlineTableGamePlayer.ViewModel
{
    class SettingWindowViewModel : ViewModelBase
    {
        private string _cameraIndexSetting;

        public ICommand cameraIndexSaveCommand { get; }

        #region プロパティ
        public string cameraIndexSetting
        {
            get { return _cameraIndexSetting; }
            set
            {
                if (int.TryParse(value, out int n))
                {
                    _cameraIndexSetting = value;
                    this.OnPropertyChanged(nameof(cameraIndexSetting));
                }
            }
        }
        #endregion

        public SettingWindowViewModel()
        {
            cameraIndexSetting=Settings.Default.cameraIndex.ToString();
            cameraIndexSaveCommand = new SettingSaveCommand(CameraIndexSave);
        }

        private void CameraIndexSave()
        {
            Settings.Default.cameraIndex = int.Parse(cameraIndexSetting);
            Console.WriteLine(cameraIndexSetting);
        }


    }
}
