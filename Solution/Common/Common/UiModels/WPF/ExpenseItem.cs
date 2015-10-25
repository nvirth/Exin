using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Common.Db;
using Common.Db.Entities;
using C = Common.Configuration.Constants.Xml.TransactionItem;

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

			Unit = ManagersRelief.UnitManager.GetDefaultUnit;
			Category = ManagersRelief.CategoryManager.GetDefaultCategory;
		}

		#endregion
	}

	[Serializable]
	public partial class ExpenseItem
	{
		public override XElement ToXml()
		{
			return new XElement(C.ExpenseItem, new object[]
			{
				new XElement(C.Title, Title),
				new XElement(C.Amount, Amount),
				new XElement(C.Quantity, Quantity),
				new XElement(C.Unit, Unit.Name),
				new XElement(C.Category, Category.Name),
				new XElement(C.Comment, Comment),
			});
		}

		public static ExpenseItem FromXml(DateTime date, XElement xmlEi)
		{
			var unitString = (string)xmlEi.Element(C.Unit);
			var unit = ManagersRelief.UnitManager.GetByName(unitString, nullIfNotFound: false);

			var categoryString = (string)xmlEi.Element(C.Category);
			var category = ManagersRelief.CategoryManager.GetByName(categoryString, nullIfNotFound: false);

			var expenseItem = new ExpenseItem
			{
				Amount = ((int)xmlEi.Element(C.Amount)),
				Quantity = ((int)xmlEi.Element(C.Quantity)),
				Title = ((string)xmlEi.Element(C.Title)).Trim(),
				Comment = ((string)xmlEi.Element(C.Comment) ?? "").Trim(),
				Unit = unit,
				Category = category,
				Date = date,
			};
			return expenseItem;
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
		public static IEqualityComparer<ExpenseItem> DefaultExpenseItemComparer => DefaultComparerInstance;
	}

	public partial class ExpenseItem //Category comparer
	{
		private sealed class CategoryComparer : IComparer<ExpenseItem>
		{
			public int Compare(ExpenseItem x, ExpenseItem y)
			{
				var xNull = x?.Category == null;
				if(xNull || y == null)
				{
					if(xNull && y == null)
						return 0;
					else if(xNull)
						return -1;
					else //if(y == null)
						return +1;
				}

				var result = x.Category.CompareTo(y.Category);
				return result;
			}
		}
		private static readonly IComparer<ExpenseItem> _categoryComparerInstance = new CategoryComparer();
		public static IComparer<ExpenseItem> CategoryComparerInstance => _categoryComparerInstance;
	}
}
