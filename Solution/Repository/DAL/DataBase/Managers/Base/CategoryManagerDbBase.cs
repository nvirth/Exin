using System.Collections.Generic;
using Common.Configuration;
using Common.Db.Entities;
using DAL.RepoCommon;
using DAL.RepoCommon.Interfaces;

namespace DAL.DataBase.Managers.Base
{
	public abstract class CategoryManagerDbBase : RepoConfigurableBase, ICategoryManagerDao, IDbManagerMarker
	{
		protected CategoryManagerDbBase(IRepoConfiguration repoConfiguration) : base(repoConfiguration)
		{
		}

		public abstract List<Category> GetAll();
		public abstract void Add(Category item);
	}
}