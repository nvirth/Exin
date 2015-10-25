using BLL.WpfManagers;
using Common.UiModels.WPF;
using Common.Utils.Helpers;

namespace WPF.ViewModels.SummaryModels
{
	public class MonthlyIncomesViewModel : MutableSummaryViewModelBase
	{
		protected override TransactionItemBase NewTransactionItem()
		{
			return new IncomeItem();
		}

		public override SummaryEngineBase ManagerBase
		{
			get { return base.ManagerBase; }
			protected set { Manager = (MonthlyIncomesManager)value; }
		}
		public MonthlyIncomesManager Manager
		{
			get { return (MonthlyIncomesManager)ManagerBase; }
			set
			{
				if(base.ManagerBase == value)
					return;

				base.ManagerBase = value;
				OnPropertyChanged();
			}
		}

		public override TransactionItemBase ActualItem
		{
			get { return base.ActualItem; }
			protected set { ActualIncomeItem = (IncomeItem)value; }
		}
		public IncomeItem ActualIncomeItem
		{
			get { return (IncomeItem)ActualItem; }
			set
			{
				if(base.ActualItem == value)
					return;

				base.ActualItem = value;

				ActualItem.IsValidationOn = false;
				OnPropertyChanged();
				ActualItem.IsValidationOn = true;
			}
		}
	}
}
