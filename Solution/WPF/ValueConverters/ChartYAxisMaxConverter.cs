using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;
using Exin.Common.Logging;
using Exin.Common.Logging.Core;

namespace WPF.ValueConverters
{
	public class ChartYAxisMaxConverter : IValueConverter
	{
		private static Regex Regex = new Regex(@"\D");

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var doubleValue = (double) value;
			return ((int) doubleValue/1000).ToString(CultureInfo.InvariantCulture);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			try
			{
				var stringValue = (string)value;
				if(string.IsNullOrWhiteSpace(stringValue))
					return 0;

				stringValue = Regex.Replace(stringValue, "");
				if(string.IsNullOrWhiteSpace(stringValue))
					return 0;

				var intValue = int.Parse(stringValue);
				return intValue * 1000;
			}
			catch (Exception e)
			{
				Log.Warn(this, m => m("Unexpected error occured. "), LogTarget.All, e);
				return 0;
			}
		}

		public int Convert(int value)
		{
			return value * 1000;
		}
	}
}
