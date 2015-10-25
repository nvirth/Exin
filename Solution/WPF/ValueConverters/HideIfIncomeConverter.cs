using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Common;
using Common.Db.Entities;
using DAL.DataBase.Managers;
using DAL.RepoCommon.Managers;

namespace WPF.ValueConverters
{
	public class HideIfIncomeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var transactionItemType  = value as TransactionItemType?;
			if(transactionItemType.HasValue && transactionItemType.Value == TransactionItemType.Income)
				return Visibility.Collapsed;

			return Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
