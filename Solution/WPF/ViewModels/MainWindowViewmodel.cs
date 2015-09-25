using System;
using System.Collections.ObjectModel;
using BLL.WpfManagers;
using Common.Db.Entities;
using Common.UiModels.WPF;
using Common.UiModels.WPF.Base;
using Common.Utils.Helpers;
using DAL.DataBase.Managers;
using DAL.RepoCommon.Managers;
using WPF.Utils;
using WPF.ViewModels.SummaryModels;

namespace WPF.ViewModels
{
	public class MainWindowViewmodel : ChainedCommonBase
	{
		private StatisticsViewmodel _statistics;
		public StatisticsViewmodel Statistics => _statistics ?? (_statistics = new StatisticsViewmodel());

		private MonthlyIncomesViewModel _monthlyIncomesViewModel;
		public MonthlyIncomesViewModel MonthlyIncomesViewModel => _monthlyIncomesViewModel ?? (_monthlyIncomesViewModel = new MonthlyIncomesViewModel());

		private MonthlyExpensesViewModel _monthlyExpensesViewModel;
		public MonthlyExpensesViewModel MonthlyExpensesViewModel => _monthlyExpensesViewModel ?? (_monthlyExpensesViewModel = new MonthlyExpensesViewModel());

		private DailyExpensesViewModel _dailyExpensesViewModel;
		public DailyExpensesViewModel DailyExpensesViewModel => _dailyExpensesViewModel ?? (_dailyExpensesViewModel = new DailyExpensesViewModel());


		#region Obsolete

		[Obsolete("Should be replaced with SummaryViewModel")]
		public DailyExpenses DailyExpenses
		{
			get { return DailyExpensesViewModel.DailyExpenses; }
			set
			{
				DailyExpensesViewModel.DailyExpenses = value;
				OnPropertyChanged(); 
			}
		}

		[Obsolete("Should be replaced with SummaryViewModel")]
		public MonthlyExpenses MonthlyExpenses
		{
			get { return MonthlyExpensesViewModel.MonthlyExpenses; }
			set
			{
				MonthlyExpensesViewModel.MonthlyExpenses = value;
				OnPropertyChanged();
			}
		}

		[Obsolete("Should be replaced with SummaryViewModel")]
		public MonthlyIncomes MonthlyIncomes
		{
			get { return MonthlyIncomesViewModel.MonthlyIncomes; }
			set
			{
				MonthlyIncomesViewModel.MonthlyIncomes = value;
				OnPropertyChanged(); 
			}
		}

		[Obsolete("Should be replaced with SummaryViewModel")]
		public IncomeItem ActualIncomeItem
		{
			get { return MonthlyIncomesViewModel.ActualIncomeItem; }
			set
			{
				MonthlyIncomesViewModel.ActualIncomeItem = value;
				MonthlyIncomesViewModel.ActualIncomeItem.IsValidationOn = false;
				OnPropertyChanged();
				MonthlyIncomesViewModel.ActualIncomeItem.IsValidationOn = true;
			}
		}

		[Obsolete("Should be replaced with SummaryViewModel")]
		public ExpenseItem ActualExpenseItem
		{
			get { return DailyExpensesViewModel.ActualExpenseItem; }
			set
			{
				DailyExpensesViewModel.ActualExpenseItem = value;
				DailyExpensesViewModel.ActualExpenseItem.IsValidationOn = false;
				OnPropertyChanged(); 
				DailyExpensesViewModel.ActualExpenseItem.IsValidationOn = true;
			}
		}

		#endregion


		private ObservableCollection<Category> _allCategories;
		public ObservableCollection<Category> AllCategories => _allCategories ?? (_allCategories = new ObservableCollection<Category>(CategoryManager.Instance.GetAllValid()));

		private ObservableCollection<Unit> _allUnits;
		public ObservableCollection<Unit> AllUnits => _allUnits ?? (_allUnits = new ObservableCollection<Unit>(UnitManager.Instance.GetAllValid()));

		public DatePickerFromCodeDateChanger DateChanger;
	}
}
