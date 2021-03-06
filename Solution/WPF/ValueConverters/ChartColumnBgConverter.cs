﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Common.Db.Entities;
using DAL.DataBase.Managers;
using DAL.RepoCommon.Managers;

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

			if(category == CategoryManager.Instance.GetCategoryFullIncomeSummary)
				return Brushes.LawnGreen;

			if(category == CategoryManager.Instance.GetCategoryFullExpenseSummary)
				return Brushes.Red;

			return Brushes.OrangeRed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
