using OnlineTableGamePlayer.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace OnlineTableGamePlayer.Command
{
    class OnlyWindowCommand : ICommand
    {
        #region メンバ変数
        private readonly Action _action;
        #endregion

        #region イベント
        public event EventHandler CanExecuteChanged;
        #endregion

        #region コンストラクタ
        internal OnlyWindowCommand(Action action)
        {
            this._action = action;
        }
        #endregion


        #region メソッド
        public bool CanExecute(object parameter)
        {
            var window = Application.Current.Windows.OfType<SettingWinodw>().FirstOrDefault();
            if (window == null)
            {
                var window2 = Application.Current.Windows.OfType<FrameSettingWindow>().FirstOrDefault();
                if (window2 == null)
                {
                    return true;
                }
            }            
             return false; 
            
        }

        public void Execute(object parameter)
        {
            this._action();
        }
        #endregion
    }
}
