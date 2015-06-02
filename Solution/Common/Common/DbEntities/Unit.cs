using System;
using Common.Utils.Helpers;

namespace Common.DbEntities
{
	public partial class Unit
	{
		public int ID { get; set; }
		public string Name { get; set; }
		public string DisplayName { get; set; }
	}

	[Serializable]
	public partial class Unit : ICloneable
	{
		public object Clone()
		{
			return this.DeepClone();
		}
	}

	public partial class Unit : IEquatable<Unit>
	{
		public bool Equals(Unit other)
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
			return Equals((Unit) obj);
		}

		public override int GetHashCode()
		{
			return ID;
		}

		public static bool operator ==(Unit left, Unit right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(Unit left, Unit right)
		{
			return !Equals(left, right);
		}
	}
}
