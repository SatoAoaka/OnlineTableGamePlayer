using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace OnlineTableGamePlayer.Command
{
    class AnytimeReadyCommand : ICommand
    {
        #region メンバ変数
        private readonly Action _action;
        #endregion

        #region イベント
        public event EventHandler CanExecuteChanged;
        #endregion

        #region コンストラクタ
        internal AnytimeReadyCommand(Action action)
        {
            this._action = action;
        }
        #endregion


        #region メソッド
        public bool CanExecute(object parameter)
        {
            return true;

        }

        public void Execute(object parameter)
        {
            this._action();
        }
        #endregion
    }
}
