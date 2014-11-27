using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Micropolis.Common
{
    /// <summary>
    /// The delegate command.
    /// </summary>
    public class DelegateCommand : ICommand
    {
        private Action executeMethod;

        public DelegateCommand(Action executeMethod)
        {
            this.executeMethod = executeMethod;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            this.executeMethod.Invoke();
        }
    }
}
