using System.Collections.ObjectModel;
using BLL.WpfManagers;
using Common.Db.Entities;
using Common.UiModels.WPF;
using Common.UiModels.WPF.Base;
using DAL.DataBase.Managers;
using WPF.Utils;

namespace WPF.ViewModels
{
	public class MainWindowViewmodel : ChainedCommonBase
	{
		// Singleton instance here
		private StatisticsViewmodel _statistics = new StatisticsViewmodel();
		public StatisticsViewmodel Statistics
		{
			get { return _statistics; }
			set
			{
				_statistics = value;
				OnPropertyChanged();
			}
		}

		private DailyExpenses _dailyExpenses;
		public DailyExpenses DailyExpenses
		{
			get { return _dailyExpenses; }
			set
			{
				SaveSort(_dailyExpenses, value);
				_dailyExpenses = value;
				OnPropertyChanged();
			}
		}

		private MonthlyExpenses _monthlyExpenses;
		public MonthlyExpenses MonthlyExpenses
		{
			get { return _monthlyExpenses; }
			set
			{
				SaveSort(_monthlyExpenses, value);
				_monthlyExpenses = value;
				OnPropertyChanged();
			}
		}

		private MonthlyIncomes _monthlyIncomes;
		public MonthlyIncomes MonthlyIncomes
		{
			get { return _monthlyIncomes; }
			set
			{
				SaveSort(MonthlyIncomes, value);
				_monthlyIncomes = value;
				OnPropertyChanged();
			}
		}

		private IncomeItem _actualIncomeItem;
		public IncomeItem ActualIncomeItem
		{
			get { return _actualIncomeItem; }
			set
			{
				_actualIncomeItem = value;

				_actualIncomeItem.IsValidationOn = false;
				OnPropertyChanged();
				_actualIncomeItem.IsValidationOn = true;
			}
		}

		private ExpenseItem _actualExpenseItem;
		public ExpenseItem ActualExpenseItem
		{
			get { return _actualExpenseItem; }
			set
			{
				_actualExpenseItem = value;

				_actualExpenseItem.IsValidationOn = false;
				OnPropertyChanged();
				_actualExpenseItem.IsValidationOn = true;
			}
		}

		private ObservableCollection<Category> _allCategories;
		public ObservableCollection<Category> AllCategories => _allCategories ?? (_allCategories = new ObservableCollection<Category>(CategoryManager.GetAllValid()));

	    private ObservableCollection<Unit> _allUnits;
		public ObservableCollection<Unit> AllUnits => _allUnits ?? (_allUnits = new ObservableCollection<Unit>(UnitManager.GetAllValid()));

	    public DatePickerFromCodeDateChanger DateChanger;

		// ---

		private void SaveSort(SummaryEngineBase oldValue, SummaryEngineBase newValue)
		{
			if(oldValue != null)
				newValue.SortDescriptions = oldValue.SortDescriptions;
		}
	}
}
