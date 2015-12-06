using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Common;
using Common.Log;
using Common.Utils.Helpers;
using WPF.ViewModels.SummaryModels;
using Common.Configuration;
using Common.Configuration.Settings;

namespace WPF.Utils
{
	public class ClipboardManager
	{
		#region MainWindow

		private MainWindow _mainWindow;
		public MainWindow MainWindow
		{
			get
			{
				if(_mainWindow == null)
				{
					var msg = "{0} should be set. ".Formatted(this.Property(x => x.MainWindow));
					throw ExinLog.ger.LogException(msg, new InvalidOperationException(msg));
				}
				return _mainWindow;
			}
			set
			{
				if(_mainWindow == value)
				{
					return;
				}
				if(_mainWindow != null)
				{
					var msg = "{0} property may be set only once. ".Formatted(this.Property(x => x.MainWindow));
					throw ExinLog.ger.LogException(msg, new InvalidOperationException(msg));
				}
				_mainWindow = value;
			}
		}

		#endregion

		public void Copy(CopyFormat? copyFormat = null)
		{
			SummaryViewModelBase summaryViewModel;
			switch((TabSummaryNumber)MainWindow.MainTabControl.SelectedIndex)
			{
				case TabSummaryNumber.DailyExpenses:
					summaryViewModel = MainWindow.ViewModel.DailyExpensesViewModel;
					break;
				case TabSummaryNumber.MonthlyExpenses:
					summaryViewModel = MainWindow.ViewModel.MonthlyExpensesViewModel;
					break;
				case TabSummaryNumber.MonthlyIncomes:
					summaryViewModel = MainWindow.ViewModel.MonthlyIncomesViewModel;
					break;
				default:
					throw new NotImplementedException();
			}
			summaryViewModel.CopySelectionToClipboard(copyFormat);
		}
	}
}
