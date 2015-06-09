using Common;
using Common.Configuration;

namespace DAL.DataBase
{
	public class DbConfigurableBase
	{
		public DbType DbType { get; }
		public DbAccessMode DbAccessMode { get; }

		public DbConfigurableBase(DbType dbType, DbAccessMode dbAccessMode)
		{
			DbType = dbType == 0 ? Config.DbType : dbType;
			DbAccessMode = dbAccessMode == 0 ? Config.DbAccessMode : dbAccessMode;
		}
	}
}
