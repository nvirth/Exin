using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Common;
using Common.Log;
using Common.Utils.Helpers;
using DAL.DataBase.EntityFramework.EntitiesMsSql;
using DAL.DataBase.EntityFramework.EntitiesSqlite;
using DAL.DataBase.Managers;
using EntityFramework.Extensions;
using Localization;
using Config = Common.Config.Config;
using TransactionItemCommon = Common.DbEntities.TransactionItem;

namespace DAL.DataBase.EntityFramework.Managers
{
	public static class TransactionItemManagerEfFactory
	{
		public static TransactionItemManagerEfBase Create()
		{
			switch(Config.DbType)
			{
				case DbType.MsSql:
					return new TransactionItemManagerEfMsSql();

				case DbType.SQLite:
					return new TransactionItemManagerEfSQLite();

				default:
					throw new NotImplementedException(string.Format(Localized.TransactionItemManagerEfFactory_is_not_implemented_for_this_DbType__FORMAT__, Config.DbType));
			}

		}
	}

	public abstract class TransactionItemManagerEfBase : TransactionItemManagerCommonBase
	{
	}

	public class TransactionItemManagerEfMsSql : TransactionItemManagerEfBase
	{
		#region READ

		public override List<TransactionItemCommon> GetInterval(DateTime fromDate, DateTime toDate, TransactionItemType transactionItemType)
		{
			fromDate = fromDate.Date;
			toDate = toDate.Date;

			using(var ctx = Utils.InitContextForMsSql())
			{
				var query = ctx.TransactionItem
					.Where(ti => ti.Date >= fromDate)
					.Where(ti => ti.Date <= toDate);

				if(transactionItemType == TransactionItemType.Expense)
					query = query.Where(ti => ti.IsExpenseItem);
				else if(transactionItemType == TransactionItemType.Income)
					query = query.Where(ti => ti.IsIncomeItem);
				//else if(transactionItemType == TransactionItemType.Both) <-- No filter predicate

				var result = Utils.ExecRead<TransactionItem_MsSql, TransactionItemCommon>(query).ToList();
				return result;
			}
		}

		public override List<TransactionItemCommon> GetAll(TransactionItemType? transactionItemType = null)
		{
			using(var ctx = Utils.InitContextForMsSql())
			{
				IQueryable<TransactionItem_MsSql> query = ctx.TransactionItem;

				if(transactionItemType.HasValue)
					if(transactionItemType == TransactionItemType.Expense)
						query = query.Where(ti => ti.IsExpenseItem);
					else if(transactionItemType == TransactionItemType.Income)
						query = query.Where(ti => ti.IsIncomeItem);

				var result = Utils.ExecRead<TransactionItem_MsSql, TransactionItemCommon>(query).ToList();
				return result;
			}
		}

		#endregion

		#region CREATE, UPDATE, DELETE

		#region CREATE

		#region Insert

		public override void Insert(TransactionItemCommon transactionItem, bool withId = false)
		{
			using(var ctx = Utils.InitContextForMsSql())
			using(ctx.WithIdentityInsert(ctx.Property(c => c.TransactionItem), activate: withId))
			{
				Insert(ctx, transactionItem, withId);
			}
		}

		public void Insert(ExinEfMsSqlContext ctx, TransactionItemCommon transactionItem, bool withId = false)
		{
			try
			{
				Utils.ExecAdd(ctx.TransactionItem, transactionItem, ctx);
			}
			catch(Exception e)
			{
				ExinLog.ger.LogException(Localized.Could_not_insert_the_transaction_item_, e, transactionItem);
				throw;
			}
		}

		#endregion

		#region InsertMany

		public override void InsertMany(IList<TransactionItemCommon> transactionItems, bool withId = false, bool forceOneByOne = false)
		{
			using(var ctx = Utils.InitContextForMsSql())
			{
				InsertMany(ctx, transactionItems, withId);
			}
		}

