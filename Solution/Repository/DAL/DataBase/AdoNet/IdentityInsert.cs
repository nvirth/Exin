using System;

namespace DAL.DataBase.AdoNet
{
	public class IdentityInsert : IDisposable
	{
		private readonly ExinAdoNetContextBase _ctx;
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
		public IdentityInsert(ExinAdoNetContextBase ctx, string tableName, bool activate = true)
		{
			_ctx = ctx;
			_tableName = tableName;
			_activate = activate;

			if(_activate)
				_ctx.SetIdentityInsertOn(_tableName);
		}

		public void Dispose()
		{
			if(_activate)
				_ctx.SetIdentityInsertOff(_tableName);
		}
	}
}
