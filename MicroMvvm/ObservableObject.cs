// MicroMvvm.ObservableObject
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;

[Serializable]
public abstract class ObservableObject : INotifyPropertyChanged
{
	[field: NonSerialized]
	public event PropertyChangedEventHandler PropertyChanged;

	protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
	{
		this.PropertyChanged?.Invoke(this, e);
	}

	protected void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpresssion)
	{
		string propertyName = PropertySupport.ExtractPropertyName(propertyExpresssion);
		RaisePropertyChanged(propertyName);
	}

	protected void RaisePropertyChanged(string propertyName)
	{
		OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
	}

	[DebuggerStepThrough]
	[Conditional("DEBUG")]
	public void VerifyPropertyName(string propertyName)
	{
		_ = TypeDescriptor.GetProperties(this)[propertyName];
	}
}
