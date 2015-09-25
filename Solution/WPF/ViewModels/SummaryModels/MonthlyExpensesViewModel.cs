using BLL.WpfManagers;
using Common.UiModels.WPF;

namespace WPF.ViewModels.SummaryModels
{
	public class MonthlyExpensesViewModel : SummaryViewModelBase
	{
		public MonthlyExpenses MonthlyExpenses
		{
			get { return (MonthlyExpenses)SummaryEngine; }
			set
			{
				SummaryEngine = value;
				OnPropertyChanged();
			}
		}
	}
}
