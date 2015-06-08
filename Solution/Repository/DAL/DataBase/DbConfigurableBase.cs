using Common;

namespace DAL.DataBase
{
	public class DbConfigurableBase
	{
		public DbType DbType { get; }
		public DbAccessMode DbAccessMode { get; }

		public DbConfigurableBase(DbType dbType, DbAccessMode dbAccessMode)
		{
			DbType = dbType;
			DbAccessMode = dbAccessMode;
		}
	}
}
