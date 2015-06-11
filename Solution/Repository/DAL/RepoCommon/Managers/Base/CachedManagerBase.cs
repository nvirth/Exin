using System;
using System.Collections.Generic;
using Common.Configuration;
using Common.Db.Entities.Base;
using Common.Log;
using Common.Utils.Helpers;
using DAL.RepoCommon.Interfaces;
using Localization;

namespace DAL.RepoCommon.Managers.Base
{
	public abstract class CachedManagerBase<T> : RepoConfigurableBase, ICachedManager<T>
		where T : LocalizedEntityBase
	{
		private readonly List<T> _cacheFull = new List<T>();
		private readonly List<T> _cacheValid = new List<T>();

		protected CachedManagerBase(IRepoConfiguration repoConfiguration) : base(repoConfiguration)
		{
		}

		public abstract void RefreshCache();
		public abstract void Add(T item);
		protected abstract int ValidFrom { get; }
		protected abstract string LocalizedTypeName { get; }
		protected abstract string LocalizedTypeNameLowercase { get; }

		public void ClearCache()
		{
			_cacheValid.Clear();
			_cacheFull.Clear();
		}

		protected void RefreshCache(IEnumerable<T> items)
		{
			ClearCache();

			foreach(var u in items)
			{
				_cacheFull.Add(u);
				if(u.ID >= ValidFrom)
					_cacheValid.Add(u);
			}
		}

		#region READ

		public List<T> GetAll()
		{
			CheckCache();
			return _cacheFull;
		}

		public List<T> GetAllValid()
		{
			CheckCache();
			return _cacheValid;
		}

		public T Get(int ID)
		{
			CheckCache();

			var item = _cacheFull.Find(c => c.ID == ID);
			if(item == null)
			{
                var msg = string.Format(Localized.Could_not_find_the__0___ID___1___in_the_database, LocalizedTypeNameLowercase, ID);
				throw ExinLog.ger.LogException(msg, new Exception(msg));
			}

			return item;
		}

		public T GetByName(string name, bool nullIfNotFound = false)
		{
			CheckCache();

			var item = _cacheFull.Find(c => c.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
			if(item == null && !nullIfNotFound)
			{
                var msg = string.Format(Localized.Could_not_find_the__0___Name___1___in_the_database, LocalizedTypeNameLowercase, name);
				throw ExinLog.ger.LogException(msg, new Exception(msg));
			}

			return item;
		}

		#endregion

		private void CheckCache()
		{
			//if(_cacheFull != null && _cacheFull.Count != 0)
			if(_cacheValid != null && _cacheValid.Count != 0)
				return;

			RefreshCache();
		}

		protected void CheckExistsInCache(T item)
		{
			var existingWithSameName = GetByName(item.Name, nullIfNotFound: true);
			if(existingWithSameName != null)
			{
                string msg = Localized._0__already_exists_with_the_specified_name__.Formatted(LocalizedTypeName);
				throw ExinLog.ger.LogException(msg, new Exception(msg));
			}
		}

		protected void AddToCache(T item)
		{
			_cacheFull.Add(item);
			if(item.ID >= ValidFrom)
				_cacheValid.Add(item);
		}
	}
}
