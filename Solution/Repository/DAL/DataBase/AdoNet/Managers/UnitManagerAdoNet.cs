using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Common.Configuration;
using Common.Db.Entities;
using Common.Log;
using Common.Utils.Helpers;
using DAL.DataBase.Managers;
using DAL.DataBase.Managers.Base;
using DAL.RepoCommon;
using Localization;
using DbType = Common.DbType;

namespace DAL.DataBase.AdoNet.Managers
{
	public static class UnitManagerAdoNetFactory
	{
		public static UnitManagerAdoNetBase Create(IRepoConfiguration repoConfiguration)
		{
			switch(repoConfiguration.DbType)
			{
				case DbType.MsSql:
					return new UnitManagerAdoNetMsSql(repoConfiguration);

				case DbType.SQLite:
					return new UnitManagerAdoNetSQLite(repoConfiguration);

				default:
					throw new NotImplementedException(string.Format(Localized.UnitManagerAdoNetFactory_is_not_implemented_for_this_DbType__FORMAT__, repoConfiguration.DbType));
			}

		}
	}

	public abstract class UnitManagerAdoNetBase : UnitManagerDbBase
	{
		protected UnitManagerAdoNetBase(IRepoConfiguration repoConfiguration) : base(repoConfiguration)
		{
		}

		public const string TableName = "Unit";

		#region READ (GetAll)

		// TODO FIXME IEnumerable
		public override List<Unit> GetAll()
		{
			using(var ctx = ExinAdoNetContextFactory.Create(LocalConfig))
			{
				var units = GetAll(ctx);
				return units.ToList();
			}
		}

		protected virtual string BuildSelectAllQuery()
		{
			return "SELECT * FROM " + TableName;
		}

		private IEnumerable<Unit> GetAll(ExinAdoNetContextBase ctx)
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
					DisplayNames = dr.Field<string>(unit.Property(c => c.DisplayNames)),
				});
			return units;
		}

		#endregion

		#region CREATE (Add)

		public override void Add(Unit unit)
		{
			using(var ctx = ExinAdoNetContextFactory.Create(LocalConfig))
			{
				Add(unit, ctx);
			}
		}

		// This is a standard query
		protected virtual void BuildInserQueryWithParams(Unit unit, ExinAdoNetContextBase ctx)
		{
			ctx.Command.CommandText = @"INSERT INTO " + TableName + @" 
												  ([ID], [Name], [DisplayNames])
											VALUES(@ID, @Name, @DisplayNames);";

			ctx.Command.Parameters.Clear();
			ctx.Command.Parameters.AddWithValue("@ID", unit.ID, LocalConfig.DbType);
			ctx.Command.Parameters.AddWithValue("@Name", unit.Name, LocalConfig.DbType);
			ctx.Command.Parameters.AddWithValue("@DisplayNames", unit.DisplayNames, LocalConfig.DbType);
		}

		protected virtual void ExecInsertQuery(ExinAdoNetContextBase ctx)
		{
			ctx.Command.ExecuteNonQuery();
		}

		public void Add(Unit unit, ExinAdoNetContextBase ctx)
		{
			try
			{
				BuildInserQueryWithParams(unit, ctx);
				ExecInsertQuery(ctx);
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

		public UnitManagerAdoNetMsSql(IRepoConfiguration repoConfiguration) : base(repoConfiguration)
		{
		}
	}

	public class UnitManagerAdoNetSQLite : UnitManagerAdoNetBase
	{
		public UnitManagerAdoNetSQLite(IRepoConfiguration repoConfiguration) : base(repoConfiguration)
		{
		}

		protected override void BuildInserQueryWithParams(Unit unit, ExinAdoNetContextBase ctx)
		{
			base.BuildInserQueryWithParams(unit, ctx);

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
