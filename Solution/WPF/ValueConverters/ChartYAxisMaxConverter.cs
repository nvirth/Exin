using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Common.DbEntities;
using DAL.DataBase.Managers;

namespace WPF.ValueConverters
{
	public class ChartYAxisMaxConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var doubleValue = (double) value;
			return ((int) doubleValue/1000).ToString(CultureInfo.InvariantCulture);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var intValue = int.Parse((string)value);
			return intValue * 1000;
		}
	}
}
