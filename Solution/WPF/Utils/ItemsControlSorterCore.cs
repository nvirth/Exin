using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Common.Utils.Helpers;

namespace WPF.Utils
{
	/// <summary>
	/// ItemsControlSorter: generic part
	/// </summary>
	public static class ItemsControlSorterCore
	{
		private const char DownArrow = '\uffec'; // alt: '\u21e9' '\u2193'
		private const char UpArrow = '\uffea'; // alt: '\u21e7' '\u2191'

		private static string DownArrowStr = DownArrow.ToString(CultureInfo.InvariantCulture);
		private static string UpArrowStr = UpArrow.ToString(CultureInfo.InvariantCulture);

		public static SortControlResult SortControl(RoutedEventArgs e, string sortByProperty = null)
		{
			var headerClicked = e.OriginalSource as GridViewColumnHeader;
			if(headerClicked == null || headerClicked.Role == GridViewColumnHeaderRole.Padding)
				return null;

			var toSort = headerClicked.FindAncestor<ItemsControl>();
			if(toSort == null)
				return null;

			var propertyFail = SetupSortByProperty(ref sortByProperty, headerClicked);
			if(propertyFail)
				return null;

			ICollectionView dataView = CollectionViewSource.GetDefaultView(toSort.ItemsSource);

			var ascSorter = new SortDescription(sortByProperty, ListSortDirection.Ascending);
			var descSorter = new SortDescription(sortByProperty, ListSortDirection.Descending);
			bool comesDescending = dataView.SortDescriptions.Contains(ascSorter);
			bool comesNull = dataView.SortDescriptions.Contains(descSorter);
			bool comesAscendig = !comesDescending && !comesNull;

			var contentString = (headerClicked.Content as string) ?? "xxx"; // "xxx" <- mock
			if(comesDescending)
			{
				dataView.SortDescriptions.Remove(ascSorter);
				dataView.SortDescriptions.Add(descSorter);

				if(!contentString.StartsWith(DownArrowStr))
					contentString = DownArrow + contentString.Substring(1);
			}
			else if(comesNull)
			{
				dataView.SortDescriptions.Remove(descSorter);

				if(contentString.StartsWith(DownArrowStr) || contentString.StartsWith(UpArrowStr))
					contentString = contentString.Substring(1); // without arrow
			}
			else if(comesAscendig)
			{
				dataView.SortDescriptions.Add(ascSorter);

				if(!contentString.StartsWith(UpArrowStr))
					contentString = UpArrow + contentString;
			}

			if(headerClicked.Content is string)
				headerClicked.Content = contentString;

			dataView.Refresh();
			return new SortControlResult()
			{
				ItemsControl = toSort,
				ICollectionView = dataView
			};
		}
		private static bool SetupSortByProperty(ref string sortByProperty, GridViewColumnHeader headerClicked)
		{
			if(string.IsNullOrWhiteSpace(sortByProperty))
			{
				var binding = headerClicked.Column.DisplayMemberBinding as Binding;
				if(binding?.Path == null)
					return true;

				sortByProperty = binding.Path.Path;
			}
			return false;
		}
		public class SortControlResult
		{
			public ItemsControl ItemsControl { get; set; }
			public ICollectionView ICollectionView { get; set; }
		}
	}
}