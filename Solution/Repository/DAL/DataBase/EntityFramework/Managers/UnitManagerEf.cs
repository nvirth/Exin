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
using UnitCommon = Common.Db.Entities.Unit;

namespace DAL.DataBase.EntityFramework.Managers
{
	public static class UnitManagerEfFactory
	{
		public static UnitManagerEfBase Create(IRepoConfiguration repoConfiguration)
		{
			switch(repoConfiguration.DbType)
			{
				case DbType.MsSql:
					return new UnitManagerEfMsSql(repoConfiguration);

				case DbType.SQLite:
					return new UnitManagerEfSqlite(repoConfiguration);

				default:
					throw new NotImplementedException(string.Format(Localized.UnitManagerEfFactory_is_not_implemented_for_this_DbType__FORMAT__, repoConfiguration.DbType));
			}

		}
	}

	public abstract class UnitManagerEfBase : UnitManagerCommonBase
	{
		protected UnitManagerEfBase(IRepoConfiguration repoConfiguration) : base(repoConfiguration)
		{
		}

		#region Cache

		protected override void RefreshCache_FromDb()
		{
			using(var ctx = ExinEfContextFactory.Create(LocalConfig))
			{
				var units = RefreshCache_GetFromDb(ctx);
				RefreshCache_Refresh(units);
			}
		}

		protected abstract IEnumerable<UnitCommon> RefreshCache_GetFromDb(DbContext dbContext);

		#endregion

		#region CREATE

		public override void Add(UnitCommon unit)
		{
			using(var ctx = ExinEfContextFactory.Create(LocalConfig))
			{
				Add(unit, ctx);
			}
		}

		public void Add(UnitCommon unit, DbContext ctx)
		{
			CheckExistsInCache(unit);

			try
			{
				AddUnit(unit, ctx);
				ctx.SaveChanges();

				AddToCache(unit);
			}
			catch(Exception e)
			{
				ExinLog.ger.LogException(Localized.Could_not_insert_the_Unit_record, e);
				throw;
			}
		}

		protected abstract void AddUnit(UnitCommon unitCommon, DbContext dbContext);

		#endregion
	}

	public class UnitManagerEfSqlite : UnitManagerEfBase
	{
		public UnitManagerEfSqlite(IRepoConfiguration repoConfiguration) : base(repoConfiguration)
		{
		}

		protected override IEnumerable<UnitCommon> RefreshCache_GetFromDb(DbContext dbContext)
		{
			var ctx = Utils.InitContextForSqlite(dbContext, LocalConfig);
			var units = Utils.ExecRead<Unit_Sqlite, UnitCommon>(ctx.Unit);
			return units;
		}

		protected override void AddUnit(UnitCommon unitCommon, DbContext dbContext)
		{
			var ctx = Utils.InitContextForSqlite(dbContext, LocalConfig);
			Utils.ExecAdd(ctx.Unit, unitCommon);
		}
	}

	public class UnitManagerEfMsSql : UnitManagerEfBase
	{
		public UnitManagerEfMsSql(IRepoConfiguration repoConfiguration) : base(repoConfiguration)
		{
		}

		protected override IEnumerable<UnitCommon> RefreshCache_GetFromDb(DbContext dbContext)
		{
			var ctx = Utils.InitContextForMsSql(dbContext, LocalConfig);
			var units = Utils.ExecRead<Unit_MsSql, UnitCommon>(ctx.Unit);
			return units;
		}

		protected override void AddUnit(UnitCommon unitCommon, DbContext dbContext)
		{
			var ctx = Utils.InitContextForMsSql(dbContext, LocalConfig);
			Utils.ExecAdd(ctx.Unit, unitCommon);
		}
	}
}
