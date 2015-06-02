﻿using System;
using Common.Utils.Helpers;
using Localization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Common.Log
{
	public static class ExceptionHelpers
	{
		#region AddData

		/// <summary>
		/// Adds data to the Exception's Data (IDictionary) property.
		/// The key will be the name of the object value's type.
		/// </summary>
		public static void AddData(this Exception e, object value)
		{
			e.AddData(value.GetType().Name, value);
		}

		public static void AddData(this Exception e, string key, object value)
		{
			// Serializing the object value
			var stringValue = value.SerializeToLog();

			// If the key already exist in the exception's data dictionary, rename it
			if(e.Data.Keys.Contains(key))
			{
				if(e.Data[key].Equals(stringValue))
				{
					return;
				}
				else
				{
					var randomInt = new Random(DateTime.Now.Millisecond).Next(99999);
					e.AddData(key + "-multipleKey-" + randomInt, stringValue);
					return;
				}
			}
			else // If the key not existed yet, we can add it
			{
				e.Data.Add(key, stringValue);
			}
		}

		#endregion

		#region SerializeToLog

		/// <summary>
		/// Use this to convert enums to string (instead of int) while json serializing
		/// </summary>
		private static readonly StringEnumConverter _stringEnumConverter =
			new StringEnumConverter
			{
				CamelCaseText = true
			};

		public static string SerializeToLog(this object value)
		{
			string stringValue = null;

			// If the object value is a simple type, "serialize" it with a .ToString call
			if(value.GetType().IsValueType || (value is string))
			{
				stringValue = value.ToString();
			}
			else // If the object value is a complex type, serialize it (normally)
			{
				switch(Config.LogDataMode)
				{
					case Config.JSON:
						stringValue = JsonConvert.SerializeObject(value, Formatting.Indented, _stringEnumConverter);
						break;

					case Config.XML:
						stringValue = value.ToXml().ToString();
						break;

					default:
						throw new NotImplementedException(Localized.The_SerializeToLog_is_not_implemented_to_this_log_mode__ + Config.LogDataMode);
				}
			}

			return stringValue;
		}

		#endregion
	}
}
