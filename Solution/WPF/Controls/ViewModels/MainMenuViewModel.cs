using System;
using System.Linq;
using System.Windows;
using Common;
using Common.Configuration;
using Common.Configuration.Settings;
using Common.UiModels.WPF;
using Common.Utils;
using Common.Utils.Helpers;
using Localization;
using WPF.ViewModels.SummaryModels;

namespace WPF.Controls.ViewModels
{
	public class MainMenuViewModel
	{
		private readonly MainWindow _mainWindow;

		public MainMenuViewModel(MainWindow mainWindow)
		{
			_mainWindow = mainWindow;
		}

		public void Copy(CopyFormat? copyFormat = null)
		{
			_mainWindow.ViewModel.ClipboardManager.Copy(copyFormat);
		}

		public void Options(object sender, RoutedEventArgs e)
		{
			MessagePresenter.Instance.WriteLine(Localized.Options_menuitem_is_not_implemented_yet);
		}

		public void Shortcuts(object sender, RoutedEventArgs e)
		{
			MessagePresenter.Instance.WriteLine(Localized.Menu_Shortcuts);
		}

		public void Search(object sender, RoutedEventArgs e)
		{
			MessagePresenter.Instance.WriteLine(Localized.Menu_Search);
		}

		public void Exit(object sender, RoutedEventArgs e)
		{
			_mainWindow.Close();
		}
	}
}
