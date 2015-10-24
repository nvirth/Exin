using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Common.Log;
using WPF.Controls.ViewModels;
using WPF.Utils;

namespace WPF.Controls
{
	public partial class MainMenu : UserControl
	{
		#region MenuManager

		private MainMenuViewModel _mainMenuViewModel;
		public MainMenuViewModel MainMenuViewModel
		{
			get
			{
				if(_mainMenuViewModel == null)
				{
					if(MainWindow == null)
					{
						const string msg = "MainWindow should be set";
						throw ExinLog.ger.LogException(msg, new InvalidOperationException(msg));
					}
					_mainMenuViewModel = new MainMenuViewModel(MainWindow);
				}
				return _mainMenuViewModel;
			}
		}

		#endregion

		#region MainWindow

		public static readonly DependencyProperty MainWindowProperty = DependencyProperty.Register(
			"MainWindow", typeof(MainWindow), typeof(MainMenu), new PropertyMetadata(null));

		public MainWindow MainWindow
		{
			get { return (MainWindow)GetValue(MainWindowProperty); }
			set { SetValue(MainWindowProperty, value); }
		}

		#endregion

		public MainMenu()
		{
			InitializeComponent();
			LayoutRoot.DataContext = this;
		}

		private void MenuItem_Copy_OnClick(object sender, RoutedEventArgs e)
		{
			MainMenuViewModel.Copy(sender, e);
		}

		private void MenuItem_Options_OnClick(object sender, RoutedEventArgs e)
		{
			MainMenuViewModel.Options(sender, e);
		}

		private void MenuItem_Shortcuts_OnClick(object sender, RoutedEventArgs e)
		{
			MainMenuViewModel.Shortcuts(sender, e);
		}

		private void MenuItem_Search_OnClick(object sender, RoutedEventArgs e)
		{
			MainMenuViewModel.Search(sender, e);
		}

		private void MenuItem_Exit_OnClick(object sender, RoutedEventArgs e)
		{
			MainMenuViewModel.Exit(sender, e);
		}
	}
}
