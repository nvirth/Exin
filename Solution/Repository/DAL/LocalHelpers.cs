using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SQLite;
using Common;
using Common.Config;
using Common.DbEntities;
using Common.UiModels;
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
