using BLL.WpfManagers;
using Common.UiModels.WPF;
using Common.UiModels.WPF.Base;
using WPF.Utils;

namespace WPF.ViewModels.SummaryModels
{
	public abstract class SummaryViewModelBase : ChainedCommonBase
	{
		private SummaryEngineBase _summaryEngine;
		protected SummaryEngineBase SummaryEngine
		{
			get { return _summaryEngine; }
			set
			{
				Util.SaveSort(_summaryEngine, value);
				_summaryEngine = value;
				OnPropertyChanged();
			}
		}

		private TransactionItemBase _actualItem;
		protected TransactionItemBase ActualItem
		{
			get { return _actualItem; }
			set
			{
				_actualItem = value;

				_actualItem.IsValidationOn = false;
				OnPropertyChanged();
				_actualItem.IsValidationOn = true;
			}
		}
	}
}