		protected virtual void InsertMany(ExinEfMsSqlContext ctx, IList<TransactionItemCommon> transactionItems, bool withId = false)
		{
			using(ctx.WithIdentityInsert(ctx.Property(c => c.TransactionItem), activate: withId))
			{
				if(transactionItems == null || transactionItems.Count == 0)
					return;

				if(transactionItems.Count == 1)
				{
					Insert(ctx, transactionItems[0], withId);
					return;
				}

				using(var transactionScope = new TransactionScope())
				{
					try
					{
						Utils.ExecAddRange(ctx.TransactionItem, transactionItems, ctx);
						transactionScope.Complete();
					}
					catch(Exception e)
					{
						var msg = Localized.Could_not_insert_the_transaction_items_ + transactionItems.Count + Localized._pc_;
						ExinLog.ger.LogException(msg, e, transactionItems);
						throw;
					}
				}
			}
		}

		#endregion

		#endregion

		#region UPDATE

		public override int UpdateFullRecord(TransactionItemCommon transactionItem)
		{
			using(var ctx = Utils.InitContextForMsSql())
			{
				try
				{
					int queryResult;
					Utils.ExecUpdate(ctx.TransactionItem, transactionItem, ctx, out queryResult);
					return queryResult;
				}
				catch(Exception e)
				{
					var msg = Localized.Could_not_update_the_transaction_record__TransactionItem__ID_ + transactionItem.ID + ")" + Environment.NewLine;
					msg += Localized.So_these_modifications_did_not_apply_in_the_database; //data: transactionItem
					ExinLog.ger.LogException(msg, e, transactionItem);
					throw;
				}
			}
		}

		#endregion

		#region DELETE

		#region Delete (1 record)

		public override int Delete(int id)
		{
			using(var ctx = Utils.InitContextForMsSql())
			{
				try
				{
					int queryResult = ctx.TransactionItem.Where(ti => ti.ID == id).Delete();
					return queryResult;
				}
				catch(Exception e)
				{
					var msg = Localized.Could_not_remove_the_transaction_record__TransactionItem__ID_ + id + ")";
					ExinLog.ger.LogException(msg, e);
					throw;
				}
			}
		}

		#endregion

		#region ClearDay

		public override int ClearDay(DateTime date, TransactionItemType transactionItemType)
		{
			using(var ctx = Utils.InitContextForMsSql())
			{
				return ClearDay(ctx, date, transactionItemType);
			}
		}

		protected virtual int ClearDay(ExinEfMsSqlContext ctx, DateTime date, TransactionItemType transactionItemType)
		{
			var isExpense = transactionItemType == TransactionItemType.Expense;
			if(isExpense)
				date = date.Date;

			var isIncome = transactionItemType == TransactionItemType.Income;
			if(isIncome)
				date = new DateTime(date.Year, date.Month, 1);

			try
			{
				int queryResult = ctx.TransactionItem
					.Where(ti => ti.Date == date)
					.Where(ti => ti.IsExpenseItem == isExpense)
					.Where(ti => ti.IsIncomeItem == isIncome)
					.Delete();

				return queryResult;
			}
			catch(Exception e)
			{
				var transactionItemTypeStr = isExpense ? Localized.expenses : isIncome ? Localized.incomes : Localized.transaction_items;
				var msg = string.Format(Localized.Could_not_remove_the_daily_0_at_1__FORMAT__, transactionItemTypeStr, date.ToShortDateString());
				ExinLog.ger.LogException(msg, e);
				throw;
			}
		}

		#endregion

		#endregion

		/// <param name="transactionItems">If it's null, throws an exception. If it's empty, only clears the day. </param>
		public override void ReplaceDailyItems(IList<TransactionItemCommon> transactionItems, TransactionItemType transactionItemType, DateTime date)
		{
			using(var transactionScope = new TransactionScope())
			using(var ctx = Utils.InitContextForMsSql())
			{
				ReplaceDailyItems(ctx, transactionItems, transactionItemType, date);

				transactionScope.Complete();
			}
		}

		protected virtual void ReplaceDailyItems(ExinEfMsSqlContext ctx, IList<TransactionItemCommon> transactionItems, TransactionItemType transactionItemType, DateTime date)
		{
			if(transactionItems == null)
			{
				string msg = Localized.ReplaceDailyItems_method_needs_a_not_null_list__;
				var e = new Exception(msg);
				ExinLog.ger.LogException(msg, e);
				throw e;
			}

			if(transactionItems.Any(transactionItem => transactionItem.Date != date))
			{
				string msg = Localized.ReplaceDailyItems_method_replaces_only_1_day__the_item_s_dates_must_be_the_same__day_;
				var e = new Exception(msg);
				ExinLog.ger.LogException(msg, e);
				throw e;
			}

			ClearDay(ctx, date, transactionItemType);
			InsertMany(ctx, transactionItems);
		}

