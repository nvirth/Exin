using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Localization;

namespace Common.Utils.Helpers
{
	public static class EnumValues
	{
		private static readonly Dictionary<Type, Enum[]> _enumsValues = new Dictionary<Type, Enum[]>();

		public static IEnumerable<T> Get<T>() where T : struct
		{
			var enumType = typeof(T);
			if(!enumType.GetTypeInfo().IsEnum)
				throw new ArgumentException(Localized.GetEnumValues_needs_an_enum_input__);

			if(_enumsValues.ContainsKey(enumType))
			{
				return _enumsValues[enumType].Cast<T>();
			}
			else
			{
				T[] enumValues = Enum.GetValues(enumType).Cast<T>().ToArray();
				_enumsValues[enumType] = enumValues.Cast<Enum>().ToArray();

				return enumValues;
			}
		}
	}
	public static class EnumValues<TEnumType> where TEnumType : struct
	{
		private static TEnumType[] _enumValues;

		public static TEnumType[] Get()
		{
			if(_enumValues == null)
			{
				var enumType = typeof(TEnumType);
				if(!enumType.GetTypeInfo().IsEnum)
					throw new ArgumentException(Localized.The_EnumValues_class_needs_an_enum_input__);

				_enumValues = Enum.GetValues(enumType).Cast<TEnumType>().ToArray();
			}

			return _enumValues;
		}
	}

	public static class EnumHelpers
	{
		public static T Parse<T>(string enumStr, bool ignoreCase = true)
		{
			var res = (T)Enum.Parse(typeof(T), enumStr, ignoreCase);
			return res;
		}
	}
}
