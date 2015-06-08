using System;
using System.Configuration;
using Common;
using Common.Configuration;
using DAL.DataBase;
using Localization;
using C = Common.Configuration.Constants.Db;

namespace DAL
{
	public class ExinConnectionString : DbConfigurableBase
	{
		private ExinConnectionStringManagerBase _core;

		public ExinConnectionString(DbType dbType, DbAccessMode dbAccessMode) : base(dbType, dbAccessMode)
		{
			Init();
		}

		private void Init()
		{
			switch(DbType)
			{
				case DbType.MsSql:
					_core = new MsSqlExinConnectionStringManager(DbType, DbAccessMode);
					break;

				case DbType.SQLite:
					_core = new SQLiteExinConnectionStringManager(DbType, DbAccessMode);
					break;

				default:
					throw new NotImplementedException(string.Format(Localized.ExinConnectionString_s_ctor_is_not_implemented_for__0_, DbType));
			}
		}

		#region Delegating members

		public string Get => _core.Get;

	    #endregion
	}

	public abstract class ExinConnectionStringManagerBase : DbConfigurableBase
	{
		protected abstract string _adoNetConnStrName { get; }
		protected abstract string _efConnStrName { get; }
		protected string _connStrName;
		protected string _connStr;

		protected ExinConnectionStringManagerBase(DbType dbType, DbAccessMode dbAccessMode) : base(dbType, dbAccessMode)
		{
			switch(DbAccessMode)
			{
				case DbAccessMode.AdoNet:
					_connStrName = _adoNetConnStrName;
					break;
				case DbAccessMode.EntityFramework:
					_connStrName = _efConnStrName;
					break;
				default:
					throw new NotImplementedException(string.Format(Localized.ExinConnectionStringManagerBase_s_ctor_is_not_implemented_for__0_, DbAccessMode));
			}
		}

		public abstract string Get { get; }
    }

	/// <summary>
	/// In Release and Debug mode, the connection string is set up by config transform.
	/// By testing, the UnitTests project's config file contains the right value (no need to config transform)
	/// </summary>
	public class MsSqlExinConnectionStringManager : ExinConnectionStringManagerBase
	{
		public MsSqlExinConnectionStringManager(DbType dbType, DbAccessMode dbAccessMode) : base(dbType, dbAccessMode)
		{
		}

		public override string Get
		{
			get
			{
				if(string.IsNullOrEmpty(_connStr))
					_connStr = ConfigurationManager.ConnectionStrings[_connStrName].ConnectionString;

				return _connStr;

			}
		}

		protected override string _adoNetConnStrName => C.MsSql_AdoNet_ConnStr;
	    protected override string _efConnStrName => C.MsSql_EF_ConnStr;
	}

	/// <summary>
	/// Because SQLite dbs are single files, we have to point to that file in the connection string. But we've got
	/// 3 modes: release/debug/test; each one have an own file. We have to cut together these files' locations.
	/// In the config files' ConnectionStrings section, we put a placeholder into our connection strings. We have to
	/// replace these to the corresponding ones; but this is easy here, because the RootDir app setting
	/// comes from config transform in tha case of Release and Debug modes...
	/// </summary>
	public class SQLiteExinConnectionStringManager : ExinConnectionStringManagerBase
	{
		public SQLiteExinConnectionStringManager(DbType dbType, DbAccessMode dbAccessMode) : base(dbType, dbAccessMode)
		{
		}

		public override string Get
		{
			get
			{
				if(string.IsNullOrEmpty(_connStr))
				{
					_connStr = ConfigurationManager.ConnectionStrings[_connStrName].ConnectionString;
					_connStr = _connStr.Replace(C.SqliteDbFullpathPlaceholder, RepoPaths.SqliteDbFile);
					_connStr = _connStr.Replace("\\\\", "\\");
				}

				return _connStr;
			}
		}

		protected override string _adoNetConnStrName => C.SQLite_AdoNet_ConnStr;
	    protected override string _efConnStrName => C.SQLite_EF_ConnStr;
	}

}
