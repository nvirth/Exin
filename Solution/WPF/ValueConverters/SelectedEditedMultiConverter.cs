using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Common.Db.Entities;
using DAL.DataBase.Managers;
using DAL.RepoCommon.Managers;

namespace WPF.ValueConverters
{
	public class SelectedEditedMultiConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if(values?.Length != 2)
				return FontWeights.Normal;

			var formItem = values[0];
			var listItem = values[1];

			var result = formItem == listItem ? FontWeights.Bold : FontWeights.Normal;
			return result;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
