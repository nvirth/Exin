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

		public override SummaryEngineBase ManagerBase
		{
			get { return base.ManagerBase; }
			protected set { Manager = (DailyExpensesManager)value; }
		}
		public DailyExpensesManager Manager
		{
			get { return (DailyExpensesManager)ManagerBase; }
			set
			{
				base.ManagerBase = value;
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
