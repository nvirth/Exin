using System;
using System.Collections.Generic;
using Common;
using Common.Configuration;
using Common.Db.Entities;
using Common.UiModels.WPF;
using Common.Utils.Helpers;
using DAL.RepoCommon.AggregateDaoManagers.Base;
using DAL.RepoCommon.Interfaces;

namespace DAL.RepoCommon.AggregateDaoManagers
{
	public class SummaryItemManagerDaoAggregate : AggregateManagerBase<ISummaryItemManagerDao>, ISummaryItemManagerDao
	{
		public SummaryItemManagerDaoAggregate(List<ISummaryItemManagerDao> managers, IRepoConfiguration repoConfiguration) : base(managers, repoConfiguration)
		{
		}

		public List<SummaryItem> GetInterval(DateTime fromDate, DateTime toDate)
		{
			switch(LocalConfig.ReadMode)
			{
				case ReadMode.FromFile:
					return FirstFileRepoManager.GetInterval(fromDate, toDate);
				case ReadMode.FromDb:
					return FirstDbManager.GetInterval(fromDate, toDate);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public void ReplaceSummary(Summary summary, DateTime date, TransactionItemType transactionItemType)
		{
			switch(LocalConfig.SaveMode)
			{
				case SaveMode.OnlyToFile:
					AllFileRepoManagers.ForEach(dao => ReplaceSummary(summary, date, transactionItemType));
					break;
				case SaveMode.FileAndDb:
					AllManagers.ForEach(dao => ReplaceSummary(summary, date, transactionItemType));
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}