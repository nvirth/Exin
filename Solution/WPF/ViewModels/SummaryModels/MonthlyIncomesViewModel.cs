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

		public override SummaryEngineBase SummaryEngine
		{
			get { return base.SummaryEngine; }
			protected set { MonthlyIncomesManager = (MonthlyIncomesManager)value; }
		}
		public MonthlyIncomesManager MonthlyIncomesManager
		{
			get { return (MonthlyIncomesManager)SummaryEngine; }
			set
			{
				base.SummaryEngine = value;
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
				base.ActualItem = value;

				ActualItem.IsValidationOn = false;
				OnPropertyChanged();
				ActualItem.IsValidationOn = true;
			}
		}
	}
}
