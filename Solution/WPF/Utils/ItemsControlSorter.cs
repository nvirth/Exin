using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WPF.Utils
{
	/// <summary>
	/// ItemsControlSorter: project specific part
	/// </summary>
	public static class ItemsControlSorter
	{
		private static MainWindow _mainWindow;

		/// <summary>
		/// This should be called during MainWindow's init mechanism
		/// </summary>
		public static void Init(MainWindow mainWindow)
		{
			_mainWindow = mainWindow;
		}

		public static void SortControl(RoutedEventArgs e, string sortByProperty = null)
		{
			var scResult = ItemsControlSorterCore.SortControl(e, sortByProperty);
			if(scResult == null)
				return;

			var summaryEngineBase = _mainWindow.ListView2SummaryEngineBase(scResult.ItemsControl as ListView);
			summaryEngineBase.SortDescriptions = scResult.ICollectionView.SortDescriptions.ToArray();
		}
	}
}