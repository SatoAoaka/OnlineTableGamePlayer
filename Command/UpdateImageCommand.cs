using System;
using System.Windows.Input;

namespace OnlineTableGamePlayer.Command
{
    internal class UpdateImageCommand : ICommand
    {
        #region メンバ変数
        private readonly Action _action;
        #endregion

        #region イベント
        public event EventHandler CanExecuteChanged;
        #endregion

        #region コンストラクタ
        internal UpdateImageCommand(Action action)
        {
            this._action = action;
        }
        #endregion


        #region メソッド
        public bool CanExecute(object parameter)
        {
            Console.WriteLine("CanExecute trought");
            return true;
        }

        public void Execute(object parameter)
        {
            this._action();
        }
        #endregion
    }
}
