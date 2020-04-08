using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

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
            return true;
        }

        public void Execute(object parameter)
        {
            this._action();
        }
    }
}
