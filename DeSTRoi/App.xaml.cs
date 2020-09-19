using Microsoft.Win32;
using System;
using System.CodeDom.Compiler;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using WPFLocalizeExtension.Engine;
using WPFLocalizeExtension.Extensions;

namespace DeSTRoi
{
	public partial class App : Application
	{
		private class State
		{
			public CultureInfo Result
			{
				get;
				set;
			}
		}

		private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			string resolvedValue;
			resolvedValue = "";
			string resolvedValue2;
			resolvedValue2 = "";
			new LocTextExtension("DeSTRoi:Global:FatErrTitle").ResolveLocalizedValue(out resolvedValue);
			new LocTextExtension("DeSTRoi:Global:FatErrContent").ResolveLocalizedValue(out resolvedValue2);
			MessageBox.Show(resolvedValue2 + e.Exception.Message, resolvedValue, MessageBoxButton.OK, MessageBoxImage.Hand);
			Shutdown(1);
		}

		private void Application_Startup(object sender, StartupEventArgs e)
		{
			SetLocale(CultureInfo.CurrentCulture);
			SystemEvents.UserPreferenceChanged += SystemEvents_UserPreferenceChanged;
		}

		private void SystemEvents_UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
		{
			if (e.Category == UserPreferenceCategory.Locale)
			{
				Thread.CurrentThread.CurrentCulture.ClearCachedData();
				Thread thread;
				thread = new Thread(delegate (object s)
				{
					((State)s).Result = Thread.CurrentThread.CurrentCulture;
				});
				State state;
				state = new State();
				thread.Start(state);
				thread.Join();
				CultureInfo result;
				result = state.Result;
				Thread.CurrentThread.CurrentCulture = result;
				Thread.CurrentThread.CurrentUICulture = result;
				SetLocale(result);
			}
		}

		public void SetLocale(string locale)
		{
			LocalizeDictionary.Instance.Culture = CultureInfo.GetCultureInfo(locale);
		}

		public void SetLocale(CultureInfo culture)
		{
			LocalizeDictionary.Instance.Culture = culture;
		}

		public void InitializeComponent()
		{
			if (!_contentLoaded)
			{
				_contentLoaded = true;
				base.DispatcherUnhandledException += Application_DispatcherUnhandledException;
				base.Startup += Application_Startup;
				base.StartupUri = new Uri("Views/MainView.xaml", UriKind.Relative);
				Uri resourceLocator;
				resourceLocator = new Uri("/DeSTRoi;component/app.xaml", UriKind.Relative);
				Application.LoadComponent(this, resourceLocator);
			}
		}

		public static void Main()
		{
			Thread tr = new Thread(() =>
			{
				DeSTRoi.App app = new DeSTRoi.App();
				app.InitializeComponent();
				app.Run();
			});

			tr.SetApartmentState(ApartmentState.STA);
			tr.Start();
			tr.Join();

		}
	}

}
