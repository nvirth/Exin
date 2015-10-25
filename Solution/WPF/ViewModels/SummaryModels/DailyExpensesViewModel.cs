using BLL.WpfManagers;
using Common.UiModels.WPF;
using Common.Utils.Helpers;

namespace WPF.ViewModels.SummaryModels
{
	public class DailyExpensesViewModel : MutableSummaryViewModelBase
	{
		protected override TransactionItemBase NewTransactionItem()
		{
			return new ExpenseItem();
		}

		public override SummaryEngineBase SummaryEngine
		{
			get { return base.SummaryEngine; }
			protected set { DailyExpenses = (DailyExpenses)value; }
		}
		public DailyExpenses DailyExpenses
		{
			get { return (DailyExpenses)SummaryEngine; }
			set
			{
				base.SummaryEngine = value;
				OnPropertyChanged();
			}
		}

		public override TransactionItemBase ActualItem
		{
			get { return base.ActualItem; }
			protected set { ActualExpenseItem = (ExpenseItem)value; }
		}
		public ExpenseItem ActualExpenseItem
		{
			get { return (ExpenseItem)ActualItem; }
			set
			{
				base.ActualItem = value;

				ActualItem.IsValidationOn = false;
				OnPropertyChanged();
				ActualItem.IsValidationOn = true;
			}
		}
	}
}
