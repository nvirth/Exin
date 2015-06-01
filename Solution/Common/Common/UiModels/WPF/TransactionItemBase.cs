using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using Common.DbEntities;
using Common.UiModels.WPF.Base;
using Common.UiModels.WPF.Validation;
using Common.Utils;
using Common.Utils.Helpers;
using UtilsShared;

namespace Common.UiModels.WPF
{
	public abstract partial class TransactionItemBase : ChainedValidationBase
	{
		#region Properties

		private int _amount;
		public virtual int Amount
		{
			get { return _amount; }
			set
			{
				var oldValue = _amount;
				_amount = value;
				OnPropertyChanged();
				OnAmountChanged(value - oldValue);
			}
		}

		private int _quantity;
		public virtual int Quantity
		{
			get { return _quantity; }
			set
			{
				_quantity = value;
				OnPropertyChanged();
			}
		}

		private Unit _unit;
		public virtual Unit Unit
		{
			get { return _unit; }
			set
			{
				_unit = value;
				OnPropertyChanged();
			}
		}

		private string _title;
		
		public string Title
		{
			get { return _title; }
			set
			{
				_title = value;
				OnPropertyChanged();
			}
		}

		private string _comment;
		public string Comment
		{
			get { return _comment; }
			set
			{
				_comment = value;
				OnPropertyChanged();
			}
		}

		public DateTime Date { get; set; }

		#endregion

		#region OnAmountChanged event

		/// <summary>
		/// This event fires, when a TransactionItemBase (derived) object's Amount property changed. 
		/// 1. argument: The TIB derived object... 
		/// 2. argument: The amount value, to be added to the 1.arg's Amount value
		/// </summary>
		[field: NonSerialized]
		public event Action<TransactionItemBase, int?> AmountChanged;

		protected virtual void OnAmountChanged(int value)
		{
			var handler = AmountChanged;
			if(handler != null)
				handler(this, value);
		}

		public void CopyAmountChangedHandlerTo(TransactionItemBase otherInstance)
		{
			// Don't worry, these are immutable :)
			// "You don't need to worry about that. The EventHandler<EventArgs> object is immutable so any change in the list of listeners in either object will cause that object to get a new EventHandler<EventArgs> instance containing the updated invocation list."
			// http://stackoverflow.com/questions/6296277/c-sharp-clone-eventhandler
			//
			otherInstance.AmountChanged = this.AmountChanged;
		}

		#endregion

		#region OnCategoryChanged event

		/// <summary>
		/// 1. argument: this (ExpenseItem) 
		/// 2. argument: oldCategory
		/// </summary>
		[field: NonSerialized]
		public event Action<TransactionItemBase, Category> CategoryChanged;

		protected virtual void OnCategoryChanged(Category oldCategory)
		{
			var handler = CategoryChanged;
			if(handler != null)
				handler(this, oldCategory);
		}

		public void CopyCategoryChangedHandlerTo(ExpenseItem otherInstance)
		{
			// Don't worry, these are immutable :)
			// "You don't need to worry about that. The EventHandler<EventArgs> object is immutable so any change in the list of listeners in either object will cause that object to get a new EventHandler<EventArgs> instance containing the updated invocation list."
			// http://stackoverflow.com/questions/6296277/c-sharp-clone-eventhandler
			//
			otherInstance.CategoryChanged = this.CategoryChanged;
		}

		#endregion

		#region Abstract methods

		public abstract XElement ToXml();

		#endregion

		#region Methods

		public TransactionItemBase WithMonthDate()
		{
			Date = new DateTime(Date.Year, Date.Month, 1);
			return this;
		}

		#endregion
	}

	public abstract partial class TransactionItemBase : IComparable<TransactionItemBase>
	{
		public int CompareTo(TransactionItemBase other)
		{
			int result = Amount.CompareTo(other.Amount);
			return result;
		}
	}

	[Serializable]
	public partial class TransactionItemBase : ICloneable
	{
		public virtual object Clone()
		{
			return this.DeepClone();
		}
	}

	public abstract partial class TransactionItemBase //DefaultEqualityComparer 
	{
		// This section is only for derived classes!

		private sealed class DefaultEqualityComparer : IEqualityComparer<TransactionItemBase>
		{
			public bool Equals(TransactionItemBase x, TransactionItemBase y)
			{
				if(ReferenceEquals(x, y))
					return true;
				if(ReferenceEquals(x, null))
					return false;
				if(ReferenceEquals(y, null))
					return false;
				if(x.GetType() != y.GetType())
					return false;
				return x._amount == y._amount 
					&& x._quantity == y._quantity 
					&& Equals(x._unit, y._unit) 
					&& string.Equals(x._title, y._title) 
					&& string.Equals(x._comment, y._comment) 
					&& x.Date.Equals(y.Date);
			}

			public int GetHashCode(TransactionItemBase obj)
			{
				unchecked
				{
					int hashCode = obj._amount;
					hashCode = (hashCode * 397) ^ obj._quantity;
					hashCode = (hashCode * 397) ^ (obj._unit != null ? obj._unit.GetHashCode() : 0);
					hashCode = (hashCode * 397) ^ (obj._title != null ? obj._title.GetHashCode() : 0);
					hashCode = (hashCode * 397) ^ (obj._comment != null ? obj._comment.GetHashCode() : 0);
					hashCode = (hashCode * 397) ^ obj.Date.GetHashCode();
					return hashCode;
				}
			}
		}

		private static readonly IEqualityComparer<TransactionItemBase> DefaultComparerInstance = new DefaultEqualityComparer();

		protected static IEqualityComparer<TransactionItemBase> DefaultTransactionItemBaseComparer
		{
			get { return DefaultComparerInstance; }
		}
	}
}
