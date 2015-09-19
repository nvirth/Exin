using System;
using System.Collections.Generic;
using Common;
using Common.Db.Entities;
using Common.UiModels.WPF;

namespace DAL.RepoCommon.Interfaces
{
	public interface ISummaryItemManagerDao
	{
		void ReplaceSummary(Summary summary, DateTime date, TransactionItemType transactionItemType);
		List<SummaryItem> GetInterval(DateTime fromDate, DateTime toDate);
	}
	public interface ISummaryItemManagerDb : ISummaryItemManagerDao
	{
		List<SummaryItem> GetAll();
	}
	public interface ISummaryItemManager : ISummaryItemManagerDao
	{
		void ReplaceDailyExpenseSummary(DateTime date);
		void ReplaceMonthlyIncomeSummary(DateTime date);

		List<SummaryItem> GetDaily(DateTime date);
		List<SummaryItem> GetMonthly(DateTime date);
		List<SummaryItem> GetYearly(DateTime date);
	}
}