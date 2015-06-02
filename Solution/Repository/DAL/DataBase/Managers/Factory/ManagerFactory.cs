using System;
using Common;
using Common.Log;
using DAL.DataBase.AdoNet.Managers;
using DAL.DataBase.EntityFramework.Managers;
using Localization;
using Config = Common.Config.Config;

namespace DAL.DataBase.Managers.Factory
{
	public static class ManagerFactory
	{
		public static readonly ICategoryManager ICategoryManager;
		public static readonly IUnitManager IUnitManager;
		public static readonly ITransactionItemManager ITransactionItemManager;
		public static readonly ISummaryItemManager ISummaryItemManager;

		static ManagerFactory()
		{
			switch(Config.DbAccessMode)
			{
				case DbAccessMode.AdoNet:
					ICategoryManager = CategoryManagerAdoNetFactory.Create();
					IUnitManager = UnitManagerAdoNetFactory.Create();
					ITransactionItemManager = TransactionItemManagerAdoNetFactory.Create();
					ISummaryItemManager = SummaryItemManagerAdoNetFactory.Create();
					break;

				case DbAccessMode.EntityFramework:
					ICategoryManager = CategoryManagerEfFactory.Create();
					IUnitManager = UnitManagerEfFactory.Create();
					ITransactionItemManager = TransactionItemManagerEfFactory.Create();
					ISummaryItemManager = SummaryItemManagerEfFactory.Create();
					break;

				default:
				{
					var msg = Localized.ManagerFactory_is_not_implemented_for__ + Config.DbAccessMode;
					ExinLog.ger.LogError(msg);
					throw new NotImplementedException(msg);
				}
			}
		}
	}
}
