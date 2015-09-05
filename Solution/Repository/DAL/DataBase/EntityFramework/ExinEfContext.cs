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
		public static DbContext Create(IRepoConfiguration repoConfiguration)
		{
			switch(repoConfiguration.DbType)
			{
				case DbType.SQLite:
					return new ExinEfSqliteContext(repoConfiguration);

				case DbType.MsSql:
					return new ExinEfMsSqlContext(repoConfiguration);

				default:
					throw new NotImplementedException(string.Format(Localized.ExinEfContextFactory_is_not_implemented_for_this_DbType__FORMAT__, repoConfiguration.DbType));
			}
		}
	}
}
namespace DAL.DataBase.EntityFramework.EntitiesSqlite
{
	public partial class ExinEfSqliteContext
	{
		public IRepoConfiguration LocalConfig { get; }

		public ExinEfSqliteContext(IRepoConfiguration repoConfiguration) : base(new ExinConnectionString(repoConfiguration).Get)
		{
			LocalConfig = repoConfiguration;
		}

		/// It creates a new IdentityInsert instance, which implements IDisposable, 
		/// and does nothing but in ctor sets identity insert on (for the specified 
		/// table), and in dtor (dispose) sets it off. 
		/// So you can use this method with 'using(...){...}' context
		public virtual IdentityInsertSqlite WithIdentityInsert(string tableName, bool? activate)
		{
			activate = activate ?? LocalConfig.DbInsertId ?? false;
			return new IdentityInsertSqlite(this, tableName, activate.Value);
		}
	}
}
namespace DAL.DataBase.EntityFramework.EntitiesMsSql
{
	public partial class ExinEfMsSqlContext
	{
		public IRepoConfiguration LocalConfig { get; }

		public ExinEfMsSqlContext(IRepoConfiguration repoConfiguration) : base(new ExinConnectionString(repoConfiguration).Get)
		{
			LocalConfig = repoConfiguration;
		}

		/// It creates a new IdentityInsert instance, which implements IDisposable, 
		/// and does nothing but in ctor sets identity insert on (for the specified 
		/// table), and in dtor (dispose) sets it off. 
		/// So you can use this method with 'using(...){...}' context
		public virtual IdentityInsertMsSql WithIdentityInsert(string tableName, bool? activate)
		{
			activate = activate ?? LocalConfig.DbInsertId ?? false;
			return new IdentityInsertMsSql(this, tableName, activate.Value);
		}
	}
}