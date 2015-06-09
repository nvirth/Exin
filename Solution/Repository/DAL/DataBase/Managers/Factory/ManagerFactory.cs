using System;
using Common;
using Common.Configuration;
using Common.Log;
using DAL.DataBase.AdoNet.Managers;
using DAL.DataBase.EntityFramework.Managers;
using Localization;

namespace DAL.DataBase.Managers.Factory
{
	public class ManagerFactory : DbConfigurableBase
	{
		public ManagerFactory(IRepoConfiguration repoConfiguration = null) : base(InitConfiguration(repoConfiguration))
		{
			switch(LocalConfig.DbAccessMode)
			{
				case DbAccessMode.AdoNet:
				case DbAccessMode.EntityFramework:
					// Config is ok, implemented
					break;

				default:
					var msg = Localized.ManagerFactory_is_not_implemented_for__ + LocalConfig.DbAccessMode;
					ExinLog.ger.LogError(msg);
					throw new NotImplementedException(msg);
			}
		}

		private static IRepoConfiguration InitConfiguration(IRepoConfiguration repoConfiguration)
		{
			return repoConfiguration ?? new RepoConfiguration();
		}

		// --

		private ICategoryManager _categoryManager;
		public ICategoryManager CategoryManager
		{
			get
			{
				if(_categoryManager == null)
				{
					switch(LocalConfig.DbAccessMode)
					{
						case DbAccessMode.AdoNet:
							_categoryManager = CategoryManagerAdoNetFactory.Create(LocalConfig);
							break;

						case DbAccessMode.EntityFramework:
							_categoryManager = CategoryManagerEfFactory.Create(LocalConfig);
							break;
					}
				}
				return _categoryManager;
			}
		}

		private IUnitManager _unitManager;
		public IUnitManager UnitManager
		{
			get
			{
				if(_unitManager == null)
				{
					switch(LocalConfig.DbAccessMode)
					{
						case DbAccessMode.AdoNet:
							_unitManager = UnitManagerAdoNetFactory.Create(LocalConfig);
							break;

						case DbAccessMode.EntityFramework:
							_unitManager = UnitManagerEfFactory.Create(LocalConfig);
							break;
					}
				}
				return _unitManager;
			}
		}

		private ITransactionItemManager _transactionItemManager;
		public ITransactionItemManager TransactionItemManager
		{
			get
			{
				if(_transactionItemManager == null)
				{
					switch(LocalConfig.DbAccessMode)
					{
						case DbAccessMode.AdoNet:
							_transactionItemManager = TransactionItemManagerAdoNetFactory.Create(LocalConfig);
							break;

						case DbAccessMode.EntityFramework:
							_transactionItemManager = TransactionItemManagerEfFactory.Create(LocalConfig);
							break;
					}
				}
				return _transactionItemManager;
			}
		}

		private ISummaryItemManager _summaryItemManager;
		public ISummaryItemManager SummaryItemManager
		{
			get
			{
				if(_summaryItemManager == null)
				{
					switch(LocalConfig.DbAccessMode)
					{
						case DbAccessMode.AdoNet:
							_summaryItemManager = SummaryItemManagerAdoNetFactory.Create(LocalConfig);
							break;

						case DbAccessMode.EntityFramework:
							_summaryItemManager = SummaryItemManagerEfFactory.Create(LocalConfig);
							break;
					}
				}
				return _summaryItemManager;
			}
		}
	}
}