		#endregion
	}

	public class TransactionItemManagerEfSQLite : TransactionItemManagerEfBase
	{
		#region READ

		public override List<TransactionItemCommon> GetInterval(DateTime fromDate, DateTime toDate, TransactionItemType transactionItemType)
		{
			fromDate = fromDate.Date;
			toDate = toDate.Date;

			using(var ctx = Utils.InitContextForSqlite())
			{
				var query = ctx.TransactionItem
					.Where(ti => ti.Date >= fromDate)
					.Where(ti => ti.Date <= toDate);

				if(transactionItemType == TransactionItemType.Expense)
					query = query.Where(ti => ti.IsExpenseItem);
				else if(transactionItemType == TransactionItemType.Income)
					query = query.Where(ti => ti.IsIncomeItem);
				//else if(transactionItemType == TransactionItemType.Both) <-- No filter predicate

				var result = Utils.ExecRead<TransactionItem_Sqlite, TransactionItemCommon>(query).ToList();
				return result;
			}
		}

		public override List<TransactionItemCommon> GetAll(TransactionItemType? transactionItemType = null)
		{
			using(var ctx = Utils.InitContextForSqlite())
			{
				IQueryable<TransactionItem_Sqlite> query = ctx.TransactionItem;

				if(transactionItemType.HasValue)
					if(transactionItemType == TransactionItemType.Expense)
						query = query.Where(ti => ti.IsExpenseItem);
					else if(transactionItemType == TransactionItemType.Income)
						query = query.Where(ti => ti.IsIncomeItem);

				var result = Utils.ExecRead<TransactionItem_Sqlite, TransactionItemCommon>(query).ToList();
				return result;
			}
		}

		#endregion

		#region CREATE, UPDATE, DELETE

		#region CREATE

		#region Insert

		public override void Insert(TransactionItemCommon transactionItem, bool withId = false)
		{
			using(var ctx = Utils.InitContextForSqlite())
			using(ctx.WithIdentityInsert(ctx.Property(c => c.TransactionItem), activate: withId))
			{
				Insert(ctx, transactionItem, withId);
			}
		}

		public void Insert(ExinEfSqliteContext ctx, TransactionItemCommon transactionItem, bool withId = false)
		{
			try
			{
				Utils.ExecAdd(ctx.TransactionItem, transactionItem, ctx);
			}
			catch(Exception e)
			{
				ExinLog.ger.LogException(Localized.Could_not_insert_the_transaction_item_, e, transactionItem);
				throw;
			}
		}

		#endregion

		#region InsertMany

		public override void InsertMany(IList<TransactionItemCommon> transactionItems, bool withId = false, bool forceOneByOne = false)
		{
			using(var ctx = Utils.InitContextForSqlite())
			using(var transaction = ctx.Database.BeginTransaction())
			{
				InsertMany(ctx, transactionItems, withId);
				transaction.Commit();
			}
		}

		protected virtual void InsertMany(ExinEfSqliteContext ctx, IList<TransactionItemCommon> transactionItems, bool withId = false)
		{
			using(ctx.WithIdentityInsert(ctx.Property(c => c.TransactionItem), activate: withId))
			{
				if (transactionItems == null || transactionItems.Count == 0)
					return;

				if (transactionItems.Count == 1)
				{
					Insert(ctx, transactionItems[0], withId);
					return;
				}
				
				try
				{
					Utils.ExecAddRange(ctx.TransactionItem, transactionItems, ctx);
				}
				catch (Exception e)
				{
					var msg = Localized.Could_not_insert_the_transaction_items_ + transactionItems.Count + Localized._pc_;
					ExinLog.ger.LogException(msg, e, transactionItems);
					throw;
				}
			}
		}

		#endregion

		#endregion

		#region UPDATE

