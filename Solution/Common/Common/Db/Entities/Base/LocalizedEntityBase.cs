using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Common.Log;
using Common.Utils.Helpers;
using Localization;
using C = Common.Configuration.Constants.XmlTags;

namespace Common.DbEntities.Base
{
	public abstract partial class LocalizedEntityBase
	{
		public int ID { get; set; }
		public string Name { get; set; }
		public string DisplayNames { get; set; }

		#region Calculated properties

		private Dictionary<string, string> _localizedDisplayNames;
		public Dictionary<string, string> LocalizedDisplayNames
		{
			get
			{
				if (_localizedDisplayNames == null || _localizedDisplayNames.Count == 0)
				{
					_localizedDisplayNames = DisplayNames // "en-US:Food;hu-HU:Kaja;"
						.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries) // ["en-US:Food", "hu-HU:Kaja"]
						.Select(s => s.Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)) // [["en-US", "Food"], ["hu-HU", "Kaja"]]
						.Aggregate(
							new Dictionary<string, string>(),
							(dictionary, sa) =>
							{
								var key = sa[0]; // "en-US" or "hu-HU"
								var value = sa[1]; // "Food" or "Kaja"
								dictionary.Add(key, value);
								return dictionary;
							});
				}
				return _localizedDisplayNames;
			}
		}

		private string _displayName;
		public string DisplayName
		{
			get
			{

				if (_displayName == null)
				{
					_displayName = LocalizedDisplayNames.GetLocalizedValue();
					if (_displayName == null)//TODO test this case
					{
						var msg = Localized.This__0__entity_does_not_contain_any_DisplayName_value___.Formatted(this.GetType().Name);
						ExinLog.ger.LogError(msg, new
						{
							ID,
							Name,
							DisplayNames
						});

						_displayName = Name;
					}
				}

				return _displayName;
			}
		}

		#endregion
	}

	[Serializable]
	public partial class LocalizedEntityBase : ICloneable
	{
		public XElement ToXml()
		{
			//TODO test DisplayNames
			// C <--> Constants.XmlTags
			return new XElement(C.ExpenseItem, new object[]
			{
				new XElement(C.ID, ID),
				new XElement(C.Name, Name),
				new XElement(C.DisplayNames, LocalizedDisplayNames.Select(kvp => new XElement(kvp.Key, kvp.Value))),
			});
		}

		public object Clone()
		{
			return this.DeepClone();
		}		
	}

	public partial class LocalizedEntityBase : IEquatable<LocalizedEntityBase>
	{
		public bool Equals(LocalizedEntityBase other)
		{
			if(ReferenceEquals(null, other))
				return false;
			if(ReferenceEquals(this, other))
				return true;
			return ID == other.ID;
		}

		public override bool Equals(object obj)
		{
			if(ReferenceEquals(null, obj))
				return false;
			if(ReferenceEquals(this, obj))
				return true;
			if(obj.GetType() != this.GetType())
				return false;
			return Equals((LocalizedEntityBase)obj);
		}

		public override int GetHashCode()
		{
			return ID;
		}

		public static bool operator ==(LocalizedEntityBase left, LocalizedEntityBase right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(LocalizedEntityBase left, LocalizedEntityBase right)
		{
			return !Equals(left, right);
		}
	}
}
