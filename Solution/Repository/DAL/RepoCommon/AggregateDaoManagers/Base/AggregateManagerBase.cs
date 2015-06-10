using System;
using System.Collections.Generic;
using System.Linq;
using Common.Configuration;
using Common.Log;

namespace DAL.DataBase.Managers
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
				const string msg = "AggregateManagerBase.ctor: Argument 'managers' can't be null or empty. ";
				throw ExinLog.ger.LogException(msg, new ArgumentException(msg, "managers"));
			}
		}
	}
}