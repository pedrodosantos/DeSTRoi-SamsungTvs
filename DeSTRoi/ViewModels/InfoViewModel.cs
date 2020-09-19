using DeSTRoi.Models;
using System.Windows;
using System.Windows.Input;

namespace DeSTRoi.ViewModels
{
	public class InfoViewModel : ObservableObject
	{
		private SamyINF _inf;

		public Window Owner
		{
			get;
			set;
		}

		public SamyINF INF
		{
			get
			{
				return _inf;
			}
			set
			{
				_inf = value;
				RaisePropertyChanged("INF");
			}
		}

		public ICommand CloseCommand => new RelayCommand(CloseCommandExecute);

		public InfoViewModel()
		{
		}

		public InfoViewModel(SamyINF inf)
			: this()
		{
			_inf = inf;
		}

		private void CloseCommandExecute()
		{
			Owner.Close();
		}
	}
}