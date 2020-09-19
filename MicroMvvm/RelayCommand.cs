// MicroMvvm.RelayCommand
using System;
using System.Diagnostics;
using System.Windows.Input;

public class RelayCommand : ICommand
{
	private readonly Func<bool> _canExecute;

	private readonly Action _execute;

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

	public RelayCommand(Action execute)
		: this(execute, null)
	{
	}

	public RelayCommand(Action execute, Func<bool> canExecute)
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
			return _canExecute();
		}
		return true;
	}

	public void Execute(object parameter)
	{
		_execute();
	}
}
