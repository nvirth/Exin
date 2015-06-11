using System.Collections.Generic;
using Common.Configuration;
using Common.Utils.Helpers;
using DAL.RepoCommon.Interfaces;

namespace DAL.RepoCommon.Managers.AggregateDaoManagers.Base
{
	public abstract class UnitOrCategoryManagerDaoAggregateBase<T>
		: AggregateManagerBase<IUnitOrCategoryManagerDao<T>>, IUnitOrCategoryManagerDao<T>
	{
		protected UnitOrCategoryManagerDaoAggregateBase(List<IUnitOrCategoryManagerDao<T>> managers,
			IRepoConfiguration repoConfiguration) : base(managers, repoConfiguration)
		{
		}

		public List<T> GetAll()
		{
			return ManagerForRead.GetAll();
		}

		public void Add(T item)
		{
			ManagersForWrite.ForEach(dao => dao.Add(item));
		}
	}
}