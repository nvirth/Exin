using System.Collections.Generic;
using Common.Configuration;
using Common.Db.Entities;

namespace DAL.DataBase.Managers
{
	public class UnitManagerDaoAggregate : UnitOrCategoryManagerDaoAggregate<Unit>, IUnitManagerDao
	{
		public UnitManagerDaoAggregate(List<IUnitOrCategoryManagerDao<Unit>> managers, IRepoConfiguration repoConfiguration) : base(managers, repoConfiguration)
		{
		}
	}
}