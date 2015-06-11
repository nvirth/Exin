using System;
using System.Collections.Generic;
using Common;
using Common.Configuration;
using Common.Db.Entities;
using Common.UiModels.WPF;
using DAL.DataBase.Managers.Factory;
using DAL.RepoCommon.Interfaces;

namespace DAL.RepoCommon.Managers
{
	public class SummaryItemManager : RepoConfigurableBase, ISummaryItemManager
	{
		public static readonly ISummaryItemManager Instance = new SummaryItemManager();
		private readonly ISummaryItemManagerDao _core;
		private readonly ITransactionItemManager _transactionItemManager;

		public SummaryItemManager(
				IRepoConfiguration repoConfiguration = null,
				ITransactionItemManager transactionItemManager = null,
				ICategoryManager categoryManager = null
			) : base(repoConfiguration)
		{
			var managerDaoFactory = new ManagerDaoFactory(repoConfiguration);
			managerDaoFactory.InitManagerIfNeeded(ref transactionItemManager);

			_transactionItemManager = transactionItemManager;
			_core = managerDaoFactory.GetSummaryItemManager(categoryManager, transactionItemManager);
		}

		#region CREATE, UPDATE

		public void ReplaceMonthlyIncomeSummary(DateTime date)
		{
			date = new DateTime(date.Year, date.Month, 1);

			var monthlyIncomes = _transactionItemManager.GetMonthlyIncomes(date);
			var summary = Summary.Summarize(monthlyIncomes);

			ReplaceSummary(summary, date, TransactionItemType.Income);
		}

		public void ReplaceDailyExpenseSummary(DateTime date)
		{
			var dailyExpenses = _transactionItemManager.GetDailyExpenses(date);
			var summary = Summary.Summarize(dailyExpenses);

			ReplaceSummary(summary, date, TransactionItemType.Expense);
		}

		#endregion

		#region READ

		public List<SummaryItem> GetDaily(DateTime date)
		{
			var fromDate = date.Date;
			var toDate = date.Date;

			var transactionItems = GetInterval(fromDate, toDate);
			return transactionItems;
		}

		public List<SummaryItem> GetMontly(DateTime date)
		{
			var fromDate = new DateTime(date.Year, date.Month, 1);
			var toDate = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));

			var transactionItems = GetInterval(fromDate, toDate);
			return transactionItems;
		}

		public List<SummaryItem> GetYearly(DateTime date)
		{
			var fromDate = new DateTime(date.Year, 1, 1);
			var toDate = new DateTime(date.Year, 12, 31);

			var transactionItems = GetInterval(fromDate, toDate);
			return transactionItems;
		}

		#endregion

		#region Delegated members from DAO (_core)

		public void ReplaceSummary(Summary summary, DateTime date, TransactionItemType transactionItemType)
		{
			_core.ReplaceSummary(summary, date, transactionItemType);
		}

		public List<SummaryItem> GetInterval(DateTime fromDate, DateTime toDate)
		{
			return _core.GetInterval(fromDate, toDate);
		}

		#endregion
	}
}
