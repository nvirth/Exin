using System;
using Common;
using Common.Log;
using DAL.DataBase.AdoNet.Managers;
using DAL.DataBase.EntityFramework.Managers;
using Localization;
using Config = Common.Configuration.Config;

namespace DAL.DataBase.Managers.Factory
{
	public class ManagerFactory : DbConfigurableBase
	{
		public ManagerFactory(DbType dbType, DbAccessMode dbAccessMode) : base(dbType, dbAccessMode)
		{
			switch(DbAccessMode)
			{
				case DbAccessMode.AdoNet:
				case DbAccessMode.EntityFramework:
					// Config is ok, implemented
					break;

				default:
					var msg = Localized.ManagerFactory_is_not_implemented_for__ + Config.DbAccessMode;
					ExinLog.ger.LogError(msg);
					throw new NotImplementedException(msg);
			}
		}

		// --

		private ICategoryManager _categoryManager;
		public ICategoryManager CategoryManager
		{
			get
			{
				if(_categoryManager == null)
				{
					switch(DbAccessMode)
					{
						case DbAccessMode.AdoNet:
							_categoryManager = CategoryManagerAdoNetFactory.Create(DbType, DbAccessMode);
							break;

						case DbAccessMode.EntityFramework:
							_categoryManager = CategoryManagerEfFactory.Create(DbType, DbAccessMode);
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
					switch(DbAccessMode)
					{
						case DbAccessMode.AdoNet:
							_unitManager = UnitManagerAdoNetFactory.Create(DbType, DbAccessMode);
							break;

						case DbAccessMode.EntityFramework:
							_unitManager = UnitManagerEfFactory.Create(DbType, DbAccessMode);
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
					switch(DbAccessMode)
					{
						case DbAccessMode.AdoNet:
							_transactionItemManager = TransactionItemManagerAdoNetFactory.Create(DbType, DbAccessMode);
							break;

						case DbAccessMode.EntityFramework:
							_transactionItemManager = TransactionItemManagerEfFactory.Create(DbType, DbAccessMode);
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
					switch(DbAccessMode)
					{
						case DbAccessMode.AdoNet:
							_summaryItemManager = SummaryItemManagerAdoNetFactory.Create(DbType, DbAccessMode);
							break;

						case DbAccessMode.EntityFramework:
							_summaryItemManager = SummaryItemManagerEfFactory.Create(DbType, DbAccessMode);
							break;
					}
				}
				return _summaryItemManager;
			}
		}
	}
}
