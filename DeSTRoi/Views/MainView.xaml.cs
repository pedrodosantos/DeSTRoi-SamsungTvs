using DeSTRoi.Models;
using DeSTRoi.ViewModels;
using Microsoft.Windows.Controls.Ribbon;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;


namespace DeSTRoi.Views
{

	public partial class MainView : RibbonWindow, IComponentConnector
	{
		private GridViewColumnHeader _CurSortCol;

		private SortAdorner _CurAdorner;


		public MainView()
		{
			InitializeComponent();
			MainViewModel mainViewModel;
			mainViewModel = (base.DataContext as MainViewModel);
			mainViewModel.PasswordChanged += mainViewModel_PasswordChanged;
		}

		public void RibbonWindow_Loaded(object sender, RoutedEventArgs e)
		{
			MainViewModel mainViewModel;
			mainViewModel = (base.DataContext as MainViewModel);
			mainViewModel.Owner = this;
			pwdFtpPwd.Password = mainViewModel.FTPPwd;
		}

		private void lvEncFiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			MainViewModel mainViewModel;
			mainViewModel = (base.DataContext as MainViewModel);
			foreach (EncryptedFileModel addedItem in e.AddedItems)
			{
				mainViewModel.SelectedEncryptedFiles.Add(addedItem);
			}
			foreach (EncryptedFileModel removedItem in e.RemovedItems)
			{
				mainViewModel.SelectedEncryptedFiles.Remove(removedItem);
			}
		}

		private void lvEncFiles_Drop(object sender, DragEventArgs e)
		{
			MainViewModel mainViewModel;
			mainViewModel = (base.DataContext as MainViewModel);
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				string[] array;
				array = (e.Data.GetData(DataFormats.FileDrop, autoConvert: true) as string[]);
				string[] array2;
				array2 = array;
				foreach (string fileName in array2)
				{
					mainViewModel.OpenFile(fileName);
				}
			}
		}

		private void SortClick(object sender, RoutedEventArgs e)
		{
			GridViewColumnHeader gridViewColumnHeader;
			gridViewColumnHeader = (sender as GridViewColumnHeader);
			string propertyName;
			propertyName = (gridViewColumnHeader.Tag as string);
			if (_CurSortCol != null)
			{
				AdornerLayer.GetAdornerLayer(_CurSortCol).Remove(_CurAdorner);
				lvEncFiles.Items.SortDescriptions.Clear();
			}
			ListSortDirection listSortDirection;
			listSortDirection = ListSortDirection.Ascending;
			if (_CurSortCol == gridViewColumnHeader && _CurAdorner.Direction == listSortDirection)
			{
				listSortDirection = ListSortDirection.Descending;
			}
			_CurSortCol = gridViewColumnHeader;
			_CurAdorner = new SortAdorner(_CurSortCol, listSortDirection);
			AdornerLayer.GetAdornerLayer(_CurSortCol).Add(_CurAdorner);
			lvEncFiles.Items.SortDescriptions.Add(new SortDescription(propertyName, listSortDirection));
		}

		private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
		{
			MainViewModel mainViewModel;
			mainViewModel = (base.DataContext as MainViewModel);
			if (mainViewModel.FTPPwd != ((PasswordBox)sender).Password)
			{
				mainViewModel.FTPPwd = ((PasswordBox)sender).Password;
			}
		}

		private void mainViewModel_PasswordChanged(object sender, string data)
		{
			try
			{
				if (pwdFtpPwd.Password != data)
				{
					pwdFtpPwd.Password = data;
				}
			}
			catch
			{
			}
		}

	}
}

