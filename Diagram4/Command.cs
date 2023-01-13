using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Diagram4
{
    internal class Command : ICommand
    {
        public event Action<object?> Called;
        public event EventHandler? CanExecuteChanged;
        public void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
        public bool CanExecute(object? parameter)
        {
            return true;
        }
        public void Execute(object? parameter)
        {
            Called?.Invoke(parameter);
        }
        public Command(Action<object?> func)
        {
            Called += func;
        }
    }
}
