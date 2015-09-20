using System;
using System.Configuration;
using Common;
using Common.Configuration;
using DAL.RepoCommon;
using Localization;
using C = Common.Configuration.Constants.Db;

namespace DAL.DataBase
{
	public class ExinConnectionString : RepoConfigurableBase
	{
		private ExinConnectionStringManagerBase _core;
		
		public ExinConnectionString(IRepoConfiguration repoConfiguration) : base(repoConfiguration)
		{
			Init();
		}

		private void Init()
		{
			switch(LocalConfig.DbType)
			{
				case DbType.MsSql:
					_core = new MsSqlExinConnectionStringManager(LocalConfig);
					break;

				case DbType.SQLite:
					_core = new SQLiteExinConnectionStringManager(LocalConfig);
					break;

				default:
					throw new NotImplementedException(string.Format(Localized.ExinConnectionString_s_ctor_is_not_implemented_for__0_, LocalConfig.DbType));
			}
		}

		#region Delegating members

		public string Get => _core.Get;

	    #endregion
	}

	public abstract class ExinConnectionStringManagerBase : RepoConfigurableBase
	{
		protected abstract string _adoNetConnStrName { get; }
		protected abstract string _efConnStrName { get; }
		protected string _connStrName;
		protected string _connStr;

		protected ExinConnectionStringManagerBase(IRepoConfiguration repoConfiguration) : base(repoConfiguration)
		{
			switch(LocalConfig.DbAccessMode)
			{
				case DbAccessMode.AdoNet:
					_connStrName = _adoNetConnStrName;
					break;
				case DbAccessMode.EntityFramework:
					_connStrName = _efConnStrName;
					break;
				default:
					throw new NotImplementedException(string.Format(Localized.ExinConnectionStringManagerBase_s_ctor_is_not_implemented_for__0_, LocalConfig.DbAccessMode));
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
		private static string _connStrCache;
		private static readonly object _lock = new object();

		public MsSqlExinConnectionStringManager(IRepoConfiguration repoConfiguration) : base(repoConfiguration)
		{
		}

		public override string Get
		{
			get
			{
				if(string.IsNullOrEmpty(_connStrCache))
					lock (_lock)
						if (string.IsNullOrEmpty(_connStrCache))
							_connStrCache = ConfigurationManager.ConnectionStrings[_connStrName].ConnectionString;

				_connStr = _connStrCache;
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
		private static string _connStrCache;
		private static readonly object _lock = new object();

		public SQLiteExinConnectionStringManager(IRepoConfiguration repoConfiguration) : base(repoConfiguration)
		{
		}

		public override string Get
		{
			get
			{
				if(string.IsNullOrEmpty(_connStrCache))
					lock (_lock)
						if (string.IsNullOrEmpty(_connStrCache))
						{
							_connStrCache = ConfigurationManager.ConnectionStrings[_connStrName].ConnectionString;
							_connStrCache = _connStrCache.Replace(C.SqliteDbFullpathPlaceholder, Config.Repo.Paths.SqliteDbFile);
							_connStrCache = _connStrCache.Replace("\\\\", "\\");
						}

				_connStr = _connStrCache;
				return _connStr;
			}
		}

		protected override string _adoNetConnStrName => C.SQLite_AdoNet_ConnStr;
	    protected override string _efConnStrName => C.SQLite_EF_ConnStr;
	}

}
