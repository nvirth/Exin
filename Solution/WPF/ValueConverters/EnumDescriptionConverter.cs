using System;
using System.Globalization;
using System.Windows.Data;
using Common.Utils;
using Common.Utils.Helpers;
using Localization;

namespace WPF.ValueConverters
{
	public class EnumDescriptionConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var enumValue = value as Enum;
			if(enumValue==null)
				throw new ArgumentException(Localized.The_value_to_convert_must_be_of_type_Enum__);

			return enumValue.ToLocalizedDescriptionString();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException(Localized.EnumDescriptionConverter_ConvertBack_is_not_supported__);
		}
	}
}
