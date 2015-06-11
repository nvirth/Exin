using System.Collections.Generic;
using Common.Configuration;
using Common.Db.Entities;
using DAL.RepoCommon.AggregateDaoManagers.Base;
using DAL.RepoCommon.Interfaces;

namespace DAL.RepoCommon.AggregateDaoManagers
{
	public class CategoryManagerDaoAggregate : UnitOrCategoryManagerDaoAggregate<Category>, ICategoryManagerDao
	{
		public CategoryManagerDaoAggregate(List<IUnitOrCategoryManagerDao<Category>> managers, IRepoConfiguration repoConfiguration) : base(managers, repoConfiguration)
		{
		}
	}
}