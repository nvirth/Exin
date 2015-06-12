using System;
using Common.Db.Entities.Base;
using Localization;

namespace Common.Db.Entities
{
	[Serializable]
	public class Unit: LocalizedEntityBase
	{
		protected override string GetLocalizedTypeNameLowercase()
		{
			return Localized.Unit_lowercase;
		}
	}
}
