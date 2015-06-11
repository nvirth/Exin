using System.Collections.Generic;
using Common.Configuration;
using Common.Db.Entities;
using DAL.RepoCommon.Interfaces;
using DAL.RepoCommon.Managers.AggregateDaoManagers.Base;

namespace DAL.RepoCommon.Managers.AggregateDaoManagers
{
	public class UnitManagerDaoAggregate : UnitOrCategoryManagerDaoAggregateBase<Unit>, IUnitManagerDao
	{
		public UnitManagerDaoAggregate(List<IUnitOrCategoryManagerDao<Unit>> managers, IRepoConfiguration repoConfiguration) : base(managers, repoConfiguration)
		{
		}
	}
}