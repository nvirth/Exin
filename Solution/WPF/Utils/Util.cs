using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using BLL.WpfManagers;
using Localization;

namespace WPF.Utils
{
	public static class Util
	{
		public static MessageBoxResult PromptErrorWindow(string errorMsg)
		{
			return MessageBox.Show(errorMsg, Localized.Error_, MessageBoxButton.OK, MessageBoxImage.Error);
		}
		public static void SaveSort(SummaryEngineBase oldValue, SummaryEngineBase newValue)
		{
			if(oldValue != null)
				newValue.SortDescriptions = oldValue.SortDescriptions;
		}
	}
}
