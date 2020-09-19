// MicroMvvm.RelayCommand<T>
using System;
using System.Diagnostics;
using System.Windows.Input;

public class RelayCommand<T> : ICommand
{
	private readonly Predicate<T> _canExecute;

	private readonly Action<T> _execute;

	public event EventHandler CanExecuteChanged
	{
		add
		{
			if (_canExecute != null)
			{
				CommandManager.RequerySuggested += value;
			}
		}
		remove
		{
			if (_canExecute != null)
			{
				CommandManager.RequerySuggested -= value;
			}
		}
	}

	public RelayCommand(Action<T> execute)
		: this(execute, (Predicate<T>)null)
	{
	}

	public RelayCommand(Action<T> execute, Predicate<T> canExecute)
	{
		if (execute == null)
		{
			throw new ArgumentNullException("execute");
		}
		_execute = execute;
		_canExecute = canExecute;
	}

	[DebuggerStepThrough]
	public bool CanExecute(object parameter)
	{
		if (_canExecute != null)
		{
			return _canExecute((T)parameter);
		}
		return true;
	}

	public void Execute(object parameter)
	{
		_execute((T)parameter);
	}
}
