using System;
using System.Collections.ObjectModel;
using System.Windows;
using BLL.WpfManagers;
using Common;
using Common.Db.Entities;
using Common.UiModels.WPF;
using Common.UiModels.WPF.Base;
using Common.Utils;
using Common.Utils.Helpers;
using DAL.DataBase.Managers;
using DAL.RepoCommon.Managers;
using Localization;
using WPF.Utils;
using WPF.ViewModels.SummaryModels;

namespace WPF.ViewModels
{
	public class MainWindowViewModel : ChainedCommonBase
	{
		private StatisticsViewModel _statistics;
		public StatisticsViewModel Statistics => _statistics ?? (_statistics = new StatisticsViewModel());

		private MonthlyIncomesViewModel _monthlyIncomesViewModel;
		public MonthlyIncomesViewModel MonthlyIncomesViewModel => _monthlyIncomesViewModel ?? (_monthlyIncomesViewModel = new MonthlyIncomesViewModel());

		private MonthlyExpensesViewModel _monthlyExpensesViewModel;
		public MonthlyExpensesViewModel MonthlyExpensesViewModel => _monthlyExpensesViewModel ?? (_monthlyExpensesViewModel = new MonthlyExpensesViewModel());

		private DailyExpensesViewModel _dailyExpensesViewModel;
		public DailyExpensesViewModel DailyExpensesViewModel => _dailyExpensesViewModel ?? (_dailyExpensesViewModel = new DailyExpensesViewModel());

		private ObservableCollection<Category> _allCategories;
		public ObservableCollection<Category> AllCategories => _allCategories ?? (_allCategories = new ObservableCollection<Category>(CategoryManager.Instance.GetAllValid()));

		private ObservableCollection<Unit> _allUnits;
		public ObservableCollection<Unit> AllUnits => _allUnits ?? (_allUnits = new ObservableCollection<Unit>(UnitManager.Instance.GetAllValid()));

		public DatePickerFromCodeDateChanger DateChanger { get; set; }

		private ClipboardManager _clipboardManager;
		public ClipboardManager ClipboardManager => _clipboardManager ?? (_clipboardManager = new ClipboardManager());

		#region Save

		public bool Save(bool saveDailyExpenses = true, bool saveMonthlyIncomes = true)
		{
			var errorMsg = "";

			if(saveDailyExpenses && DailyExpensesViewModel.Manager.IsModified)
			{
				MessagePresenter.Instance.WriteLineSeparator();
				try
				{
					DailyExpensesViewModel.Manager.SaveData();
					MessagePresenter.Instance.WriteLine(Localized.Daily_expenses_saved_successfully__);
				}
				catch(Exception ex)
				{
					var msg = Localized.Could_not_save_the_daily_expenses_ + ex.Message + "\r\n";
					MessagePresenter.Instance.WriteError(msg);
					MessagePresenter.Instance.WriteException(ex);
					errorMsg += msg;
				}
			}
			if(saveMonthlyIncomes && MonthlyIncomesViewModel.Manager.IsModified)
			{
				MessagePresenter.Instance.WriteLineSeparator();
				try
				{
					MonthlyIncomesViewModel.Manager.SaveData();
					MessagePresenter.Instance.WriteLine(Localized.Monthly_incomes_saved_successfully__);
				}
				catch(Exception ex)
				{
					var msg = Localized.Could_not_save_the_monthly_incomes_ + ex.Message + "\r\n";
					MessagePresenter.Instance.WriteError(msg);
					MessagePresenter.Instance.WriteException(ex);
					errorMsg += msg;
				}
			}

			if(!String.IsNullOrEmpty(errorMsg))
			{
				Util.PromptErrorWindow(errorMsg);
				return false;
			}
			return true;
		}

		public MessageBoxResult SaveWithPromptYesNo(bool saveDailyExpenses = true, bool saveMonthlyIncomes = true)
		{
			var messageBoxResult = PromptSaveWindow(MessageBoxButton.YesNo, saveDailyExpenses, saveMonthlyIncomes);
			switch(messageBoxResult)
			{
				case MessageBoxResult.Yes:
					Save(saveDailyExpenses, saveMonthlyIncomes);
					break;

				case MessageBoxResult.No:
				case MessageBoxResult.None:
					break;
			}
			return messageBoxResult;
		}

		public MessageBoxResult SaveWithPromptYesNoCancel(bool saveDailyExpenses = true, bool saveMonthlyIncomes = true)
		{
			var messageBoxResult = PromptSaveWindow(MessageBoxButton.YesNoCancel, saveDailyExpenses, saveMonthlyIncomes);
			switch(messageBoxResult)
			{
				case MessageBoxResult.Yes:
					Save(saveDailyExpenses, saveMonthlyIncomes);
					break;

				case MessageBoxResult.No:
				case MessageBoxResult.None:
					break;

				case MessageBoxResult.Cancel:
					break;
			}
			return messageBoxResult;
		}

		public MessageBoxResult SaveDailyExpenses()
		{
			return SaveWithPromptOkCancel(saveDailyExpenses: true, saveMonthlyIncomes: false);
		}

		public MessageBoxResult SaveMonthlyIncomes()
		{
			return SaveWithPromptOkCancel(saveDailyExpenses: false, saveMonthlyIncomes: true);
		}

		public MessageBoxResult SaveWithPromptOkCancel(bool saveDailyExpenses = true, bool saveMonthlyIncomes = true)
		{
			var messageBoxResult = PromptSaveWindow(MessageBoxButton.OKCancel, saveDailyExpenses, saveMonthlyIncomes);
			switch(messageBoxResult)
			{
				case MessageBoxResult.OK:
					Save(saveDailyExpenses, saveMonthlyIncomes);
					break;

				case MessageBoxResult.Cancel:
				case MessageBoxResult.None:
					break;
			}
			return messageBoxResult;
		}

		public MessageBoxResult PromptSaveWindow(MessageBoxButton buttons, bool saveDailyExpenses = true, bool saveMonthlyIncomes = true)
		{
			if(DailyExpensesViewModel.Manager.IsModified || MonthlyIncomesViewModel.Manager.IsModified)
			{
				var needSaveDailyExpenses = saveDailyExpenses && DailyExpensesViewModel.Manager.IsModified;
				var needSaveMonthlyIncomes = saveMonthlyIncomes && MonthlyIncomesViewModel.Manager.IsModified;

				var msg = Localized.Save_changes_ + " (";
				msg += needSaveDailyExpenses ? Localized.daily_expenses__LowerCase : "";
				msg += needSaveDailyExpenses && needSaveMonthlyIncomes ? ", " : "";
				msg += needSaveMonthlyIncomes ? Localized.monthly_incomes__LowerCase : "";
				msg += ')';

				return MessageBox.Show(msg, Localized.Save, buttons, MessageBoxImage.Question);
			}

			return MessageBoxResult.None;
		}

		#endregion

	}
}
