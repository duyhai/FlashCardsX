using System;
using System.Windows.Input;

namespace FlashCardsX.ViewModel
{
    // Helper class for implementing ICommand classes for WPF button commands.
    public class DelegateCommand : ICommand
    {
        private readonly Func<object, bool> _canExecute;
        private readonly Action<object> _executeAction;
        bool _canExecuteCache;
        public DelegateCommand(Action<object> executeAction,
                               Func<object, bool> canExecute)
        {
            _executeAction = executeAction;
            _canExecute = canExecute;
        }

        // Implementation of CanExecute.
        public bool CanExecute(object parameter)
        {
            var tempCanExecute = _canExecute(parameter);

            if (_canExecuteCache == tempCanExecute) return _canExecuteCache;
            _canExecuteCache = tempCanExecute;

            return _canExecuteCache;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        // Implementation of Execute.
        public void Execute(object parameter)
        {
            _executeAction(parameter);
        }

    }
}
