﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;
using Common;
using Common.Configuration;
using Common.Db.Entities;
using Exin.Common.Logging;
using Exin.Common.Logging.Core;
using Common.UiModels.WPF;
using DAL.DataBase.EntityFramework.EntitiesMsSql;
using DAL.DataBase.EntityFramework.EntitiesSqlite;
using DAL.DataBase.Managers;
using DAL.DataBase.Managers.Base;
using DAL.RepoCommon.Interfaces;
using EntityFramework.Extensions;
using Localization;
using SummaryItemCommon = Common.Db.Entities.SummaryItem;

namespace DAL.DataBase.EntityFramework.Managers
{
	public static class SummaryItemManagerEfFactory
	{
		public static SummaryItemManagerEfBase Create(IRepoConfiguration repoConfiguration, ICategoryManager categoryManager)
		{
			switch(repoConfiguration.DbType)
			{
				case DbType.MsSql:
					return new SummaryItemManagerEfMsSql(repoConfiguration, categoryManager);

				case DbType.SQLite:
					return new SummaryItemManagerEfSQLite(repoConfiguration, categoryManager);

				default:
					throw new NotImplementedException(string.Format(Localized.SummaryItemManagerEfBase_is_not_implemented_for_this_DbType__FORMAT__, repoConfiguration.DbType));
			}
		}
	}

	public abstract class SummaryItemManagerEfBase : SummaryItemManagerDbBase
	{
		protected SummaryItemManagerEfBase(IRepoConfiguration repoConfiguration, ICategoryManager categoryManager) : base(repoConfiguration, categoryManager)
		{
		}

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
				throw Log.Error(this,
					m => m(Localized.ResourceManager, LocalizedKeys.SummaryItem_InsertOrUpdate_failed__),
					LogTarget.All,
					e.WithData(new { LostItems = insertItems })
				);
			}
		}

		#endregion
	}

	public class SummaryItemManagerEfSQLite : SummaryItemManagerEfBase
	{
		public SummaryItemManagerEfSQLite(IRepoConfiguration repoConfiguration, ICategoryManager categoryManager) : base(repoConfiguration, categoryManager)
		{
		}

		#region READ

		public override List<SummaryItemCommon> GetAll()
		{
			return Get(item => true);
		}

		protected override List<SummaryItemCommon> GetInterval_Exec(DateTime fromDate, DateTime toDate)
		{
			return Get(item => item.Date >= fromDate && item.Date <= toDate);
		}

		private List<SummaryItemCommon> Get(Expression<Func<SummaryItem_Sqlite, bool>> filterPredicate)
		{
			// Additional Map for AutoMapper
			InitAutoMapperForEf.Init<Category_Sqlite, Category>();

			using(var ctx = Utils.InitContextForSqlite(LocalConfig))
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
			using(var ctx = Utils.InitContextForSqlite(LocalConfig))
			using (var transaction = ctx.Database.BeginTransaction())
			{
				// Delete all summaries from this day
				var itemsToDelete = ctx.SummaryItem
					.Where(si => si.Date == date) // isExpense != 2; isIncome == 2
					.Where(si => isExpense || si.CategoryID == CategoryManagerLocal.GetCategoryFullIncomeSummary.ID)
					.Where(si => !isExpense || si.CategoryID != CategoryManagerLocal.GetCategoryFullIncomeSummary.ID);
				
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
		public SummaryItemManagerEfMsSql(IRepoConfiguration repoConfiguration, ICategoryManager categoryManager) : base(repoConfiguration, categoryManager)
		{
		}

		#region READ

		public override List<SummaryItemCommon> GetAll()
		{
			return Get(item => true);
		}

		protected override List<SummaryItemCommon> GetInterval_Exec(DateTime fromDate, DateTime toDate)
		{
			return Get(item => item.Date >= fromDate && item.Date <= toDate);
		}

		private List<SummaryItemCommon> Get(Expression<Func<SummaryItem_MsSql, bool>> filterPredicate)
		{
			// Additional Map for AutoMapper
			InitAutoMapperForEf.Init<Category_MsSql, Category>();

			using(var ctx = Utils.InitContextForMsSql(LocalConfig))
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
			using(var ctx = Utils.InitContextForMsSql(LocalConfig))
			using(var transactionScope = new TransactionScope())
			{
				// Delete all summaries from this day
				ctx.SummaryItem
					.Where(si => si.Date == date) // isExpense != 2; isIncome == 2
					.Where(si => isExpense || si.CategoryID == CategoryManagerLocal.GetCategoryFullIncomeSummary.ID)
					.Where(si => !isExpense || si.CategoryID != CategoryManagerLocal.GetCategoryFullIncomeSummary.ID)
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
