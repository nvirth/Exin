using System;
using System.Collections.Generic;
using System.Data.Entity;
using Common;
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
		public static UnitManagerEfBase Create(DbType dbType, DbAccessMode dbAccessMode)
		{
			switch(dbType)
			{
				case DbType.MsSql:
					return new UnitManagerEfMsSql(dbType, dbAccessMode);

				case DbType.SQLite:
					return new UnitManagerEfSqlite(dbType, dbAccessMode);

				default:
					throw new NotImplementedException(string.Format(Localized.UnitManagerEfFactory_is_not_implemented_for_this_DbType__FORMAT__, dbType));
			}

		}
	}

	public abstract class UnitManagerEfBase : UnitManagerCommonBase
	{
		protected UnitManagerEfBase(DbType dbType, DbAccessMode dbAccessMode) : base(dbType, dbAccessMode)
		{
		}

		#region Cache

		protected override void RefreshCache_FromDb()
		{
			using(var ctx = ExinEfContextFactory.Create(DbType, DbAccessMode))
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
			using(var ctx = ExinEfContextFactory.Create(DbType, DbAccessMode))
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
		public UnitManagerEfSqlite(DbType dbType, DbAccessMode dbAccessMode) : base(dbType, dbAccessMode)
		{
		}

		protected override IEnumerable<UnitCommon> RefreshCache_GetFromDb(DbContext dbContext)
		{
			var ctx = Utils.InitContextForSqlite(dbContext, DbType);
			var units = Utils.ExecRead<Unit_Sqlite, UnitCommon>(ctx.Unit);
			return units;
		}

		protected override void AddUnit(UnitCommon unitCommon, DbContext dbContext)
		{
			var ctx = Utils.InitContextForSqlite(dbContext, DbType);
			Utils.ExecAdd(ctx.Unit, unitCommon);
		}
	}

	public class UnitManagerEfMsSql : UnitManagerEfBase
	{
		public UnitManagerEfMsSql(DbType dbType, DbAccessMode dbAccessMode) : base(dbType, dbAccessMode)
		{
		}

		protected override IEnumerable<UnitCommon> RefreshCache_GetFromDb(DbContext dbContext)
		{
			var ctx = Utils.InitContextForMsSql(dbContext, DbType);
			var units = Utils.ExecRead<Unit_MsSql, UnitCommon>(ctx.Unit);
			return units;
		}

		protected override void AddUnit(UnitCommon unitCommon, DbContext dbContext)
		{
			var ctx = Utils.InitContextForMsSql(dbContext, DbType);
			Utils.ExecAdd(ctx.Unit, unitCommon);
		}
	}
}
