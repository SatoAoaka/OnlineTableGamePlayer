using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace OnlineTableGamePlayer.Command
{
    class SettingSaveCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private Action _action;


        internal SettingSaveCommand(Action action)
        {
            _action = action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            this._action();
        }
    }
}
