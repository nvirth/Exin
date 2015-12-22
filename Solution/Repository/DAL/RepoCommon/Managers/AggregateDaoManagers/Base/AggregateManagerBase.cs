using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.Configuration;
using Exin.Common.Logging;
using Exin.Common.Logging.Core;
using DAL.RepoCommon.Interfaces;
using Localization;

namespace DAL.RepoCommon.Managers.AggregateDaoManagers.Base
{
	public abstract class AggregateManagerBase<T> : RepoConfigurableBase
	{
		private readonly List<T> _managers;

		protected IEnumerable<T> AllManagers => _managers;

		protected T FirstDbManager => _managers.First(manager => manager is IDbManagerMarker);
		protected IEnumerable<T> AllDbManagers => _managers.Where(manager => manager is IDbManagerMarker);

		protected T FirstFileRepoManager => _managers.First(manager => manager is IFileRepoManagerMarker);
		protected IEnumerable<T> AllFileRepoManagers => _managers.Where(manager => manager is IFileRepoManagerMarker);

		public AggregateManagerBase(List<T> managers, IRepoConfiguration repoConfiguration) : base(repoConfiguration)
		{
			_managers = managers;

			if(managers == null || managers.Count == 0)
			{
				throw Log.Fatal(this,
					m => m(Localized.ResourceManager, LocalizedKeys.AggregateManagerBase_ctor__Argument__managers__can_t_be_null_or_empty__),
					LogTarget.All,
					new ArgumentException(Localized.AggregateManagerBase_ctor__Argument__managers__can_t_be_null_or_empty__, "managers")
				);
			}
		}

		// --

		protected T ManagerForRead
		{
			get
			{
				switch(LocalConfig.ReadMode)
				{
					case ReadMode.FromFile:
						return FirstFileRepoManager;
					case ReadMode.FromDb:
						return FirstDbManager;
					default:
						throw new NotImplementedException("Aggregate managers are not implemented for ReadMode: " + LocalConfig.ReadMode);
				}
			}
		}

		protected IEnumerable<T> ManagersForWrite
		{
			get
			{
				switch(LocalConfig.SaveMode)
				{
					case SaveMode.OnlyToFile:
						return AllFileRepoManagers;
					case SaveMode.OnlyToDb:
						return AllDbManagers;
					case SaveMode.FileAndDb:
						return AllManagers;
					default:
						throw new NotImplementedException("Aggregate managers are not implemented for SaveMode: " + LocalConfig.SaveMode);
				}
			}
		}
	}
}