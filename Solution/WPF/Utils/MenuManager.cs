using System.Windows;
using Common.Utils;
using Localization;

namespace WPF.Utils
{
	public static class MenuManager
	{
		private static MainWindow _mainWindow;

		/// <summary>
		/// This should be called during MainWindow's init mechanism
		/// </summary>
		public static void Init(MainWindow mainWindow)
		{
			_mainWindow = mainWindow;
		}

		public static void Copy(object sender, RoutedEventArgs e)
		{
			MessagePresenter.Instance.WriteLine(Localized.Copy_menuitem_is_not_implemented_yet);
		}

		public static void Options(object sender, RoutedEventArgs e)
		{
			MessagePresenter.Instance.WriteLine(Localized.Options_menuitem_is_not_implemented_yet);
		}

		public static void Shortcuts(object sender, RoutedEventArgs e)
		{
			MessagePresenter.Instance.WriteLine(Localized.Menu_Shortcuts);
		}

		public static void Search(object sender, RoutedEventArgs e)
		{
			MessagePresenter.Instance.WriteLine(Localized.Menu_Search);
		}

		public static void Exit(object sender, RoutedEventArgs e)
		{
			_mainWindow.Close();
		}
	}
}
