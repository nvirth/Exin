using System;
using DAL.DataBase.EntityFramework.EntitiesMsSql;
using DAL.DataBase.EntityFramework.EntitiesSqlite;
using Localization;

namespace DAL.DataBase.EntityFramework
{
	public class IdentityInsertMsSql : IDisposable
	{
		private readonly string _tableName;
		private readonly bool _activate;

		/// <summary>
		/// In ctor it activates the identity insert for a specific table. 
		/// In dtor it deactivates it. 
		/// So it can be used in a 'using(...){...}' context. 
		/// 
		/// It activates the identity insert only, if the parameter 'activate' is true 
		/// (so it can be used parametrized in methods, where identity insert is not always on) 
		/// </summary>
		public IdentityInsertMsSql(ExinEfMsSqlContext ctx, string tableName, bool activate = true)
		{
			_tableName = tableName;
			_activate = activate;

			if (_activate)
				throw new NotImplementedException(Localized.Identity_insert_using_MS_SQL_with_EntityFramework_is_not_implemented__Use_the__simple__Ado_Net_version_instead__);
		}

		public void Dispose()
		{
			if(_activate)
				throw new NotImplementedException(Localized.Identity_insert_using_MS_SQL_with_EntityFramework_is_not_implemented__Use_the__simple__Ado_Net_version_instead__);
		}
	}
	public class IdentityInsertSqlite : IDisposable
	{
		private readonly string _tableName;
		private readonly bool _activate;

		/// <summary>
		/// In ctr it activates the identity insert for a specific table. 
		/// In dtor it deactivates it. 
		/// So it can be used in a 'using(...){...}' context. 
		/// 
		/// It activates the identity insert only, if the parameter 'activate' is true 
		/// (so it can be used parametrized in methods, where identity insert is not always on) 
		/// </summary>
		public IdentityInsertSqlite(ExinEfSqliteContext ctx, string tableName, bool activate = true)
		{
			_tableName = tableName;
			_activate = activate;

			if (_activate)
				throw new NotImplementedException(Localized.Identity_insert_using_SQLite_with_EntityFramework_is_not_implemented__Use_the__simple__Ado_Net_version_instead__);
		}

		public void Dispose()
		{
			if(_activate)
				throw new NotImplementedException(Localized.Identity_insert_using_SQLite_with_EntityFramework_is_not_implemented__Use_the__simple__Ado_Net_version_instead__);
		}
	}
}
