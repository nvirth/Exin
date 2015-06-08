using System;
using System.Collections.Generic;
using System.Data;
using Common.Db.Entities;
using Common.Log;
using Common.Utils.Helpers;
using DAL.DataBase.Managers;
using Localization;
using Config = Common.Configuration.Config;
using DbType = Common.DbType;

namespace DAL.DataBase.AdoNet.Managers
{
	public static class CategoryManagerAdoNetFactory
	{
		public static CategoryManagerAdoNetBase Create()
		{
			switch(Config.DbType)
			{
				case DbType.MsSql:
					return new CategoryManagerAdoNetMsSql();

				case DbType.SQLite:
					return new CategoryManagerAdoNetSQLite();

				default:
					throw new NotImplementedException(string.Format(Localized.CategoryManagerAdoNetFactory_is_not_implemented_for_this_DbType__FORMAT__, Config.DbType));
			}
		}
	}

	public abstract class CategoryManagerAdoNetBase : CategoryManagerCommonBase
	{
		public const string TableName = "Category";

		#region Cache

		protected override void RefreshCache_FromDb()
		{
			using(var ctx = ExinAdoNetContextFactory.Create())
			{
				var categories = RefreshCache_GetFromDb(ctx);
				RefreshCache_Refresh(categories);
			}
		}

		protected virtual string BuildSelectAllQuery()
		{
			return "SELECT * FROM " + TableName;
		}

		private IEnumerable<Category> RefreshCache_GetFromDb(ExinAdoNetContextBase ctx)
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

		#region CREATE

		public override void Add(Category category)
		{
			using(var ctx = ExinAdoNetContextFactory.Create())
			{
				Add(category, ctx);
			}
		}

		// This is a standard query
		protected virtual void BuildInserQueryWithParams(Category category, ExinAdoNetContextBase ctx)
		{
			ctx.Command.CommandText = @"INSERT INTO " + TableName + @" 
												  ([ID], [Name], [DisplayNames])
											VALUES(@ID, @Name, @DisplayNames);";

			ctx.Command.Parameters.Clear();
			ctx.Command.Parameters.AddWithValue("@ID", category.ID);
			ctx.Command.Parameters.AddWithValue("@Name", category.Name);
			ctx.Command.Parameters.AddWithValue("@DisplayNames", category.DisplayNames);
		}

		protected virtual void ExecInsertQuery(ExinAdoNetContextBase ctx)
		{
			ctx.Command.ExecuteNonQuery();
		}

		public void Add(Category category, ExinAdoNetContextBase ctx)
		{
			CheckExistsInCache(category);

			try
			{
				BuildInserQueryWithParams(category, ctx);
				ExecInsertQuery(ctx);

				AddToCache(category);
			}
			catch(Exception e)
			{
				ExinLog.ger.LogException(Localized.Could_not_insert_the_Category_record, e);
				throw;
			}
		}

		#endregion
	}

	public class CategoryManagerAdoNetMsSql : CategoryManagerAdoNetBase
	{
		// No need to override/expand anything
	}

	public class CategoryManagerAdoNetSQLite : CategoryManagerAdoNetBase
	{
		protected override void BuildInserQueryWithParams(Category category, ExinAdoNetContextBase ctx)
		{
			base.BuildInserQueryWithParams(category, ctx);

			if(!ctx.IsIdentityInsertOn)
			{
				ctx.Command.Parameters.RemoveAt("@ID");
				ctx.Command.Parameters.AddWithValue("@ID", null);
			}
		}

		protected override void ExecInsertQuery(ExinAdoNetContextBase ctx)
		{
			ctx.ExecInTransactionWithCommit();
		}
	}
}
