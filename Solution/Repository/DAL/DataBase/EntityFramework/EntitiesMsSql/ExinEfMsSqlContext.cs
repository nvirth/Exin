using System.Data.Entity;
using DAL.DataBase.EntityFramework.EntitiesSqlite;

namespace DAL.DataBase.EntityFramework.EntitiesMsSql
{
	public partial class ExinEfMsSqlContext
	{
		public ExinEfMsSqlContext(string connStrOrName) : base(connStrOrName) { }
	}
}
