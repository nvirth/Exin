using BLL.WpfManagers;
using Common.UiModels.WPF;

namespace WPF.ViewModels.SummaryModels
{
	public class MonthlyExpensesViewModel : SummaryViewModelBase
	{
		public MonthlyExpensesManager Manager
		{
			get { return (MonthlyExpensesManager)ManagerBase; }
			set
			{
				ManagerBase = value;
				OnPropertyChanged();
			}
		}
	}
}
