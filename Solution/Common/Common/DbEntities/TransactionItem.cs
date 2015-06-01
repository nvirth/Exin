using System;
using System.Collections.Generic;
using Common.Utils;
using Common.Utils.Helpers;
using UtilsShared;

namespace Common.DbEntities
{
	public partial class TransactionItem
	{
		public int ID { get; set; }
		public int Amount { get; set; }
		public int Quantity { get; set; }
		public int UnitID { get; set; }
		public string Title { get; set; }
		public string Comment { get; set; }
		public DateTime Date { get; set; }
		public int CategoryID { get; set; }
		public bool IsExpenseItem { get; set; }
		public bool IsIncomeItem { get; set; }

		public Category Category { get; set; }
		public Unit Unit { get; set; }
	}

	public partial class TransactionItem : IEquatable<TransactionItem>
	{
		#region Equality members (generated)

		public bool Equals(TransactionItem other)
		{
			if(ReferenceEquals(null, other))
				return false;
			if(ReferenceEquals(this, other))
				return true;
			if(this.GetHashCode() != other.GetHashCode())
				return false;
			return ID == other.ID && Amount == other.Amount && Quantity == other.Quantity && UnitID == other.UnitID && string.Equals(Title, other.Title) && string.Equals(Comment, other.Comment) && Date.Equals(other.Date) && CategoryID == other.CategoryID && IsExpenseItem.Equals(other.IsExpenseItem) && IsIncomeItem.Equals(other.IsIncomeItem) && Equals(Category, other.Category) && Equals(Unit, other.Unit);
		}

		public override bool Equals(object obj)
		{
			if(ReferenceEquals(null, obj))
				return false;
			if(ReferenceEquals(this, obj))
				return true;
			if(obj.GetType() != this.GetType())
				return false;
			return Equals((TransactionItem)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = ID;
				hashCode = (hashCode * 397) ^ Amount;
				hashCode = (hashCode * 397) ^ Quantity;
				hashCode = (hashCode * 397) ^ UnitID;
				hashCode = (hashCode * 397) ^ (Title != null ? Title.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Comment != null ? Comment.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ Date.GetHashCode();
				hashCode = (hashCode * 397) ^ CategoryID;
				hashCode = (hashCode * 397) ^ IsExpenseItem.GetHashCode();
				hashCode = (hashCode * 397) ^ IsIncomeItem.GetHashCode();
				hashCode = (hashCode * 397) ^ (Category != null ? Category.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Unit != null ? Unit.GetHashCode() : 0);
				return hashCode;
			}
		}

		public static bool operator ==(TransactionItem left, TransactionItem right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(TransactionItem left, TransactionItem right)
		{
			return !Equals(left, right);
		}

		#endregion
	}
	public partial class TransactionItem // WithoutIdEqualityComparer 
	{
		#region WithoutIdEqualityComparer (generated)

		private sealed class WithoutIdEqualityComparer : IEqualityComparer<TransactionItem>
		{
			public bool Equals(TransactionItem x, TransactionItem y)
			{
				if(ReferenceEquals(x, y))
					return true;
				if(ReferenceEquals(x, null))
					return false;
				if(ReferenceEquals(y, null))
					return false;
				if(x.GetType() != y.GetType())
					return false;
				if(GetHashCode(x) != GetHashCode(y))
					return false;
				return x.Amount == y.Amount &&
					x.Quantity == y.Quantity &&
					x.UnitID == y.UnitID &&
					string.Equals(x.Title, y.Title) &&
					string.Equals(x.Comment, y.Comment) &&
					x.Date.Equals(y.Date) &&
					x.CategoryID == y.CategoryID &&
					x.IsExpenseItem.Equals(y.IsExpenseItem) &&
					x.IsIncomeItem.Equals(y.IsIncomeItem) &&
					Equals(x.Category, y.Category) &&
					Equals(x.Unit, y.Unit);
			}

			public int GetHashCode(TransactionItem obj)
			{
				unchecked
				{
					var hashCode = obj.Amount;
					hashCode = (hashCode * 397) ^ obj.Quantity;
					hashCode = (hashCode * 397) ^ obj.UnitID;
					hashCode = (hashCode * 397) ^ (obj.Title != null ? obj.Title.GetHashCode() : 0);
					hashCode = (hashCode * 397) ^ (obj.Comment != null ? obj.Comment.GetHashCode() : 0);
					hashCode = (hashCode * 397) ^ obj.Date.GetHashCode();
					hashCode = (hashCode * 397) ^ obj.CategoryID;
					hashCode = (hashCode * 397) ^ obj.IsExpenseItem.GetHashCode();
					hashCode = (hashCode * 397) ^ obj.IsIncomeItem.GetHashCode();
					hashCode = (hashCode * 397) ^ (obj.Category != null ? obj.Category.GetHashCode() : 0);
					hashCode = (hashCode * 397) ^ (obj.Unit != null ? obj.Unit.GetHashCode() : 0);
					return hashCode;
				}
			}
		}

		private static readonly IEqualityComparer<TransactionItem> WithoutIdComparerInstance = new WithoutIdEqualityComparer();

		public static IEqualityComparer<TransactionItem> WithoutIdComparer
		{
			get { return WithoutIdComparerInstance; }
		}

		#endregion
	}
	public partial class TransactionItem : IComparable<TransactionItem>
	{
		public int CompareTo(TransactionItem other)
		{
			int result = Amount.CompareTo(other.Amount);
			return result;
		}
	}

	[Serializable]
	public partial class TransactionItem : ICloneable
	{
		public object Clone()
		{
			return this.DeepClone();
		}
	}
}
