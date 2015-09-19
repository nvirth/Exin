using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Common.Configuration;
using Common.Db.Entities;
using Common.UiModels.WPF;
using DAL.DataBase.Managers;
using DAL.RepoCommon.Managers;

namespace WPF.ValueConverters
{
	public enum SummaryToStringConverterParam
	{
		SumIn, SumOut, SumOutWithCategories
	}

	public class SummaryToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var summary = value as Summary;
			if(summary == null)
				return "-";

			var parameterStrongly = parameter as SummaryToStringConverterParam?;
			if(!parameterStrongly.HasValue)
				return "?";

			var cultureHu = Cultures.hu_HU; //TODO currency

			switch(parameterStrongly)
			{
				case SummaryToStringConverterParam.SumIn:
					return summary.SumIn.ToString("C0", cultureHu);
				case SummaryToStringConverterParam.SumOut:
					return summary.SumOut.ToString("C0", cultureHu);
				case SummaryToStringConverterParam.SumOutWithCategories:
					throw new NotSupportedException();
				case null:
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
