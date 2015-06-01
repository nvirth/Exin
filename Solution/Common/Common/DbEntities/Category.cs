using System;
using Common.Utils;
using Common.Utils.Helpers;
using UtilsShared;

namespace Common.DbEntities
{
	public partial class Category
	{
		public int ID { get; set; }
		public string Name { get; set; }
		public string DisplayName { get; set; }
	}

	[Serializable]
	public partial class Category : ICloneable
	{
		public object Clone()
		{
			return this.DeepClone();
		}		
	}

	public partial class Category : IEquatable<Category>
	{
		public bool Equals(Category other)
		{
			if (ReferenceEquals(null, other))
				return false;
			if (ReferenceEquals(this, other))
				return true;
			if(this.GetHashCode() != other.GetHashCode())
				return false;
			return ID == other.ID;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != this.GetType())
				return false;
			return Equals((Category) obj);
		}

		public override int GetHashCode()
		{
			return ID;
		}

		public static bool operator ==(Category left, Category right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(Category left, Category right)
		{
			return !Equals(left, right);
		}
	}
}
