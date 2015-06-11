using Common.Configuration;
using Common.Db;
using Common.Db.Entities;
using DAL.DataBase.Managers.Factory;
using DAL.RepoCommon.Interfaces;
using DAL.RepoCommon.Managers.Base;
using Localization;
using C = Common.Configuration.Constants.Resources.DefaultUnits;

namespace DAL.RepoCommon.Managers
{
	public class UnitManager: CachedManagerBase<Unit>, IUnitManager
	{
		public static readonly IUnitManager Instance = new UnitManager();
		private readonly IUnitManagerDao _core;

		protected override int ValidFrom => Config.UnitValidFrom;
		protected override string LocalizedTypeName => Localized.Unit;
		protected override string LocalizedTypeNameLowercase => Localized.unit_lowercase;

		public UnitManager(IRepoConfiguration repoConfiguration = null) : base(repoConfiguration)
		{
			_core = new ManagerDaoFactory(repoConfiguration).UnitManager;
		}

		// -- IUnitManager implementation

		public Unit GetDefaultUnit => GetUnitPc;
		public Unit GetUnitPc => GetByName(C.Db);
	    public Unit GetUnitNone => GetByName(C.None);

		public override void RefreshCache()
		{
			var units = _core.GetAll();
			RefreshCache(units);
		}

		public override void Add(Unit unit)
		{
			CheckExistsInCache(unit);
			_core.Add(unit);
			AddToCache(unit);
		}

		// --

		static UnitManager()
		{
			// For safety sake
			StaticInitializer.InitAllStatic();
		}

		public static void InitManagerRelief()
		{
			ManagersRelief.UnitManager.InitDefaultUnit(() => Instance.GetDefaultUnit);
			ManagersRelief.UnitManager.InitGetByName(Instance.GetByName);
		}
	}
}
