using System.Collections.Generic;
using Common.Configuration;
using Common.Db.Entities;
using DAL.RepoCommon.Interfaces;
using DAL.RepoCommon.Managers.AggregateDaoManagers.Base;

namespace DAL.RepoCommon.Managers.AggregateDaoManagers
{
	public class CategoryManagerDaoAggregate : UnitOrCategoryManagerDaoAggregateBase<Category>, ICategoryManagerDao
	{
		public CategoryManagerDaoAggregate(List<IUnitOrCategoryManagerDao<Category>> managers, IRepoConfiguration repoConfiguration) : base(managers, repoConfiguration)
		{
		}
	}
}