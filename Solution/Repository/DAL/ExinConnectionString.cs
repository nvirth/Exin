using System;
using System.Configuration;
using Common;
using Common.Configuration;
using Localization;
using C = Common.Configuration.Constants.Db;

namespace DAL
{
	public interface IExinConnectionStringManager
	{
		string Get { get; }
	}

	public static class ExinConnectionString
	{
		private static IExinConnectionStringManager _core;

		static ExinConnectionString()
		{
			ReloadManager();
		}

		private static void ReloadManager()
		{
			switch (Config.DbType)
			{
				case DbType.MsSql:
					_core = new MsSqlExinConnectionStringManager();
					break;

				case DbType.SQLite:
					_core = new SQLiteExinConnectionStringManager();
					break;

				default:
					throw new NotImplementedException(string.Format(Localized.ExinConnectionString_s_ctor_is_not_implemented_for__0_, Config.DbType));
			}
		}

		#region Delegating members

		public static string Get => _core.Get;

	    #endregion
	}

	public abstract class ExinConnectionStringManagerBase
	{
		protected abstract string _adoNetConnStrName { get; }
		protected abstract string _efConnStrName { get; }
		protected string _connStrName;
		protected string _connStr;

		protected ExinConnectionStringManagerBase()
		{
			switch(Config.DbAccessMode)
			{
				case DbAccessMode.AdoNet:
					_connStrName = _adoNetConnStrName;
					break;
				case DbAccessMode.EntityFramework:
					_connStrName = _efConnStrName;
					break;
				default:
					throw new NotImplementedException(string.Format(Localized.ExinConnectionStringManagerBase_s_ctor_is_not_implemented_for__0_, Config.DbType));
			}
		}
	}

	/// <summary>
	/// In Release and Debug mode, the connection string is set up by config transform.
	/// By testing, the UnitTests project's config file contains the right value (no need to config transform)
	/// </summary>
	public class MsSqlExinConnectionStringManager : ExinConnectionStringManagerBase, IExinConnectionStringManager
	{
		public string Get
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
	public class SQLiteExinConnectionStringManager : ExinConnectionStringManagerBase, IExinConnectionStringManager
	{
		public string Get
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
