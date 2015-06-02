using System;
using System.Collections.Generic;
using Common.DbEntities;
using Common.UiModels.WPF.Base;
using Common.Utils.Helpers;

namespace Common.UiModels.WPF
{
	public partial class Summary : ChainedValidationBase
	{
		#region Properties (and their plus methods)

		private int _sumIn;
		public int SumIn
		{
			get { return _sumIn; }
			set
			{
				_sumIn = value;
				OnPropertyChanged();
			}
		}

		private int _sumOut;
		public int SumOut
		{
			get { return _sumOut; }
			set
			{
				_sumOut = value;
				OnPropertyChanged();
			}
		}

		public Dictionary<Category, int> SumOutWithCategories { get; private set; }
		public void UpdateDictionary(int amount, Category key)
		{
			SumOutWithCategories.PlusEqual(key, amount);
			OnPropertyChanged(this.Property(si => si.SumOutWithCategories));
		}

		#endregion

		#region Ctors

		public Summary()
		{
			_sumIn = 0;
			_sumOut = 0;
			SumOutWithCategories = new Dictionary<Category, int>();
		}

		#endregion

		#region Methods

		/// <summary>
		/// If not null, <param name="amount"/> will be added to the summary (update);
		/// if null, then the actual object's amount (add)
		/// </summary>
		public void Update(TransactionItemBase item, int? amount = null)
		{
			amount = amount ?? item.Amount;

			var expenseItem = item as ExpenseItem;
			var isExpense = expenseItem != null; // else item is Income

			var category = isExpense ? expenseItem.Category : null;
			UpdateCore(amount.Value, isExpense, category);
		}

		/// <summary>
		/// If not null, <param name="amount"/> will be added to the summary (update);
		/// if null, then the actual object's amount (add)
		/// </summary>
		public void Update(TransactionItem item, int? amount = null)
		{
			amount = amount ?? item.Amount;
			UpdateCore(amount.Value, item.IsExpenseItem, item.Category);
		}

		private void UpdateCore(int amount, bool isExpense, Category expenseCategory = null)
		{
			if(isExpense)
			{
				var key = expenseCategory;

				SumOut += amount;
				UpdateDictionary(amount, key);
			}
			else //if (isIncome)
			{
				SumIn += amount;
			}
		}

		#endregion
	}

	[Serializable]
	public partial class Summary : ICloneable
	{
		public object Clone()
		{
			return this.DeepClone();
		}
	}
}
