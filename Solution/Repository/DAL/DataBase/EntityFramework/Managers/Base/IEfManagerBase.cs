using System.Data.Entity;

namespace DAL.DataBase.EntityFramework.Managers.Base
{
	internal interface IEfManagerBase
	{
		void CheckDbContext(DbContext dbContext);
	}
}
