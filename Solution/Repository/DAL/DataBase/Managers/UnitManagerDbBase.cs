using System.Collections.Generic;
using Common.Configuration;
using Common.Db.Entities;

namespace DAL.DataBase.Managers
{
	public abstract class UnitManagerDbBase : RepoConfigurableBase, IUnitManagerDao
	{
		protected UnitManagerDbBase(IRepoConfiguration repoConfiguration) : base(repoConfiguration)
		{
		}

		public abstract List<Unit> GetAll();
		public abstract void Add(Unit item);
	}
}