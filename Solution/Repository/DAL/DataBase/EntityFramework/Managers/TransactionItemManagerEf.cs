using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Common;
using Common.Configuration;
using Exin.Common.Logging;
using Exin.Common.Logging.Core;
using Common.Utils.Helpers;
using DAL.DataBase.EntityFramework.EntitiesMsSql;
using DAL.DataBase.EntityFramework.EntitiesSqlite;
using DAL.DataBase.Managers;
using DAL.DataBase.Managers.Base;
using DAL.RepoCommon.Interfaces;
using EntityFramework.Extensions;
using Localization;
using TransactionItemCommon = Common.Db.Entities.TransactionItem;

namespace DAL.DataBase.EntityFramework.Managers
{
	public static class TransactionItemManagerEfFactory
	{
		public static TransactionItemManagerEfBase Create(IRepoConfiguration repoConfiguration,
			ICategoryManager categoryManager, IUnitManager unitManager)
		{
			switch(repoConfiguration.DbType)
			{
				case DbType.MsSql:
					return new TransactionItemManagerEfMsSql(repoConfiguration, categoryManager, unitManager);

				case DbType.SQLite:
					return new TransactionItemManagerEfSQLite(repoConfiguration, categoryManager, unitManager);

				default:
					throw new NotImplementedException(string.Format(Localized.TransactionItemManagerEfFactory_is_not_implemented_for_this_DbType__FORMAT__, repoConfiguration.DbType));
			}

		}
	}

	public abstract class TransactionItemManagerEfBase : TransactionItemManagerDbBase
	{
		protected TransactionItemManagerEfBase(IRepoConfiguration repoConfiguration,
			ICategoryManager categoryManager, IUnitManager unitManager) : base(repoConfiguration, categoryManager, unitManager)
		{
		}
	}

	public class TransactionItemManagerEfMsSql : TransactionItemManagerEfBase
	{
		public TransactionItemManagerEfMsSql(IRepoConfiguration repoConfiguration,
			ICategoryManager categoryManager, IUnitManager unitManager) : base(repoConfiguration, categoryManager, unitManager)
		{
		}

		#region READ

		public override DateTime GetFirstDate()
		{
			using(var ctx = Utils.InitContextForMsSql(LocalConfig))
			{
				// Note: Exception if table is empty
				var transactionItem = ctx.TransactionItem
					.OrderBy(ti => ti.Date)
					.First();

				return transactionItem.Date;
			}
		}

		public override DateTime GetLastDate()
		{
			using(var ctx = Utils.InitContextForMsSql(LocalConfig))
			{
				// Note: Exception if table is empty
				var transactionItem = ctx.TransactionItem
					.OrderBy(ti => ti.Date)
					.Last();

				return transactionItem.Date;
			}
		}

