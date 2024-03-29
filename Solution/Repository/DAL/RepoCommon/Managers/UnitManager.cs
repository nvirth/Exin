﻿using System.Linq;
using Common.Configuration;
using Common.Db;
using Common.Db.Entities;
using DAL.RepoCommon.Interfaces;
using DAL.RepoCommon.Managers.Base;
using DAL.RepoCommon.Managers.Factory;
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
		protected override string LocalizedTypeNameLowercase => Localized.Unit_lowercase;

		public UnitManager(IRepoConfiguration repoConfiguration = null) : base(repoConfiguration)
		{
			_core = new ManagerDaoFactory(LocalConfig).UnitManager;
		}

		// -- IUnitManager implementation

		public Unit GetDefaultUnit => GetUnitPc;
		public Unit GetUnitPc => GetByName(C.Pc);
	    public Unit GetUnitNone => GetByName(C.None);

		public override void RefreshCache()
		{
			var units = _core.GetAll();
			units = units.OrderBy(unit => unit.DisplayName).ToList();
			RefreshCache(units);
		}

		public override void Add(Unit unit)
		{
			CheckExistsInCache(unit);
			_core.Add(unit);
			AddToCache(unit);
		}
	}
}
