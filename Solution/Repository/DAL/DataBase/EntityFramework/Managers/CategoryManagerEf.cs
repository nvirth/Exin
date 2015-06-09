using System;
using System.Collections.Generic;
using System.Data.Entity;
using Common;
using Common.Configuration;
using Common.Log;
using DAL.DataBase.EntityFramework.EntitiesMsSql;
using DAL.DataBase.EntityFramework.EntitiesSqlite;
using DAL.DataBase.Managers;
using Localization;
using CategoryCommon = Common.Db.Entities.Category;

namespace DAL.DataBase.EntityFramework.Managers
{
	public static class CategoryManagerEfFactory
	{
		public static CategoryManagerEfBase Create(IRepoConfiguration repoConfiguration)
		{
			switch(repoConfiguration.DbType)
			{
				case DbType.MsSql:
					return new CategoryManagerEfMsSql(repoConfiguration);

				case DbType.SQLite:
					return new CategoryManagerEfSqlite(repoConfiguration);

				default:
					throw new NotImplementedException(string.Format(Localized.CategoryManagerEfFactory_is_not_implemented_for_this_DbType__FORMAT__, repoConfiguration.DbType));
			}
		}
	}

	public abstract	class CategoryManagerEfBase : CategoryManagerCommonBase
	{
		protected CategoryManagerEfBase(IRepoConfiguration repoConfiguration) : base(repoConfiguration)
		{
		}
		
		#region Cache

		protected override void RefreshCache_FromDb()
		{
			using(var ctx = ExinEfContextFactory.Create(LocalConfig))
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
			using(var ctx = ExinEfContextFactory.Create(LocalConfig))
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
		public CategoryManagerEfSqlite(IRepoConfiguration repoConfiguration) : base(repoConfiguration)
		{
		}

		protected override IEnumerable<CategoryCommon> RefreshCache_GetFromDb(DbContext dbContext)
		{
			var ctx = Utils.InitContextForSqlite(dbContext, LocalConfig);
			var categories = Utils.ExecRead<Category_Sqlite, CategoryCommon>(ctx.Category);
			return categories;
		}

		protected override void AddCategory(CategoryCommon categoryCommon, DbContext dbContext)
		{
			var ctx = Utils.InitContextForSqlite(dbContext, LocalConfig);
			Utils.ExecAdd(ctx.Category, categoryCommon);
		}
	}

	public class CategoryManagerEfMsSql : CategoryManagerEfBase
	{
		public CategoryManagerEfMsSql(IRepoConfiguration repoConfiguration) : base(repoConfiguration)
		{
		}

		protected override IEnumerable<CategoryCommon> RefreshCache_GetFromDb(DbContext dbContext)
		{
			var ctx = Utils.InitContextForMsSql(dbContext, LocalConfig);
			var categories  = Utils.ExecRead<Category_MsSql, CategoryCommon>(ctx.Category);
			return categories;
		}

		protected override void AddCategory(CategoryCommon categoryCommon, DbContext dbContext)
		{
			var ctx = Utils.InitContextForMsSql(dbContext, LocalConfig);
			Utils.ExecAdd(ctx.Category, categoryCommon);
		}
	}
}
