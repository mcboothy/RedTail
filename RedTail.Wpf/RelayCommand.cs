using System;
using System.Diagnostics;
using System.Windows.Input;

namespace RedTail.WpfLib
{
    /// <summary>
    /// Provides a wrapper for ICommand that allows the user to supply their
    /// own action and predicate to satisfy the ICommand callbadks
    /// </summary>
    public class RelayCommand : ICommand
    {
        protected readonly Action<object> Action;
        protected readonly Predicate<object> Predicate;

        public RelayCommand(Action<object> action)
            : this(action, null)
        {
            
        }

        public RelayCommand(Action<object> action, Predicate<object> predicate)
        {
            Action = action;
            Predicate = predicate;

            Debug.Assert(Action != null);
        }


        public void Execute(object parameter)
        {
            Action(parameter);
        }

        public bool CanExecute(object parameter)
        {
            return Predicate == null || Predicate(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value;  }
        }
    }
}
