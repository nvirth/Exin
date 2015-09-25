using BLL.WpfManagers;
using Common.UiModels.WPF;

namespace WPF.ViewModels.SummaryModels
{
	public class MonthlyIncomesViewModel : SummaryViewModelBase
	{
		public MonthlyIncomes MonthlyIncomes
		{
			get { return (MonthlyIncomes)SummaryEngine; }
			set
			{
				SummaryEngine = value;
				OnPropertyChanged();
			}
		}

		public IncomeItem ActualIncomeItem
		{
			get { return (IncomeItem)ActualItem; }
			set
			{
				ActualItem = value;
				OnPropertyChanged();
			}
		}
	}
}
