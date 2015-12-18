using System;
using System.Collections.Generic;
using System.Configuration;
using Common;
using Common.Configuration;
using Exin.Common.Logging;
using Exin.Common.Logging.Core;
using Common.Utils.Helpers;
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
		private static Dictionary<string, string> _availableConnectionStrings;
		public static Dictionary<string, string> AvailableConnectionStrings
		{
			get
			{
				if(_availableConnectionStrings == null)
				{
					const string msSql_EF_ConnStr_Format = "metadata=res://*/DataBase.EntityFramework.EntitiesMsSql.ExinEfMsSql.csdl|res://*/DataBase.EntityFramework.EntitiesMsSql.ExinEfMsSql.ssdl|res://*/DataBase.EntityFramework.EntitiesMsSql.ExinEfMsSql.msl;provider=System.Data.SqlClient;provider connection string=\"{0}\"";
					var msSqlConnStrs = Config.Repo.Settings.MsSqlSettings.ConnectionStrings;

					_availableConnectionStrings = new Dictionary<string, string>(4) {
						{C.SQLite_AdoNet_ConnStr, "data source=#SQLITE_REPO_FULLPATH#"},
						{C.SQLite_EF_ConnStr, "metadata=res://*/DataBase.EntityFramework.EntitiesSqlite.ExinEf.csdl|res://*/DataBase.EntityFramework.EntitiesSqlite.ExinEf.ssdl|res://*/DataBase.EntityFramework.EntitiesSqlite.ExinEf.msl;provider=System.Data.SQLite;provider connection string=&quot;data source=#SQLITE_REPO_FULLPATH#&quot;"},
						{C.MsSql_AdoNet_ConnStr, msSqlConnStrs.AdoNet},
						{C.MsSql_EF_ConnStr, msSql_EF_ConnStr_Format.Formatted(msSqlConnStrs.EntityFramework)}
					};
				}
				return _availableConnectionStrings;
			}
		}

		protected abstract string adoNetConnStrName { get; }
		protected abstract string efConnStrName { get; }
		protected string connStrName;
		protected string connStr;

		protected ExinConnectionStringManagerBase(IRepoConfiguration repoConfiguration) : base(repoConfiguration)
		{
			switch(LocalConfig.DbAccessMode)
			{
				case DbAccessMode.AdoNet:
					connStrName = adoNetConnStrName;
					break;
				case DbAccessMode.EntityFramework:
					connStrName = efConnStrName;
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
						if(string.IsNullOrEmpty(_connStrCache))
						{
							_connStrCache = AvailableConnectionStrings[connStrName];
							Validate();
						}

				connStr = _connStrCache;
				return connStr;

			}
		}

		private void Validate()
		{
			var valid = connStrName == adoNetConnStrName
				? Config.Repo.Settings.MsSqlSettings.ConnectionStrings.ValidateAdoNet()
				: Config.Repo.Settings.MsSqlSettings.ConnectionStrings.ValidateEntityFramework();

			if(valid)
				return;

			const string msg = "Could not find the MS SQL connection string. ";
			throw ExinLog.ger.LogException(msg, new ConfigurationErrorsException(msg));
		}

		protected override string adoNetConnStrName => C.MsSql_AdoNet_ConnStr;
	    protected override string efConnStrName => C.MsSql_EF_ConnStr;
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
							_connStrCache = AvailableConnectionStrings[connStrName];
							_connStrCache = _connStrCache.Replace(C.SqliteDbFullpathPlaceholder, Config.Repo.Paths.SqliteDbFile);
							_connStrCache = _connStrCache.Replace("\\\\", "\\");
						}

				connStr = _connStrCache;
				return connStr;
			}
		}

		protected override string adoNetConnStrName => C.SQLite_AdoNet_ConnStr;
	    protected override string efConnStrName => C.SQLite_EF_ConnStr;
	}

}
