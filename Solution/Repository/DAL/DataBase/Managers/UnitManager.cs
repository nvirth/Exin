using System;
using System.Collections.Generic;
using Common;
using Common.Configuration;
using Common.Db;
using Common.Db.Entities;
using Common.Log;
using DAL.DataBase.Managers.Factory;
using DAL.FileRepo;
using Localization;
using Config = Common.Configuration.Config;
using C = Common.Configuration.Constants.Resources.DefaultUnits;

namespace DAL.DataBase.Managers
{
	public interface IUnitManager
	{
		void RefreshCache();
		void ClearCache();

		List<Unit> GetAll();
		List<Unit> GetAllValid();
		Unit Get(int ID);
		Unit GetByName(string name, bool nullIfNotFound = false);

		void Add(Unit unit);
	}

	public abstract class UnitManagerCommonBase : DbConfigurableBase, IUnitManager
	{
		protected UnitManagerCommonBase(IRepoConfiguration repoConfiguration) : base(repoConfiguration)
		{
			RefreshCache();
		}

		#region Cache

		private readonly List<Unit> _cacheFull = new List<Unit>();
		private readonly List<Unit> _cacheValid = new List<Unit>();

		public void ClearCache()
		{
			_cacheValid.Clear();
			_cacheFull.Clear();
		}

		public void RefreshCache()
		{
			switch(Config.Repo.ReadMode)
			{
				case ReadMode.FromFile:
					RefreshCache_FromFile();
					break;
				case ReadMode.FromDb:
					RefreshCache_FromDb();
					break;
				default:
					var msg = Localized.UnitManagerCommonBase_RefreshCache_is_not_implemented_for_ReadMode__ + Config.Repo.ReadMode;
					ExinLog.ger.LogError(msg);
					throw new NotImplementedException(msg);
			}
		}

		private void RefreshCache_FromFile()
		{
			var units = FileRepoManager.Instance.GetUnits();
			RefreshCache_Refresh(units);
		}

		protected void RefreshCache_Refresh(IEnumerable<Unit> units)
		{
			ClearCache();

			foreach(var u in units)
			{
				_cacheFull.Add(u);
				if(u.ID >= Config.UnitValidFrom)
					_cacheValid.Add(u);
			}
		}

		protected abstract void RefreshCache_FromDb();

		#endregion

		#region READ

		public List<Unit> GetAll()
		{
			if(_cacheFull != null && _cacheFull.Count != 0)
				return _cacheFull;

			RefreshCache();
			return _cacheFull;
		}

		public List<Unit> GetAllValid()
		{
			if(_cacheValid != null && _cacheValid.Count != 0)
				return _cacheValid;

			RefreshCache();
			return _cacheValid;
		}

		public Unit Get(int ID)
		{
			var unit = _cacheFull.Find(c => c.ID == ID);
			if(unit == null)
			{
				var msg = string.Format(Localized.Could_not_find_the_Unit__ID_0__in_the_database__FORMAT__, ID);
				var e = new Exception(msg);
				ExinLog.ger.LogException(msg, e);
				throw e;
			}

			return unit;
		}

		public Unit GetByName(string name, bool nullIfNotFound = false)
		{
			var unit = _cacheFull.Find(c => c.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
			if(unit == null && !nullIfNotFound)
			{
				var msg = string.Format(Localized.Could_not_find_the_Unit__Name_0__in_the_database__FORMAT__, name);
				var e = new Exception(msg);
				ExinLog.ger.LogException(msg, e);
				throw e;
			}

			return unit;
		}

		#endregion

		#region CREATE

		public abstract void Add(Unit unit);

		protected void CheckExistsInCache(Unit unit)
		{
			var existingWithSameName = GetByName(unit.Name, nullIfNotFound: true);
			if(existingWithSameName != null)
			{
				string msg = Localized.Unit_already_exists_with_the_specified_name;
				var e = new Exception(msg);
				ExinLog.ger.LogException(msg, e);
				throw e;
			}
		}

		protected void AddToCache(Unit unit)
		{
			_cacheFull.Add(unit);
			if(unit.ID >= Config.UnitValidFrom)
				_cacheValid.Add(unit);
		}

		#endregion
	}

	public static class UnitManager
	{
		public static readonly IUnitManager Instance = new ManagerFactory().UnitManager;

		public static Unit GetDefaultUnit => GetUnitPc;
		public static Unit GetUnitPc => Instance.GetByName(C.Db);
	    public static Unit GetUnitNone => Instance.GetByName(C.None);

		static UnitManager()
		{
			ManagersRelief.UnitManager.InitDefaultUnit(() => GetDefaultUnit);
			ManagersRelief.UnitManager.InitGetByName(Instance.GetByName);
		}
	}
}
