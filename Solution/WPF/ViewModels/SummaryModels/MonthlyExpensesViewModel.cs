using BLL.WpfManagers;
using Common.UiModels.WPF;

namespace WPF.ViewModels.SummaryModels
{
	public class MonthlyExpensesViewModel : SummaryViewModelBase
	{
		public MonthlyExpensesManager MonthlyExpensesManager
		{
			get { return (MonthlyExpensesManager)SummaryEngine; }
			set
			{
				SummaryEngine = value;
				OnPropertyChanged();
			}
		}
	}
}
