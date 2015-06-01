using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using Common;
using Common.DbEntities;
using Common.Log;
using Common.Utils;
using Common.Utils.Helpers;
using DAL.DataBase.AdoNet.Managers.Base;
using DAL.DataBase.Managers;
using DAL.FileRepo;
using Localization;
using Config = Common.Config.Config;
using DbType = Common.DbType;

namespace DAL.DataBase.AdoNet.Managers
{
	public static class UnitManagerAdoNetFactory
	{
		public static UnitManagerAdoNetBase Create()
		{
			switch(Config.DbType)
			{
				case DbType.MsSql:
					return new UnitManagerAdoNetMsSql();

				case DbType.SQLite:
					return new UnitManagerAdoNetSQLite();

				default:
					throw new NotImplementedException(string.Format(Localized.UnitManagerAdoNetFactory_is_not_implemented_for_this_DbType__FORMAT__, Config.DbType));
			}

		}
	}

	public abstract class UnitManagerAdoNetBase : UnitManagerCommonBase
	{
		public const string TableName = "Unit";

		#region Cache

		protected override void RefreshCache_FromDb()
		{
			using(var ctx = ExinAdoNetContextFactory.Create())
			{
				var units = RefreshCache_GetFromDb(ctx);
				RefreshCache_Refresh(units);
			}
		}

		protected virtual string BuildSelectAllQuery()
		{
			return "SELECT * FROM " + TableName;
		}

		private IEnumerable<Unit> RefreshCache_GetFromDb(ExinAdoNetContextBase ctx)
		{
			ctx.Command.CommandText = BuildSelectAllQuery();
			ctx.Adapter.SelectCommand = ctx.Command;
			ctx.Adapter.Fill(ctx.DataSet);

			var unit = new Unit();
			var units = ctx.DataSet.Tables[0].AsEnumerable().Select(
				dr => new Unit
				{
					ID = Convert.ToInt32(dr[unit.Property(c => c.ID)]),
					Name = dr.Field<string>(unit.Property(c => c.Name)),
					DisplayName = dr.Field<string>(unit.Property(c => c.DisplayName)),
				});
			return units;
		}

		#endregion

		#region CREATE

		public override void Add(Unit unit)
		{
			using(var ctx = ExinAdoNetContextFactory.Create())
			{
				Add(unit, ctx);
			}
		}

		// This is a standard query
		protected virtual void BuildInserQueryWithParams(Unit unit, ExinAdoNetContextBase ctx)
		{
			ctx.Command.CommandText = @"INSERT INTO " + TableName + @" 
												  ([ID], [Name], [DisplayName])
											VALUES(@ID, @Name, @DisplayName);";

			ctx.Command.Parameters.Clear();
			ctx.Command.Parameters.AddWithValue("@ID", unit.ID);
			ctx.Command.Parameters.AddWithValue("@Name", unit.Name);
			ctx.Command.Parameters.AddWithValue("@DisplayName", unit.DisplayName);
		}

		protected virtual void ExecInsertQuery(ExinAdoNetContextBase ctx)
		{
			ctx.Command.ExecuteNonQuery();
		}

		public void Add(Unit unit, ExinAdoNetContextBase ctx)
		{
			CheckExistsInCache(unit);

			try
			{
				BuildInserQueryWithParams(unit, ctx);
				ExecInsertQuery(ctx);

				AddToCache(unit);
			}
			catch(Exception e)
			{
				ExinLog.ger.LogException(Localized.Could_not_insert_the_Unit_record, e);
				throw;
			}
		}

		#endregion
	}

	public class UnitManagerAdoNetMsSql : UnitManagerAdoNetBase
	{
		// No need to override/expand anything
	}

	public class UnitManagerAdoNetSQLite : UnitManagerAdoNetBase
	{
		protected override void BuildInserQueryWithParams(Unit unit, ExinAdoNetContextBase ctx)
		{
			base.BuildInserQueryWithParams(unit, ctx);

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
