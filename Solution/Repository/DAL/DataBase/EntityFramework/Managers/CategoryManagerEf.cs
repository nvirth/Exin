using System;
using System.Collections.Generic;
using System.Data.Entity;
using Common;
using Common.Log;
using DAL.DataBase.EntityFramework.EntitiesMsSql;
using DAL.DataBase.EntityFramework.EntitiesSqlite;
using DAL.DataBase.Managers;
using Localization;
using CategoryCommon = Common.DbEntities.Category;
using Config = Common.Config.Config;

namespace DAL.DataBase.EntityFramework.Managers
{
	public static class CategoryManagerEfFactory
	{
		public static CategoryManagerEfBase Create()
		{
			switch(Config.DbType)
			{
				case DbType.MsSql:
					return new CategoryManagerEfMsSql();

				case DbType.SQLite:
					return new CategoryManagerEfSqlite();

				default:
					throw new NotImplementedException(string.Format(Localized.CategoryManagerEfFactory_is_not_implemented_for_this_DbType__FORMAT__, Config.DbType));
			}
		}
	}

	public abstract	class CategoryManagerEfBase : CategoryManagerCommonBase
	{
		#region Cache

		protected override void RefreshCache_FromDb()
		{
			using(var ctx = ExinEfContextFactory.Create())
			{
				var categories = RefreshCache_GetFromDb(ctx);
				RefreshCache_Refresh(categories);
			}
		}

		protected abstract IEnumerable<CategoryCommon> RefreshCache_GetFromDb(DbContext dbContext);

		#endregion

		#region CREATE

		public override void Add(CategoryCommon category)
		{
			using(var ctx = ExinEfContextFactory.Create())
			{
				Add(category, ctx);
			}
		}

		public void Add(CategoryCommon category, DbContext ctx)
		{
			CheckExistsInCache(category);

			try
			{
				AddCategory(category, ctx);
				ctx.SaveChanges();

				AddToCache(category);
			}
			catch(Exception e)
			{
				ExinLog.ger.LogException(Localized.Could_not_insert_the_Category_record, e);
				throw;
			}
		}

		protected abstract void AddCategory(CategoryCommon categoryCommon, DbContext dbContext);

		#endregion
	}

	public class CategoryManagerEfSqlite : CategoryManagerEfBase
	{
		protected override IEnumerable<CategoryCommon> RefreshCache_GetFromDb(DbContext dbContext)
		{
			var ctx = Utils.InitContextForSqlite(dbContext);
			var categories = Utils.ExecRead<Category_Sqlite, CategoryCommon>(ctx.Category);
			return categories;
		}

		protected override void AddCategory(CategoryCommon categoryCommon, DbContext dbContext)
		{
			var ctx = Utils.InitContextForSqlite(dbContext);
			Utils.ExecAdd(ctx.Category, categoryCommon);
		}
	}

	public class CategoryManagerEfMsSql : CategoryManagerEfBase
	{
		protected override IEnumerable<CategoryCommon> RefreshCache_GetFromDb(DbContext dbContext)
		{
			var ctx = Utils.InitContextForMsSql(dbContext);
			var categories  = Utils.ExecRead<Category_MsSql, CategoryCommon>(ctx.Category);
			return categories;
		}

		protected override void AddCategory(CategoryCommon categoryCommon, DbContext dbContext)
		{
			var ctx = Utils.InitContextForMsSql(dbContext);
			Utils.ExecAdd(ctx.Category, categoryCommon);
		}
	}
}
