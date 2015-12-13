using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Common.Configuration;
using Common.Db.Entities;
using Common.Log;
using Common.Log.Core;
using Common.Utils.Helpers;
using DAL.DataBase.Managers;
using DAL.DataBase.Managers.Base;
using DAL.RepoCommon;
using Localization;
using DbType = Common.DbType;

namespace DAL.DataBase.AdoNet.Managers
{
	public static class CategoryManagerAdoNetFactory
	{
		public static CategoryManagerAdoNetBase Create(IRepoConfiguration repoConfiguration)
		{
			switch(repoConfiguration.DbType)
			{
				case DbType.MsSql:
					return new CategoryManagerAdoNetMsSql(repoConfiguration);

				case DbType.SQLite:
					return new CategoryManagerAdoNetSQLite(repoConfiguration);

				default:
					throw new NotImplementedException(string.Format(Localized.CategoryManagerAdoNetFactory_is_not_implemented_for_this_DbType__FORMAT__, repoConfiguration.DbType));
			}
		}
	}

	public abstract class CategoryManagerAdoNetBase : CategoryManagerDbBase
	{
		protected CategoryManagerAdoNetBase(IRepoConfiguration repoConfiguration) : base(repoConfiguration)
		{
		}

		public const string TableName = "Category";

		#region READ (GetAll)

		// todo FIXME IEnumerable
		public override List<Category> GetAll()
		{
			using(var ctx = ExinAdoNetContextFactory.Create(LocalConfig))
			{
				var categories = GetAll(ctx);
				return categories.ToList();
			}
		}

		protected virtual string BuildSelectAllQuery()
		{
			return "SELECT * FROM " + TableName;
		}

		private IEnumerable<Category> GetAll(ExinAdoNetContextBase ctx)
		{
			ctx.Command.CommandText = BuildSelectAllQuery();
			ctx.Adapter.SelectCommand = ctx.Command;
			ctx.Adapter.Fill(ctx.DataSet);

			var category = new Category();

			var categories = ctx.DataSet.Tables[0].AsEnumerable().Select(
				dr => new Category
				{
					ID = Convert.ToInt32(dr[category.Property(c => c.ID)]),
					Name = dr.Field<string>(category.Property(c => c.Name)),
					DisplayNames = dr.Field<string>(category.Property(c => c.DisplayNames)),
				});
			return categories;
		}

		#endregion

		#region CREATE (Add)

		public override void Add(Category category)
		{
			using(var ctx = ExinAdoNetContextFactory.Create(LocalConfig))
			{
				Add(category, ctx);
			}
		}
		public void Add(Category category, ExinAdoNetContextBase ctx)
		{
			try
			{
				using(ctx.WithIdentityInsert(TableName, activate: LocalConfig.DbInsertId))
				{
					BuildInserQueryWithParams(category, ctx);
					ExecInsertQuery(ctx);
				}
			}
			catch(Exception e)
			{
				ExinLog.ger.LogException(Localized.Could_not_insert_the_Category_record, e);
				throw;
			}
		}

		// This is a standard query
		protected virtual void BuildInserQueryWithParams(Category category, ExinAdoNetContextBase ctx)
		{
			ctx.Command.CommandText = @"INSERT INTO " + TableName + @" 
												  ([ID], [Name], [DisplayNames])
											VALUES(@ID, @Name, @DisplayNames);";

			ctx.Command.Parameters.Clear();
			ctx.Command.Parameters.AddWithValue("@ID", category.ID, LocalConfig.DbType);
			ctx.Command.Parameters.AddWithValue("@Name", category.Name, LocalConfig.DbType);
			ctx.Command.Parameters.AddWithValue("@DisplayNames", category.DisplayNames, LocalConfig.DbType);
		}

		protected virtual void ExecInsertQuery(ExinAdoNetContextBase ctx)
		{
			ctx.Command.ExecuteNonQuery();
		}

		#endregion
	}

	public class CategoryManagerAdoNetMsSql : CategoryManagerAdoNetBase
	{
		// No need to override/expand anything

		public CategoryManagerAdoNetMsSql(IRepoConfiguration repoConfiguration) : base(repoConfiguration)
		{
		}
	}

	public class CategoryManagerAdoNetSQLite : CategoryManagerAdoNetBase
	{
		public CategoryManagerAdoNetSQLite(IRepoConfiguration repoConfiguration) : base(repoConfiguration)
		{
		}

		protected override void BuildInserQueryWithParams(Category category, ExinAdoNetContextBase ctx)
		{
			base.BuildInserQueryWithParams(category, ctx);

			if(!ctx.IsIdentityInsertOn)
			{
				ctx.Command.Parameters.RemoveAt("@ID");
				ctx.Command.Parameters.AddWithValue("@ID", null, LocalConfig.DbType);
			}
		}

		protected override void ExecInsertQuery(ExinAdoNetContextBase ctx)
		{
			ctx.ExecInTransactionWithCommit();
		}
	}
}
