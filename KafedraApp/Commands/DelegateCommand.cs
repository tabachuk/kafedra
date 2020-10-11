using System;
using System.Windows.Input;

namespace KafedraApp.Commands
{
	public class DelegateCommand : ICommand
	{
		private readonly Action _action;

        private readonly Func<bool> _canExecute;
 
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
 
        public DelegateCommand(
            Action execute,
            Func<bool> canExecute = null)
        {
            _action = execute;
            _canExecute = canExecute;
        }
 
        public bool CanExecute(object parameter) =>
            _canExecute == null || _canExecute();
 
        public void Execute(object parameter) => _action();
	}

    public class DelegateCommand<T> : ICommand
    {
        private readonly Action<T> _action;

        private readonly Func<T, bool> _canExecute;

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public DelegateCommand(
            Action<T> execute,
            Func<T, bool> canExecute = null)
        {
            _action = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) =>
            _canExecute == null || _canExecute((T)Convert.ChangeType(parameter, typeof(T)));

        public void Execute(object parameter) => _action((T)Convert.ChangeType(parameter, typeof(T)));
    }
}
