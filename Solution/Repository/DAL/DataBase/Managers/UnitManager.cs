using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Common;
using Common.Log;
using DAL.DataBase.AdoNet;
using DAL.DataBase.Managers.Factory;
using DAL.FileRepo;
using Localization;
using Config = Common.Config.Config;
using Unit = Common.DbEntities.Unit;

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
		Unit GetByDisplayName(string displayName, bool nullIfNotFound = false);

		void Add(Unit unit);
	}

	public abstract class UnitManagerCommonBase : IUnitManager
	{
		protected UnitManagerCommonBase()
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
			switch(Config.ReadMode)
			{
				case ReadMode.FromFile:
					RefreshCache_FromFile();
					break;
				case ReadMode.FromDb:
					RefreshCache_FromDb();
					break;
				default:
					var msg = Localized.UnitManagerCommonBase_RefreshCache_is_not_implemented_for_ReadMode__ + Config.ReadMode;
					ExinLog.ger.LogError(msg);
					throw new NotImplementedException(msg);
			}
		}

		private void RefreshCache_FromFile()
		{
			var units = FileRepoManager.GetUnits();
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

		public Unit GetByDisplayName(string displayName, bool nullIfNotFound = false)
		{
			var unit = _cacheFull.Find(c => c.DisplayName.Equals(displayName, StringComparison.InvariantCultureIgnoreCase));
			if(unit == null && !nullIfNotFound)
			{
				var msg = string.Format(Localized.Could_not_find_the_Unit__DisplayName_0__in_the_database__FORMAT__, displayName);
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
			var existingWithSameName =
				GetByDisplayName(unit.DisplayName, nullIfNotFound: true)
				?? GetByDisplayName(unit.Name, nullIfNotFound: true)
				?? GetByName(unit.DisplayName, nullIfNotFound: true)
				?? GetByName(unit.Name, nullIfNotFound: true);
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
		private static readonly IUnitManager Manager = ManagerFactory.IUnitManager;
		public static Unit GetUnitDb { get { return Manager.GetByName("Db"); } }
		public static Unit GetUnitNone { get { return Manager.Get(0); } }

		public static void ClearCache()
		{
			Manager.ClearCache();
		}

		public static void RefreshCache()
		{
			Manager.RefreshCache();
		}

		public static List<Unit> GetAll()
		{
			return Manager.GetAll();
		}

		public static List<Unit> GetAllValid()
		{
			return Manager.GetAllValid();
		}

		public static Unit Get(int ID)
		{
			return Manager.Get(ID);
		}

		public static Unit GetByName(string name, bool nullIfNotFound = false)
		{
			return Manager.GetByName(name, nullIfNotFound);
		}

		public static Unit GetByDisplayName(string displayName, bool nullIfNotFound = false)
		{
			return Manager.GetByDisplayName(displayName, nullIfNotFound);
		}

		public static void Add(Unit unit)
		{
			Manager.Add(unit);
		}
	}
}
