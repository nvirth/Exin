using System.Collections.Generic;
using Common.Configuration;
using Common.Db.Entities;

namespace DAL.DataBase.Managers
{
	public class CategoryManagerDaoAggregate : UnitOrCategoryManagerDaoAggregate<Category>, ICategoryManagerDao
	{
		public CategoryManagerDaoAggregate(List<IUnitOrCategoryManagerDao<Category>> managers, IRepoConfiguration repoConfiguration) : base(managers, repoConfiguration)
		{
		}
	}
}