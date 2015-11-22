using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using C = Common.Configuration.Constants.Xml.TransactionItem;

namespace Common.Utils.Helpers
{
	public static class XmlHelpers
	{
		#region ToXml

		public static XElement ToXmlShallow<T>(this IEnumerable<T> listToConvert, Func<T, bool> filter = null, string rootName = C.root)
		{
			var list = (filter == null) ? listToConvert : listToConvert.Where(filter);
			return new XElement(rootName,
				(from node in list
					select new XElement(typeof(T).Name,
						from subnode in node.GetType().GetRuntimeProperties()
						select new XElement(subnode.Name, subnode.GetValue(node, null)))));

		}

		/// <summary>
		/// Don't use this; the ToXml is a better version
		/// </summary>
		public static XElement ToXmlShallow(this object obj)
		{
			IEnumerable<XElement> xElements =
				from property in obj.GetType().GetRuntimeProperties()
				select new XElement(property.Name, property.GetValue(obj));

			return new XElement(obj.GetType().Name, xElements);
		}

		public static XElement ToXml(this object obj)
		{
			var type = obj.GetType();

			// If obj is value type or string, then return it wrapped
			if(type.IsValueType || (obj is string))
				return new XElement(type.Name.ToFriendlyUrl(), obj);

			// If obj is a class (but not string), iterate it's properties, and serialize them to xml format
			IEnumerable<XElement> xElements =
				type.GetRuntimeProperties()
					.Select(property => {
						object propertyValue = property.GetValue(obj);
						if(propertyValue != null)
						{
							var propertyType = propertyValue.GetType();

							// If the property is not a value type or a string, call .ToXml to the property as well
							if(!propertyType.IsValueType && !(propertyValue is string))
							{
								var enumerablePropertyValue = propertyValue as IEnumerable<object>;
								if(enumerablePropertyValue != null)
								{
									// If an IEnumerable type is empty, don't attach it to the xml
									propertyValue = enumerablePropertyValue.Any()
										? (object)propertyValue.ToXml()
										: null; // enumerablePropertyValue.GetType().Name + " = null";
								}
								else // Non-IEnumberable types
								{
									propertyValue = propertyValue.ToXml();
								}
							}
						}

						return new XElement(property.Name.ToFriendlyUrl(), propertyValue);
					});

			return new XElement(type.Name.ToFriendlyUrl(), xElements);
		}

		public static XElement ToXElement<T>(this T obj)
		{
			using(var memoryStream = new MemoryStream())
			{
				using(TextWriter streamWriter = new StreamWriter(memoryStream))
				{
					var xmlSerializer = new XmlSerializer(typeof(T));
					xmlSerializer.Serialize(streamWriter, obj);
					return XElement.Parse(Encoding.UTF8.GetString(memoryStream.ToArray(), 0, (int)memoryStream.Length));
				}
			}
		}

		#endregion

		public static T FromXElement<T>(this XElement xElement)
		{
			var xmlSerializer = new XmlSerializer(typeof(T));
			return (T)xmlSerializer.Deserialize(xElement.CreateReader());
		}

		/// Wrapper around XElement.Element, which can handle null values, and can apply defaults to them
		public static bool Element(this XElement xElement, string name, bool defaultValue)
		{
			var res = ((bool?)xElement.Element(name) ?? defaultValue);
			return res;
		}
		public static XElement GetOrCreateElement(this XElement xml, string tagName)
		{
			var xElement = xml.Element(tagName);
			if(xElement == null)
			{
				xml.Add(new XElement(tagName, null));
				xElement = xml.Element(tagName);
			}
			return xElement;
		}
		public static bool IsReallyEmpty(this XElement xml)
		{
			if(xml == null)
				return true;
			if(!string.IsNullOrWhiteSpace(xml.Value))
				return false;

			var isEmpty = !xml.HasElements && !xml.HasAttributes;
			return isEmpty;

			// Note: xml.IsEmpty method returns true for <Tag />, but false for <Tag></Tag>
		}

		#region Parse

		public static string ParseString(this XElement xml, string xName = null)
		{
			var element = xName == null ? xml : xml?.Element(xName);
			if(element.IsReallyEmpty())
				return "";

			var stringValue = ((string)element).Trim();
			if(string.IsNullOrWhiteSpace(stringValue))
				return "";

			return stringValue;
		}

		public static bool ParseBool(this XElement xml, string xName)
		{
			var element = xml.Element(xName);
			var result = (bool)element;
			return result;
		}

		public static int ParseInt(this XElement xml, string xName)
		{
			var element = xml.Element(xName);
			var result = (int)element;
			return result;
		}

		public static int? ParseIntNullable(this XElement xml, string xName)
		{
			var element = xml.Element(xName);
			if(element.IsReallyEmpty())
				return null;

			var stringValue = ((string)element).Trim();
			if(string.IsNullOrWhiteSpace(stringValue))
				return null;

			var intValue = (int)element;
			return intValue;
		}

		#endregion

	}
}