		public override int UpdateFullRecord(TransactionItemCommon transactionItem)
		{
			using(var ctx = Utils.InitContextForSqlite())
			{
				try
				{
					int queryResult;
					Utils.ExecUpdate(ctx.TransactionItem, transactionItem, ctx, out queryResult);
					return queryResult;
				}
				catch(Exception e)
				{
					var msg = Localized.Could_not_update_the_transaction_record__TransactionItem__ID_ + transactionItem.ID + ")" + Environment.NewLine;
					msg += Localized.So_these_modifications_did_not_apply_in_the_database; //data: transactionItem
					ExinLog.ger.LogException(msg, e, transactionItem);
					throw;
				}
			}
		}

		#endregion

		#region DELETE

		#region Delete (1 record)

		public override int Delete(int id)
		{
			using(var ctx = Utils.InitContextForSqlite())
			{
				try
				{
					var item = ctx.TransactionItem.Single(ti => ti.ID == id);
					ctx.TransactionItem.Remove(item);
					int queryResult = ctx.SaveChanges();
					return queryResult;
				}
				catch(Exception e)
				{
					var msg = Localized.Could_not_remove_the_transaction_record__TransactionItem__ID_ + id + ")";
					ExinLog.ger.LogException(msg, e);
					throw;
				}
			}
		}

		#endregion

		#region ClearDay

		public override int ClearDay(DateTime date, TransactionItemType transactionItemType)
		{
			using(var ctx = Utils.InitContextForSqlite())
			using(var transacion = ctx.Database.BeginTransaction())
			{
				int result = ClearDay(ctx, date, transactionItemType);
				transacion.Commit();
				return result;
			}
		}

		protected virtual int ClearDay(ExinEfSqliteContext ctx, DateTime date, TransactionItemType transactionItemType)
		{
			var isExpense = transactionItemType == TransactionItemType.Expense;
			if(isExpense)
				date = date.Date;

			var isIncome = transactionItemType == TransactionItemType.Income;
			if(isIncome)
				date = new DateTime(date.Year, date.Month, 1);

			try
			{
				var itemsToDelete = ctx.TransactionItem
					.Where(ti => ti.Date == date)
					.Where(ti => ti.IsExpenseItem == isExpense)
					.Where(ti => ti.IsIncomeItem == isIncome);

				foreach (var transactionItemSqlite in itemsToDelete)
					ctx.TransactionItem.Remove(transactionItemSqlite);

				int queryResult = ctx.SaveChanges();
				return queryResult;
			}
			catch(Exception e)
			{
				var transactionItemTypeStr = isExpense ? Localized.expenses : isIncome ? Localized.incomes : Localized.transaction_items;
				var msg = string.Format(Localized.Could_not_remove_the_daily_0_at_1__FORMAT__, transactionItemTypeStr, date.ToShortDateString());
				ExinLog.ger.LogException(msg, e);
				throw;
			}
		}

		#endregion

		#endregion

		/// <param name="transactionItems">If it's null, throws an exception. If it's empty, only clears the day. </param>
		public override void ReplaceDailyItems(IList<TransactionItemCommon> transactionItems, TransactionItemType transactionItemType, DateTime date)
		{
			using(var ctx = Utils.InitContextForSqlite())
			using(var transaction = ctx.Database.BeginTransaction())
			{
				ReplaceDailyItems(ctx, transactionItems, transactionItemType, date);

				transaction.Commit();
			}
		}

		protected virtual void ReplaceDailyItems(ExinEfSqliteContext ctx, IList<TransactionItemCommon> transactionItems, TransactionItemType transactionItemType, DateTime date)
		{
			if(transactionItems == null)
			{
				string msg = Localized.ReplaceDailyItems_method_needs_a_not_null_list__;
				var e = new Exception(msg);
				ExinLog.ger.LogException(msg, e);
				throw e;
			}

			if(transactionItems.Any(transactionItem => transactionItem.Date != date))
			{
				string msg = Localized.ReplaceDailyItems_method_replaces_only_1_day__the_item_s_dates_must_be_the_same__day_;
				var e = new Exception(msg);
				ExinLog.ger.LogException(msg, e);
				throw e;
			}

			ClearDay(ctx, date, transactionItemType);
			InsertMany(ctx, transactionItems);
		}

		#endregion
	}


}
