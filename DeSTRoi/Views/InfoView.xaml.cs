
using DeSTRoi.ViewModels;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;

namespace DeSTRoi.Views
{
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public partial class InfoView : Window, IComponentConnector
	{
		public InfoView()
		{
			InitializeComponent();
		}

		public InfoView(InfoViewModel vm)
		{
			base.DataContext = vm;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			((InfoViewModel)base.DataContext).Owner = this;
		}

	}
}
