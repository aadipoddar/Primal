﻿using System.Windows.Input;

namespace PrimalEditor
{
	class RelayCommand<T> : ICommand
	{
		private readonly Action<T> _execute;
		private readonly Predicate<T> _canExecute;

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}

		public bool CanExecute(object parameter)
		{
			return _canExecute?.Invoke((T)parameter) ?? true;
		}

		public void Execute(object parameter)
		{
			_execute((T)parameter);
		}

		public RelayCommand(Action<T> execute, Predicate<T> canExecute = null)
		{
			_execute = execute ?? throw new ArgumentNullException(nameof(execute));
			_canExecute = canExecute;
		}
	}
}
