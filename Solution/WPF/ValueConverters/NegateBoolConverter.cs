using System;
using System.Globalization;
using System.Windows.Data;

namespace WPF.ValueConverters
{
	public class NegateBoolConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var boolValue = value as bool?;
			if (boolValue != null)
				return !boolValue.Value;

			return false;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
