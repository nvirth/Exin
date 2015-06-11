using System.Collections.Generic;
using Common.Configuration;
using Common.Db.Entities;
using DAL.RepoCommon;
using DAL.RepoCommon.Interfaces;

namespace DAL.DataBase.Managers.Base
{
	public abstract class UnitManagerDbBase : RepoConfigurableBase, IUnitManagerDao, IDbManagerMarker
	{
		protected UnitManagerDbBase(IRepoConfiguration repoConfiguration) : base(repoConfiguration)
		{
		}

		public abstract List<Unit> GetAll();
		public abstract void Add(Unit item);
	}
}