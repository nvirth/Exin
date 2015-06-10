using System.Collections.Generic;
using Common.Configuration;
using Common.Db.Entities;

namespace DAL.DataBase.Managers
{
	public abstract class CategoryManagerDbBase : RepoConfigurableBase, ICategoryManagerDao
	{
		protected CategoryManagerDbBase(IRepoConfiguration repoConfiguration) : base(repoConfiguration)
		{
		}

		public abstract List<Category> GetAll();
		public abstract void Add(Category item);
	}
}