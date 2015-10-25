using System;
using Common.Db.Entities.Base;
using Localization;

namespace Common.Db.Entities
{
	[Serializable]
	public class Category : LocalizedEntityBase, IComparable<Category>
	{
		protected override string GetLocalizedTypeNameLowercase()
		{
			return Localized.Category_lowercase;
		}

		public int CompareTo(Category other)
		{
			if(this.DisplayName == null || other.DisplayName == null)
			{
				if(this.DisplayName == null && other.DisplayName == null)
					return 0;
				else if(this.DisplayName == null)
					return -1;
				else //if(other.DisplayName == null)
					return +1;
			}

			var result = string.Compare(this.DisplayName, other.DisplayName, StringComparison.OrdinalIgnoreCase);
			return result;
		}
	}
}
