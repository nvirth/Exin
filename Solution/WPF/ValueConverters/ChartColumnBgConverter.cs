using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Common.DbEntities;
using DAL.DataBase.Managers;

namespace WPF.ValueConverters
{
	public class ChartColumnBgConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			// Somehow, this method is called by the framework outside my using case
			// I think the Legend does it... You can't turn it off, just hide :(
			if (value == null)
				return null;

			var keyValuePair = (KeyValuePair<Category, int>) value;
			var category = keyValuePair.Key;

			if(category == CategoryManager.GetCategoryFullIncomeSummary)
				return Brushes.LawnGreen;

			if(category == CategoryManager.GetCategoryFullExpenseSummary)
				return Brushes.Red;

			return Brushes.OrangeRed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
