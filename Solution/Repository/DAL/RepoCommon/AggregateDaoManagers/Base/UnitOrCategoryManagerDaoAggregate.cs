using System;
using System.Collections.Generic;
using Common;
using Common.Configuration;
using Common.Utils.Helpers;
using DAL.RepoCommon.Interfaces;

namespace DAL.RepoCommon.AggregateDaoManagers.Base
{
	public class UnitOrCategoryManagerDaoAggregate<T> : AggregateManagerBase<IUnitOrCategoryManagerDao<T>>, IUnitOrCategoryManagerDao<T>
	{
		public UnitOrCategoryManagerDaoAggregate(List<IUnitOrCategoryManagerDao<T>> managers, IRepoConfiguration repoConfiguration) : base(managers, repoConfiguration)
		{
		}

		public List<T> GetAll()
		{
			switch(LocalConfig.ReadMode)
			{
				case ReadMode.FromFile:
					return FirstFileRepoManager.GetAll();
				case ReadMode.FromDb:
					return FirstDbManager.GetAll();
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public void Add(T item)
		{
			switch(LocalConfig.SaveMode)
			{
				case SaveMode.OnlyToFile:
					AllFileRepoManagers.ForEach(dao => dao.Add(item));
					break;
				case SaveMode.FileAndDb:
					AllManagers.ForEach(dao => dao.Add(item));
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}