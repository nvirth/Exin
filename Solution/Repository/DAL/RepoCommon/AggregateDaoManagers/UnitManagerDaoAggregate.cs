using System.Collections.Generic;
using Common.Configuration;
using Common.Db.Entities;
using DAL.RepoCommon.AggregateDaoManagers.Base;
using DAL.RepoCommon.Interfaces;

namespace DAL.RepoCommon.AggregateDaoManagers
{
	public class UnitManagerDaoAggregate : UnitOrCategoryManagerDaoAggregate<Unit>, IUnitManagerDao
	{
		public UnitManagerDaoAggregate(List<IUnitOrCategoryManagerDao<Unit>> managers, IRepoConfiguration repoConfiguration) : base(managers, repoConfiguration)
		{
		}
	}
}