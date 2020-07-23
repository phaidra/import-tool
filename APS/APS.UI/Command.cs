using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace APS.UI
{
    public class Command : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private Action<object> _execute;
        private Func<object, bool> _canExecute;

        public Command(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke(parameter) ?? true;
        }

        public void Execute(object parameter)
        {
            _execute?.Invoke(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
