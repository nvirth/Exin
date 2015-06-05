using System;
using System.Data.Entity;
using Common;
using Common.Configuration;
using DAL.DataBase.EntityFramework.EntitiesMsSql;
using DAL.DataBase.EntityFramework.EntitiesSqlite;
using Localization;

namespace DAL.DataBase.EntityFramework
{
	public static class ExinEfContextFactory
	{
		public static DbContext Create()
		{
			switch(Config.DbType)
			{
				case DbType.SQLite:
					return new ExinEfSqliteContext(ExinConnectionString.Get);

				case DbType.MsSql:
					return new ExinEfMsSqlContext(ExinConnectionString.Get);

				default:
					throw new NotImplementedException(string.Format(Localized.ExinEfContextFactory_is_not_implemented_for_this_DbType__FORMAT__, Config.DbType));
			}
		}
	}
}
namespace DAL.DataBase.EntityFramework.EntitiesSqlite
{
	public partial class ExinEfSqliteContext
	{		
		/// <summary>
		/// It creates a new IdentityInsert instance, which implements IDisposable, 
		/// and does nothing but in ctor sets identity insert on (for the specified 
		/// table), and in dtor (dispose) sets it off. 
		/// So you can use this method with 'using(...){...}' context
		/// </summary>
		public virtual IdentityInsertSqlite WithIdentityInsert(string tableName, bool activate)
		{
			return new IdentityInsertSqlite(this, tableName, activate);
		}
	}
}
namespace DAL.DataBase.EntityFramework.EntitiesMsSql
{
	public partial class ExinEfMsSqlContext
	{		
		/// <summary>
		/// It creates a new IdentityInsert instance, which implements IDisposable, 
		/// and does nothing but in ctor sets identity insert on (for the specified 
		/// table), and in dtor (dispose) sets it off. 
		/// So you can use this method with 'using(...){...}' context
		/// </summary>
		public virtual IdentityInsertMsSql WithIdentityInsert(string tableName, bool activate)
		{
			return new IdentityInsertMsSql(this, tableName, activate);
		}
	}
}