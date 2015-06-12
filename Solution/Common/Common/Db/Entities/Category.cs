using System;
using Common.Db.Entities.Base;
using Localization;

namespace Common.Db.Entities
{
	[Serializable]
	public class Category : LocalizedEntityBase
	{
		protected override string GetLocalizedTypeNameLowercase()
		{
			return Localized.Category_lowercase;
		}
	}
}
