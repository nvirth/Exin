using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Transactions;
using AutoMapper;
using Common;
using Common.DbEntities;
using Common.Log;
using Common.UiModels.WPF;
using DAL.DataBase.EntityFramework.EntitiesMsSql;
using DAL.DataBase.EntityFramework.EntitiesSqlite;
using DAL.DataBase.Managers;
using Localization;
using EntityFramework.Extensions;
using Config = Common.Config.Config;
using DbType = Common.DbType;
using SummaryItemCommon = Common.DbEntities.SummaryItem;

namespace DAL.DataBase.EntityFramework.Managers
{
	public static class SummaryItemManagerEfFactory
	{
		public static SummaryItemManagerEfBase Create()
		{
			switch(Config.DbType)
			{
				case DbType.MsSql:
					return new SummaryItemManagerEfMsSql();

				case DbType.SQLite:
					return new SummaryItemManagerEfSQLite();

				default:
					throw new NotImplementedException(string.Format(Localized.SummaryItemManagerEfBase_is_not_implemented_for_this_DbType__FORMAT__, Config.DbType));
			}
		}
	}

	public abstract class SummaryItemManagerEfBase : SummaryItemManagerCommonBase
	{
		#region UPDATE

		protected abstract void InserOrUpdateSummary_Exec_Core(Summary summary, DateTime date, bool isExpense, out List<SummaryItemCommon> insertItems);

		protected override void InserOrUpdateSummary_Exec(Summary summary, DateTime date, TransactionItemType transactionItemType)
		{
			var isExpense = transactionItemType == TransactionItemType.Expense;
			List<SummaryItemCommon> insertItems = null;

			try
			{
				InserOrUpdateSummary_Exec_Core(summary, date, isExpense, out insertItems);
			}
			catch(Exception e)
			{
				ExinLog.ger.LogException(Localized.SummaryItem_InsertOrUpdate_failed__, e,
					new
					{
						LostItems = insertItems
					});
				throw;
			}
		}

		#endregion
	}

	public class SummaryItemManagerEfSQLite : SummaryItemManagerEfBase
	{
		#region READ

		public override List<SummaryItemCommon> GetAll()
		{
			return Get(item => true);
		}

		public override List<SummaryItemCommon> GetInterval_Exec(DateTime fromDate, DateTime toDate)
		{
			return Get(item => item.Date >= fromDate && item.Date <= toDate);
		}

		private List<SummaryItemCommon> Get(Expression<Func<SummaryItem_Sqlite, bool>> filterPredicate)
		{
			// Additional Map for AutoMapper
			InitAutoMapperForEf.Init<Category_Sqlite, Category>();

			using(var ctx = Utils.InitContextForSqlite())
			{
				var readQuery = ctx.SummaryItem.Where(filterPredicate);
				var result = Utils.ExecRead<SummaryItem_Sqlite, SummaryItemCommon>(readQuery).ToList();
				return result;
			}
		}

		#endregion

		#region INSERT, UPDATE

		protected override void InserOrUpdateSummary_Exec_Core(Summary summary, DateTime date, bool isExpense, out List<SummaryItemCommon> insertItems)
		{
			using(var ctx = Utils.InitContextForSqlite())
			using (var transaction = ctx.Database.BeginTransaction())
			{
				// Delete all summaries from this day
				var itemsToDelete = ctx.SummaryItem
					.Where(si => si.Date == date) // isExpense != 2; isIncome == 2
					.Where(si => isExpense || si.CategoryID == CategoryManager.GetCategoryFullIncomeSummary.ID)
					.Where(si => !isExpense || si.CategoryID != CategoryManager.GetCategoryFullIncomeSummary.ID);
				
				foreach (var summaryItemSqlite in itemsToDelete)
					ctx.SummaryItem.Remove(summaryItemSqlite);

				// Insert new data
				insertItems = CollectSummariesToInsert(summary, date, isExpense);
				Utils.ExecAddRange(ctx.SummaryItem, insertItems);

				// Commit all the changes same time
				ctx.SaveChanges();
				transaction.Commit();
			}
		}

		#endregion
	}

	public class SummaryItemManagerEfMsSql : SummaryItemManagerEfBase
	{
		#region READ

		public override List<SummaryItemCommon> GetAll()
		{
			return Get(item => true);
		}

		public override List<SummaryItemCommon> GetInterval_Exec(DateTime fromDate, DateTime toDate)
		{
			return Get(item => item.Date >= fromDate && item.Date <= toDate);
		}

		private List<SummaryItemCommon> Get(Expression<Func<SummaryItem_MsSql, bool>> filterPredicate)
		{
			// Additional Map for AutoMapper
			InitAutoMapperForEf.Init<Category_MsSql, Category>();

			using(var ctx = Utils.InitContextForMsSql())
			{
				var readQuery = ctx.SummaryItem.Where(filterPredicate);
				var result = Utils.ExecRead<SummaryItem_MsSql, SummaryItemCommon>(readQuery).ToList();
				return result;
			}
		}

		#endregion

		#region INSERT, UPDATE

		protected override void InserOrUpdateSummary_Exec_Core(Summary summary, DateTime date, bool isExpense, out List<SummaryItemCommon> insertItems)
		{
			using(var ctx = Utils.InitContextForMsSql())
			using(var transactionScope = new TransactionScope())
			{
				// Delete all summaries from this day
				ctx.SummaryItem
					.Where(si => si.Date == date) // isExpense != 2; isIncome == 2
					.Where(si => isExpense || si.CategoryID == CategoryManager.GetCategoryFullIncomeSummary.ID)
					.Where(si => !isExpense || si.CategoryID != CategoryManager.GetCategoryFullIncomeSummary.ID)
					.Delete(); // !!! Executed immediately

				// Insert new data
				insertItems = CollectSummariesToInsert(summary, date, isExpense);
				
				Utils.ExecAddRange(ctx.SummaryItem, insertItems, ctx);
				transactionScope.Complete();
			}
		}

		#endregion
	}
}
