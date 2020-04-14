using System;
using System.Windows;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Linq;

namespace OnlineTableGamePlayer.Command
{
    class SettingMenuOpenCommand : ICommand
    {
        #region メンバ変数
        private readonly Action _action;
        #endregion

        #region イベント
        public event EventHandler CanExecuteChanged;
        #endregion

        #region コンストラクタ

        public SettingMenuOpenCommand(Action action)
        {
            _action = action;
        }
        #endregion

        public bool CanExecute(object parameter)
        {
            //設定ウィンドウが既に開いていたらfalseを返して終了
            var window = Application.Current.Windows.OfType<SettingWinodw>().FirstOrDefault();
            if (window == null){
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Execute(object parameter)
        {
            this._action();
        }
    }
}
