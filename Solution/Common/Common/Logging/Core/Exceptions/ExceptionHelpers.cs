using System;
using System.Collections;
using System.Linq;
using Common.Utils.Helpers;
using Localization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Exin.Common.Logging.Core
{
	public static class ExceptionHelpers
	{
		#region AddData

		public static T WithData<T>(this T exception, IDictionary dictionary) where T : Exception
		{
			foreach(var key in dictionary.Keys)
				exception.AddData(key, dictionary[key]);

			return exception;
		}

		/// This method should be used to add kay-value pairs into an Exception's Data property,
		/// via an anonymusly typed object instance. (So like: new { data1 = "test1", data2 = "test2"} ). <para />
		/// This method iterates through the properties of the given instance.
		public static T WithData<T>(this T exception, object anonymInstance) where T : Exception
		{
			var properties = anonymInstance.GetType().GetProperties();

			foreach(var propertyInfo in properties)
				exception.AddData(propertyInfo.Name, propertyInfo.GetValue(anonymInstance));

			return exception;
		}

		/// <summary>
		/// Adds data to the Exception's Data (IDictionary) property.
		/// The key will be the name of the object value's type.
		/// </summary>
		public static void AddData(this Exception e, object value)
		{
			e.AddData(value.GetType().Name, value);
		}

		public static void AddData(this Exception e, object key, object value)
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

			if (value == null)
				return "null";

			// If the object value is a simple type, "serialize" it with a .ToString call
			if(value.GetType().IsValueType || (value is string))
			{
				stringValue = value.ToString();
			}
			else // If the object value is a complex type, serialize it (normally)
			{
				switch(LogConfig.LogDataMode)
				{
					case LogConfig.JSON:
						stringValue = JsonConvert.SerializeObject(value, Formatting.Indented, _stringEnumConverter);
						break;

					case LogConfig.XML:
						stringValue = value.ToXml().ToString();
						break;

					default:
						throw new NotImplementedException(Localized.The_SerializeToLog_is_not_implemented_to_this_log_mode__ + LogConfig.LogDataMode);
				}
			}

			return stringValue;
		}

		#endregion
	}
}