		public override List<TransactionItemCommon> GetInterval(DateTime fromDate, DateTime toDate, TransactionItemType transactionItemType)
		{
			fromDate = fromDate.Date;
			toDate = toDate.Date;

			using(var ctx = Utils.InitContextForMsSql(LocalConfig))
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
			using(var ctx = Utils.InitContextForMsSql(LocalConfig))
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

		public override void Insert(TransactionItemCommon transactionItem, bool? withId = null)
		{
			using(var ctx = Utils.InitContextForMsSql(LocalConfig))
			{
				Insert(ctx, transactionItem, withId);
			}
		}

		public void Insert(ExinEfMsSqlContext ctx, TransactionItemCommon transactionItem, bool? withId = null)
		{
			withId = withId ?? LocalConfig.DbInsertId ?? false;

			try
			{
				using(ctx.WithIdentityInsert(ctx.Property(c => c.TransactionItem), activate: withId.Value))
				{
					Utils.ExecAdd(ctx.TransactionItem, transactionItem, ctx);
				}
			}
			catch(Exception e)
			{
				Log.Error(this,
					m => m(Localized.ResourceManager, LocalizedKeys.Could_not_insert_the_transaction_item_),
					LogTarget.All,
					e.WithData(new { transactionItem })
				);
				throw;
			}
		}

		#endregion

		#region InsertMany

		public override void InsertMany(IEnumerable<TransactionItemCommon> transactionItems, bool? withId = null, bool forceOneByOne = false)
		{
			using(var ctx = Utils.InitContextForMsSql(LocalConfig))
			{
				InsertMany(ctx, transactionItems, withId);
			}
		}

		protected virtual void InsertMany(ExinEfMsSqlContext ctx, IEnumerable<TransactionItemCommon> transactionItems, bool? withId = null)
		{
			withId = withId ?? LocalConfig.DbInsertId ?? false;

			using(ctx.WithIdentityInsert(ctx.Property(c => c.TransactionItem), activate: withId.Value))
			{
				var transactionItemsList = transactionItems?.ToList();

				if(transactionItemsList == null || transactionItemsList.Count == 0)
					return;

				if(transactionItemsList.Count == 1)
				{
					Insert(ctx, transactionItemsList[0], withId);
					return;
				}

				using(var transactionScope = new TransactionScope())
				{
					try
					{
						Utils.ExecAddRange(ctx.TransactionItem, transactionItemsList, ctx);
						transactionScope.Complete();
					}
					catch(Exception e)
					{
						Log.Error(this,
							m => m(Localized.ResourceManager, LocalizedKeys.Could_not_insert_the_transaction_items___0__pc_, transactionItemsList.Count),
							LogTarget.All,
							e.WithData(new { transactionItemsList })
						);
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
			using(var ctx = Utils.InitContextForMsSql(LocalConfig))
			{
				try
				{
					int queryResult;
					Utils.ExecUpdate(ctx.TransactionItem, transactionItem, ctx, out queryResult);
					return queryResult;
				}
				catch(Exception e)
				{
					Log.Error(this,
						m => m(Localized.ResourceManager, LocalizedKeys.Could_not_update_the_transaction_record__ID___0____, transactionItem.ID),
						LogTarget.All,
						e.WithData(new {transactionItem})
					);
					throw;
				}
			}
		}

		#endregion

		#region DELETE

		#region Delete (1 record)

		public override int Delete(int id)
		{
			using(var ctx = Utils.InitContextForMsSql(LocalConfig))
			{
				try
				{
					int queryResult = ctx.TransactionItem.Where(ti => ti.ID == id).Delete();
					return queryResult;
				}
				catch(Exception e)
				{
					Log.Error(this,
						m => m(Localized.ResourceManager, LocalizedKeys.Could_not_remove_the_transaction_record__ID___0____, id),
						LogTarget.All,
						e
					);
					throw;
				}
			}
		}

		#endregion

		#region ClearDay

		public override int ClearDay(DateTime date, TransactionItemType transactionItemType)
		{
			using(var ctx = Utils.InitContextForMsSql(LocalConfig))
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
				Log.Error(this,
					(m, c) => {
						var r = Localized.ResourceManager;

						var transactionItemTypeStr = isExpense
							? r.GetString(LocalizedKeys.expenses, c)
							: isIncome
								? r.GetString(LocalizedKeys.incomes, c)
								: r.GetString(LocalizedKeys.transaction_items, c);

						var msg = string.Format(r.GetString(LocalizedKeys.Could_not_remove_the_daily_0_at_1__FORMAT__, c), transactionItemTypeStr, date.ToLocalizedShortDateString());
						return m(msg);
					},
					LogTarget.All,
					e
				);
				throw;
			}
		}

		#endregion

		#endregion

		/// <param name="transactionItems">If it's null, throws an exception. If it's empty, only clears the day. </param>
		public override void ReplaceDailyItems(IEnumerable<TransactionItemCommon> transactionItems, TransactionItemType transactionItemType, DateTime date)
		{
			using(var transactionScope = new TransactionScope())
			using(var ctx = Utils.InitContextForMsSql(LocalConfig))
			{
				ReplaceDailyItems(ctx, transactionItems, transactionItemType, date);

				transactionScope.Complete();
			}
		}

		protected virtual void ReplaceDailyItems(ExinEfMsSqlContext ctx, IEnumerable<TransactionItemCommon> transactionItems, TransactionItemType transactionItemType, DateTime date)
		{
			if(transactionItems == null)
			{
				throw Log.Error(this,
					m => m(Localized.ResourceManager, LocalizedKeys.ReplaceDailyItems_method_needs_a_not_null_list__),
					LogTarget.All,
					new ArgumentNullException("transactionItems", Localized.ReplaceDailyItems_method_needs_a_not_null_list__)
				);
			}
			if(transactionItems.Any(transactionItem => transactionItem.Date != date))
			{
				throw Log.Error(this,
					m => m(Localized.ResourceManager, LocalizedKeys.ReplaceDailyItems_method_replaces_only_1_day__the_item_s_dates_must_be_the_same__day_),
					LogTarget.All,
					new ArgumentException(Localized.ReplaceDailyItems_method_replaces_only_1_day__the_item_s_dates_must_be_the_same__day_)
				);
			}

			ClearDay(ctx, date, transactionItemType);
			InsertMany(ctx, transactionItems);
		}

		#endregion
	}

	public class TransactionItemManagerEfSQLite : TransactionItemManagerEfBase
	{
		public TransactionItemManagerEfSQLite(IRepoConfiguration repoConfiguration,
			ICategoryManager categoryManager, IUnitManager unitManager) : base(repoConfiguration, categoryManager, unitManager)
		{
		}

		#region READ

		public override DateTime GetFirstDate()
		{
			using(var ctx = Utils.InitContextForSqlite(LocalConfig))
			{
				// Note: Exception if table is empty
				var transactionItem = ctx.TransactionItem
					.OrderBy(ti => ti.Date)
					.First();

				return transactionItem.Date;
			}
		}

		public override DateTime GetLastDate()
		{
			using(var ctx = Utils.InitContextForSqlite(LocalConfig))
			{
				// Note: Exception if table is empty
				var transactionItem = ctx.TransactionItem
					.OrderBy(ti => ti.Date)
					.Last();

				return transactionItem.Date;
			}
		}

		public override List<TransactionItemCommon> GetInterval(DateTime fromDate, DateTime toDate, TransactionItemType transactionItemType)
		{
			fromDate = fromDate.Date;
			toDate = toDate.Date;

			using(var ctx = Utils.InitContextForSqlite(LocalConfig))
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
			using(var ctx = Utils.InitContextForSqlite(LocalConfig))
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

		public override void Insert(TransactionItemCommon transactionItem, bool? withId = null)
		{
			using(var ctx = Utils.InitContextForSqlite(LocalConfig))
			{
				Insert(ctx, transactionItem, withId);
			}
		}

		public void Insert(ExinEfSqliteContext ctx, TransactionItemCommon transactionItem, bool? withId = null)
		{
			try
			{
				using(ctx.WithIdentityInsert(ctx.Property(c => c.TransactionItem), activate: withId))
				{
					Utils.ExecAdd(ctx.TransactionItem, transactionItem, ctx);
				}
			}
			catch(Exception e)
			{
				Log.Error(this,
					m => m(Localized.ResourceManager, LocalizedKeys.Could_not_insert_the_transaction_item_),
					LogTarget.All,
					e.WithData(new { transactionItem })
				);
				throw;
			}
		}

		#endregion

		#region InsertMany

		public override void InsertMany(IEnumerable<TransactionItemCommon> transactionItems, bool? withId = null, bool forceOneByOne = false)
		{
			using(var ctx = Utils.InitContextForSqlite(LocalConfig))
			using(var transaction = ctx.Database.BeginTransaction())
			{
				InsertMany(ctx, transactionItems, withId);
				transaction.Commit();
			}
		}

		protected virtual void InsertMany(ExinEfSqliteContext ctx, IEnumerable<TransactionItemCommon> transactionItems, bool? withId = null)
		{
			using(ctx.WithIdentityInsert(ctx.Property(c => c.TransactionItem), activate: withId))
			{
				var transactionItemsList = transactionItems?.ToList();

				if(transactionItemsList == null || transactionItemsList.Count == 0)
					return;

				if (transactionItemsList.Count == 1)
				{
					Insert(ctx, transactionItemsList[0], withId);
					return;
				}
				
				try
				{
					Utils.ExecAddRange(ctx.TransactionItem, transactionItemsList, ctx);
				}
				catch (Exception e)
				{
					Log.Error(this,
						m => m(Localized.ResourceManager, LocalizedKeys.Could_not_insert_the_transaction_items___0__pc_, transactionItemsList.Count),
						LogTarget.All,
						e.WithData(new { transactionItemsList })
					);
					throw;
				}
			}
		}

		#endregion

		#endregion

		#region UPDATE

		public override int UpdateFullRecord(TransactionItemCommon transactionItem)
		{
			using(var ctx = Utils.InitContextForSqlite(LocalConfig))
			{
				try
				{
					int queryResult;
					Utils.ExecUpdate(ctx.TransactionItem, transactionItem, ctx, out queryResult);
					return queryResult;
				}
				catch(Exception e)
				{
					Log.Error(this,
						m => m(Localized.ResourceManager, LocalizedKeys.Could_not_update_the_transaction_record__ID___0____, transactionItem.ID),
						LogTarget.All,
						e.WithData(new { transactionItem })
					);
					throw;
				}
			}
		}

		#endregion

		#region DELETE

		#region Delete (1 record)

		public override int Delete(int id)
		{
			using(var ctx = Utils.InitContextForSqlite(LocalConfig))
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
					Log.Error(this,
						m => m(Localized.ResourceManager, LocalizedKeys.Could_not_remove_the_transaction_record__ID___0____, id),
						LogTarget.All,
						e
					);
					throw;
				}
			}
		}

		#endregion

		#region ClearDay

		public override int ClearDay(DateTime date, TransactionItemType transactionItemType)
		{
			using(var ctx = Utils.InitContextForSqlite(LocalConfig))
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
				// TODO there is at least 4 copy of this log call!
				Log.Error(this,
					(m, c) => {
						var r = Localized.ResourceManager;

						var transactionItemTypeStr = isExpense
							? r.GetString(LocalizedKeys.expenses, c)
							: isIncome
								? r.GetString(LocalizedKeys.incomes, c)
								: r.GetString(LocalizedKeys.transaction_items, c);

						var msg = string.Format(r.GetString(LocalizedKeys.Could_not_remove_the_daily_0_at_1__FORMAT__, c), transactionItemTypeStr, date.ToLocalizedShortDateString());
						return m(msg);
					},
					LogTarget.All,
					e
				);
				throw;
			}
		}

		#endregion

		#endregion

		/// <param name="transactionItems">If it's null, throws an exception. If it's empty, only clears the day. </param>
		public override void ReplaceDailyItems(IEnumerable<TransactionItemCommon> transactionItems, TransactionItemType transactionItemType, DateTime date)
		{
			using(var ctx = Utils.InitContextForSqlite(LocalConfig))
			using(var transaction = ctx.Database.BeginTransaction())
			{
				ReplaceDailyItems(ctx, transactionItems, transactionItemType, date);

				transaction.Commit();
			}
		}

		protected virtual void ReplaceDailyItems(ExinEfSqliteContext ctx, IEnumerable<TransactionItemCommon> transactionItems, TransactionItemType transactionItemType, DateTime date)
		{
			// TODO these checks are not SQLite specific, they should be placed into tha base class (+MsSql)

			if(transactionItems == null)
			{
				throw Log.Error(this,
					m => m(Localized.ResourceManager, LocalizedKeys.ReplaceDailyItems_method_needs_a_not_null_list__),
					LogTarget.All,
					new ArgumentNullException("transactionItems", Localized.ReplaceDailyItems_method_needs_a_not_null_list__)
				);
			}
			if(transactionItems.Any(transactionItem => transactionItem.Date != date))
			{
				throw Log.Error(this,
					m => m(Localized.ResourceManager, LocalizedKeys.ReplaceDailyItems_method_replaces_only_1_day__the_item_s_dates_must_be_the_same__day_),
					LogTarget.All,
					new ArgumentException(Localized.ReplaceDailyItems_method_replaces_only_1_day__the_item_s_dates_must_be_the_same__day_)
				);
			}

			ClearDay(ctx, date, transactionItemType);
			InsertMany(ctx, transactionItems);
		}

		#endregion
	}
}
