﻿using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.Configuration;
using Common.Db.Entities;
using Common.Utils.Helpers;
using Exin.Common.Logging;
using Exin.Common.Logging.Core;
using DAL.DataBase.AdoNet.Managers;
using DAL.DataBase.EntityFramework.Managers;
using DAL.FileRepo.Managers;
using DAL.RepoCommon.Interfaces;
using DAL.RepoCommon.Managers.AggregateDaoManagers;
using Localization;
using CategoryManagerClass = DAL.RepoCommon.Managers.CategoryManager;
using UnitManagerClass = DAL.RepoCommon.Managers.UnitManager;
using TransactionItemManagerClass = DAL.RepoCommon.Managers.TransactionItemManager;

namespace DAL.RepoCommon.Managers.Factory
{
	public class ManagerDaoFactory : RepoConfigurableBase
	{
		#region Ctor

		public ManagerDaoFactory(IRepoConfiguration repoConfiguration = null) : base(InitConfiguration(repoConfiguration))
		{
			switch (LocalConfig.DbAccessMode)
			{
				case DbAccessMode.AdoNet:
				case DbAccessMode.EntityFramework:
					// Config is ok, implemented
					break;

				default:
					Log.Fatal(this, m => m(Localized.ResourceManager, LocalizedKeys.ManagerFactory_is_not_implemented_for___0_, LocalConfig.DbAccessMode));
					throw new NotImplementedException(Localized.ManagerFactory_is_not_implemented_for___0_.Formatted(LocalConfig.DbAccessMode));
			}
		}

		private static IRepoConfiguration InitConfiguration(IRepoConfiguration repoConfiguration)
		{
			return repoConfiguration ?? new RepoConfiguration();
		}

		#endregion

		#region Managers

		private ICategoryManagerDao _categoryManager;
		public ICategoryManagerDao CategoryManager
		{
			get
			{
				if (_categoryManager == null)
				{
					InitManager<ICategoryManagerDao>(
						() => new CategoryManagerFileRepo(),
						() => CategoryManagerAdoNetFactory.Create(LocalConfig),
						() => CategoryManagerEfFactory.Create(LocalConfig),
						managers => _categoryManager = new CategoryManagerDaoAggregate(
							managers.Cast<IUnitOrCategoryManagerDao<Category>>().ToList(), LocalConfig)
					);
				}
				return _categoryManager;
			}
		}

		private IUnitManagerDao _unitManager;
		public IUnitManagerDao UnitManager
		{
			get
			{
				if(_unitManager == null)
				{
					InitManager<IUnitManagerDao>(
						() => new UnitManagerFileRepo(),
						() => UnitManagerAdoNetFactory.Create(LocalConfig),
						() => UnitManagerEfFactory.Create(LocalConfig),
						managers => _unitManager = new UnitManagerDaoAggregate(
							managers.Cast<IUnitOrCategoryManagerDao<Unit>>().ToList(), LocalConfig)
					);
				}
				return _unitManager;
			}
		}

		private ITransactionItemManagerDao _transactionItemManager;
		public ITransactionItemManagerDao GetTransactionItemManager(ICategoryManager categoryManager, IUnitManager unitManager)
		{
			InitManagerIfNeeded(ref categoryManager);
			InitManagerIfNeeded(ref unitManager);

			if (_transactionItemManager == null)
			{
				InitManager<ITransactionItemManagerDao>(
					() => new TransactionItemManagerFileRepo(),
					() => TransactionItemManagerAdoNetFactory.Create(LocalConfig, categoryManager, unitManager),
					() => TransactionItemManagerEfFactory.Create(LocalConfig, categoryManager, unitManager),
					managers => _transactionItemManager = new TransactionItemManagerDaoAggregate(managers, LocalConfig)
					);
			}
			return _transactionItemManager;
		}

		private ISummaryItemManagerDao _summaryItemManager;
		public ISummaryItemManagerDao GetSummaryItemManager(ICategoryManager categoryManager, ITransactionItemManager transactionItemManager)
		{
			InitManagerIfNeeded(ref categoryManager);
			InitManagerIfNeeded(ref transactionItemManager);

			if (_summaryItemManager == null)
			{
				InitManager<ISummaryItemManagerDao>(
					() => new SummaryItemManagerFileRepo(transactionItemManager, categoryManager),
					() => SummaryItemManagerAdoNetFactory.Create(LocalConfig, categoryManager),
					() => SummaryItemManagerEfFactory.Create(LocalConfig, categoryManager),
					managers => _summaryItemManager = new SummaryItemManagerDaoAggregate(managers, LocalConfig)
				);
			}
			return _summaryItemManager;
		}

		#endregion

		#region InitManagerIfNeeded

		public void InitManagerIfNeeded(ref ICategoryManager categoryManager)
		{
			if (categoryManager == null)
			{
				var categoryManagerInstanceConfig = (CategoryManagerClass.Instance as IRepoConfigurableBase)?.LocalConfig;
				categoryManager = LocalConfig.Equals(categoryManagerInstanceConfig)
					? CategoryManagerClass.Instance
					: new CategoryManagerClass(LocalConfig);
			}
		}

		public void InitManagerIfNeeded(ref IUnitManager unitManager)
		{
			if(unitManager == null)
			{
				var unitManagerInstanceConfig = (UnitManagerClass.Instance as IRepoConfigurableBase)?.LocalConfig;
				unitManager = LocalConfig.Equals(unitManagerInstanceConfig)
					? UnitManagerClass.Instance
					: new UnitManagerClass(LocalConfig);
			}
		}

		public void InitManagerIfNeeded(ref ITransactionItemManager transactionItemManager)
		{
			if (transactionItemManager == null)
			{
				var transactionItemManagerInstanceConfig = (TransactionItemManagerClass.Instance as IRepoConfigurableBase)?.LocalConfig;
				transactionItemManager = LocalConfig.Equals(transactionItemManagerInstanceConfig)
					? TransactionItemManagerClass.Instance
					: new TransactionItemManagerClass(LocalConfig);
			}
		}

		#endregion

		#region Helpers

		private void InitManager<T>(
			Func<T> newFileRepoManager,
			Func<T> newDbAdoNetManager,
			Func<T> newDbEfManager,
			Action<List<T>> newAggregateManager
		)
		{
			var managers = new List<T>();

			var needsFrManager = !(LocalConfig.ReadMode == ReadMode.FromDb && LocalConfig.SaveMode == SaveMode.OnlyToDb);
			if(needsFrManager)
				managers.Add(newFileRepoManager());

			var needsDbManager = !(LocalConfig.ReadMode == ReadMode.FromFile && LocalConfig.SaveMode == SaveMode.OnlyToFile);
			if(needsDbManager)
				switch(LocalConfig.DbAccessMode)
				{
					case DbAccessMode.AdoNet:
						managers.Add(newDbAdoNetManager());
						break;
					case DbAccessMode.EntityFramework:
						managers.Add(newDbEfManager());
						break;
				}

			newAggregateManager(managers);
		}

		#endregion

	}
}
