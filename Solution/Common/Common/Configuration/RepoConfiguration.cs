using System;

namespace Common.Configuration
{
	public interface IRepoConfiguration : IEquatable<IRepoConfiguration>
	{
		DbType DbType { get; }
		DbAccessMode DbAccessMode { get; }
		ReadMode ReadMode { get; }
		SaveMode SaveMode { get; }
		bool? DbInsertId { get; }
	}

	[Serializable]
	public class RepoConfiguration : IRepoConfiguration
	{
		public DbType DbType { get; set; } = 0;
		public DbAccessMode DbAccessMode { get; set; } = 0;
		public ReadMode ReadMode { get; set; } = 0;
		public SaveMode SaveMode { get; set; } = 0;
		public bool? DbInsertId { get; set; } = null;

		#region Equality members

		public bool Equals(IRepoConfiguration other)
		{
			if (ReferenceEquals(null, other))
				return false;
			if (ReferenceEquals(this, other))
				return true;
			return
				DbType == other.DbType &&
				DbAccessMode == other.DbAccessMode &&
				ReadMode == other.ReadMode &&
				SaveMode == other.SaveMode &&
				DbInsertId == other.DbInsertId;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != this.GetType())
				return false;
			return Equals((RepoConfiguration) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (int) DbType;
				hashCode = (hashCode*397) ^ (int) DbAccessMode;
				hashCode = (hashCode*397) ^ (int) ReadMode;
				hashCode = (hashCode*397) ^ (int) SaveMode;
				hashCode = (hashCode*397) ^ DbInsertId.GetHashCode();
				return hashCode;
			}
		}

		public static bool operator ==(RepoConfiguration left, RepoConfiguration right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(RepoConfiguration left, RepoConfiguration right)
		{
			return !Equals(left, right);
		}

		#endregion
	}
}