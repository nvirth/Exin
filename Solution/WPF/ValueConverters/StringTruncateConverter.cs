using System;
using System.Globalization;
using System.Windows.Data;
using Common.Utils.Helpers;

namespace WPF.ValueConverters
{
	public class StringTruncateConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return ((String)value).Truncate(15);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
