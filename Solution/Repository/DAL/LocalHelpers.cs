using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SQLite;
using Common;
using Common.Config;
using Common.DbEntities;
using Common.UiModels;
using Common.UiModels.WEB;
using Common.UiModels.WPF;
using DAL.DataBase.Managers;
using Localization;

namespace DAL
{
	public static class LocalHelpers
	{
		#region ExpenseItem helpers

		public static TransactionItem ToTransactionItem(this ExpenseItem expenseItem)
		{
			var transactionItem = new TransactionItem()
			{
				//ID = 0,

				Amount = expenseItem.Amount,
				Quantity = expenseItem.Quantity,
				Title = expenseItem.Title,
				Comment = expenseItem.Comment,
				Date = expenseItem.Date,
				IsExpenseItem = true,
				IsIncomeItem = false,

				Unit = expenseItem.Unit,
				UnitID = expenseItem.Unit.ID,

				Category = expenseItem.Category,
				CategoryID = expenseItem.Category.ID,
			};
			return transactionItem;
		}
		public static TransactionItemVM ToTransactionItemVm(this ExpenseItem expenseItem)
		{
			var transactionItemVm = new TransactionItemVM()
			{
				Amount = expenseItem.Amount,
				Quantity = expenseItem.Quantity,
				Title = expenseItem.Title,
				Comment = expenseItem.Comment,
				Date = expenseItem.Date,

				CategoryID = expenseItem.Category.ID,
				UnitID = expenseItem.Unit.ID,
				Type = TransactionItemType.Expense,
			};
			return transactionItemVm;
		}

		#endregion

		#region IncomeItem helpers

		public static TransactionItem ToTransactionItem(this IncomeItem incomeItem)
		{
			var transactionItem = new TransactionItem()
			{
				//ID = 0,
				Unit = incomeItem.Unit, // Unit.None
				UnitID = incomeItem.Unit.ID, // 0
				Category = CategoryManager.GetCategoryNone,
				CategoryID = CategoryManager.GetCategoryNone.ID,

				Amount = incomeItem.Amount,
				Quantity = 1,
				Title = incomeItem.Title,
				Comment = incomeItem.Comment,
				Date = incomeItem.Date,
				IsExpenseItem = false,
				IsIncomeItem = true,
			};
			return transactionItem;
		}
		public static TransactionItemVM ToTransactionItemVm(this IncomeItem incomeItem)
		{
			var transactionItemVm = new TransactionItemVM()
			{
				Amount = incomeItem.Amount,
				Quantity = incomeItem.Quantity,
				Title = incomeItem.Title,
				Comment = incomeItem.Comment,
				Date = incomeItem.Date,

				CategoryID = CategoryManager.GetCategoryNone.ID,
				UnitID = incomeItem.Unit.ID,
				Type = TransactionItemType.Income,
			};
			return transactionItemVm;
		}

		#endregion

		#region TransactionItem helpers

		public static ExpenseItem ToExpenseItem(this TransactionItem transactionItem)
		{
			var expenseItem = new ExpenseItem()
			{
				Amount = transactionItem.Amount,
				Quantity = transactionItem.Quantity,
				Title = transactionItem.Title,
				Comment = transactionItem.Comment,
				Date = transactionItem.Date,
				Category = transactionItem.Category,
				Unit = transactionItem.Unit,
			};
			return expenseItem;
		}
		public static IncomeItem ToIncomeItem(this TransactionItem transactionItem)
		{
			var incomeItem = new IncomeItem()
			{
				Amount = transactionItem.Amount,
				Quantity = 1,
				Title = transactionItem.Title,
				Comment = transactionItem.Comment,
				Date = transactionItem.Date,
				Unit = transactionItem.Unit, // Unit.None
			};
			return incomeItem;
		}
		public static TransactionItemVM ToTransactionItemVm(this TransactionItem transactionItem)
		{
			var transactionItemVm = new TransactionItemVM()
			{
				Amount = transactionItem.Amount,
				Quantity = transactionItem.Quantity,
				Title = transactionItem.Title,
				Comment = transactionItem.Comment,
				Date = transactionItem.Date,
				CategoryID = transactionItem.CategoryID,
				UnitID = transactionItem.UnitID,
				Type = transactionItem.IsExpenseItem ? TransactionItemType.Expense : TransactionItemType.Income,
			};
			return transactionItemVm;
		}

		#endregion

		#region TransactionItemVM helpers

		public static TransactionItem ToTransactionItem(this TransactionItemVM transactionItemVm)
		{
			return transactionItemVm.ToTransactionItem(transactionItemVm.Type, transactionItemVm.Date);
		}
		public static TransactionItem ToTransactionItem(this TransactionItemVM transactionItemVm, TransactionItemType type, DateTime date)
		{
			var transactionItem = new TransactionItem()
			{
				//ID = 0,

				Amount = transactionItemVm.Amount,
				Quantity = transactionItemVm.Quantity,
				Title = transactionItemVm.Title,
				Comment = transactionItemVm.Comment,
				Date = date,
				CategoryID = transactionItemVm.CategoryID,
				UnitID = transactionItemVm.UnitID,
				IsExpenseItem = type == TransactionItemType.Expense,
				IsIncomeItem = type == TransactionItemType.Income,
				Category = CategoryManager.Get(transactionItemVm.CategoryID),
				Unit = UnitManager.Get(transactionItemVm.UnitID),
			};
			return transactionItem;
		}

		#endregion

		public static DbParameter AddWithValue(this DbParameterCollection collection, string parameterName, object value)
		{
			DbParameter parameter;

			switch(Config.DbType)
			{
				case DbType.MsSql:
					parameter = new SqlParameter() { ParameterName = parameterName, Value = value };
					collection.Add(parameter);
					return parameter;

				case DbType.SQLite:
					parameter = new SQLiteParameter { ParameterName = parameterName, Value = value };
					collection.Add(parameter);
					return parameter;

				default:
					throw new NotImplementedException(string.Format(Localized.Creating_DbParameter_to_DbType___0__is_not_implemented_, Config.DbType));
			}
		}
	}
}
