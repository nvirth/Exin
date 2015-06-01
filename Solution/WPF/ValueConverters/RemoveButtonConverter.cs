using System;
using System.Globalization;
using System.Windows.Data;

namespace WPF.ValueConverters
{
	public class RemoveButtonConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var intValue = (int)value;
			return intValue != -1;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
