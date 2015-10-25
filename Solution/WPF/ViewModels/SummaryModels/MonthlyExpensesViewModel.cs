using BLL.WpfManagers;
using Common.UiModels.WPF;

namespace WPF.ViewModels.SummaryModels
{
	public class MonthlyExpensesViewModel : SummaryViewModelBase
	{
		public override SummaryEngineBase ManagerBase
		{
			get { return base.ManagerBase; }
			protected set { Manager = (MonthlyExpensesManager)value; }
		}
		public MonthlyExpensesManager Manager
		{
			get { return (MonthlyExpensesManager)ManagerBase; }
			set
			{
				if(base.ManagerBase == value)
					return;

				base.ManagerBase = value;
				OnPropertyChanged();
			}
		}
	}
}
