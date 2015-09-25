using BLL.WpfManagers;
using Common.UiModels.WPF;

namespace WPF.ViewModels.SummaryModels
{
	public class DailyExpensesViewModel : SummaryViewModelBase
	{
		public DailyExpenses DailyExpenses
		{
			get { return (DailyExpenses)SummaryEngine; }
			set { SummaryEngine = value; }
		}

		public ExpenseItem ActualExpenseItem
		{
			get { return (ExpenseItem)ActualItem; }
			set { ActualItem = value; }
		}
	}
}
