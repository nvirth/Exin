using System;
using System.Collections.Generic;
using Common;
using Common.Configuration;
using Common.Db.Entities;
using Common.UiModels.WPF;
using Common.Utils.Helpers;
using DAL.RepoCommon.Interfaces;
using DAL.RepoCommon.Managers.AggregateDaoManagers.Base;

namespace DAL.RepoCommon.Managers.AggregateDaoManagers
{
	public class SummaryItemManagerDaoAggregate : AggregateManagerBase<ISummaryItemManagerDao>, ISummaryItemManagerDao
	{
		public SummaryItemManagerDaoAggregate(List<ISummaryItemManagerDao> managers, IRepoConfiguration repoConfiguration) : base(managers, repoConfiguration)
		{
		}

		public List<SummaryItem> GetInterval(DateTime fromDate, DateTime toDate)
		{
			return ManagerForRead.GetInterval(fromDate, toDate);
		}

		public void ReplaceSummary(Summary summary, DateTime date, TransactionItemType transactionItemType)
		{
			ManagersForWrite.ForEach(dao => dao.ReplaceSummary(summary, date, transactionItemType));
		}
	}
}