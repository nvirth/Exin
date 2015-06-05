using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Common.DbEntities;
using Common.UiModels.WPF.DefaultValues;
using Common.Utils.Helpers;

namespace Common.UiModels.WPF
{
	public partial class ExpenseItem : TransactionItemBase
	{
		#region Properties

		private Category _category;
		public virtual Category Category
		{
			get { return _category; }
			set
			{
				var oldCategory = _category;
				_category = value;

				OnPropertyChanged();
				OnCategoryChanged(oldCategory);
			}
		}

		#endregion

		#region Ctor

		public ExpenseItem(DateTime date)
			: this()
		{
			Date = date;
		}

		public ExpenseItem()
		{
			Quantity = 1;

			Unit = DefaultValueProvider.Instance.DefaultUnit;
			Category = DefaultValueProvider.Instance.DefaultCategory;
		}

		#endregion

		#region ToXml

		public override XElement ToXml()
		{
			return new XElement("ExpenseItem", new object[]
			{
				new XElement("Title", Title),
				new XElement("Amount", Amount),
				new XElement("Quantity", Quantity),
				new XElement("Unit", Unit.DisplayName),
				new XElement("Category", Category.DisplayName),
				new XElement("Comment", Comment),
			});
		}

		#endregion
	}

	[Serializable]
	public partial class ExpenseItem
	{
		public override object Clone()
		{
			return this.DeepClone();
		}
	}

	public partial class ExpenseItem //DefaultEqualityComparer 
	{
		private sealed class DefaultEqualityComparer : IEqualityComparer<ExpenseItem>
		{
			public bool Equals(ExpenseItem x, ExpenseItem y)
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
				var @equals = Equals(x._category, y._category)
					&& DefaultTransactionItemBaseComparer.Equals(x, y);
				return @equals;
			}

			public int GetHashCode(ExpenseItem obj)
			{
				unchecked
				{
					int hashCode = DefaultTransactionItemBaseComparer.GetHashCode(obj);
					hashCode = (hashCode * 397) ^ (obj._category != null ? obj._category.GetHashCode() : 0);
					return hashCode;
				}
			}
		}

		private static readonly IEqualityComparer<ExpenseItem> DefaultComparerInstance = new DefaultEqualityComparer();

		public static IEqualityComparer<ExpenseItem> DefaultExpenseItemComparer
		{
			get { return DefaultComparerInstance; }
		}
	}
}
