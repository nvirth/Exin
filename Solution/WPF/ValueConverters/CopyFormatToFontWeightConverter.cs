using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Common.Configuration;
using Common.Configuration.Settings;

namespace WPF.ValueConverters
{
	public class CopyFormatToFontWeightConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var boldCopyFormat = value as CopyFormat?;
			if (boldCopyFormat == null)
				return FontWeights.Normal;

			var parameterCopyFormat = parameter as CopyFormat?;
			if(parameterCopyFormat == null || parameterCopyFormat != boldCopyFormat)
				return FontWeights.Normal;

			return FontWeights.Bold;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
