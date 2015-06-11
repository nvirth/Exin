using System;
using System.Collections.Generic;
using Common;
using Common.Configuration;
using Common.UiModels.WPF;
using Common.Utils.Helpers;
using DAL.RepoCommon.Interfaces;
using DAL.RepoCommon.Managers.AggregateDaoManagers.Base;

namespace DAL.RepoCommon.Managers.AggregateDaoManagers
{
	public class TransactionItemManagerDaoAggregate : AggregateManagerBase<ITransactionItemManagerDao>, ITransactionItemManagerDao
	{
		public TransactionItemManagerDaoAggregate(List<ITransactionItemManagerDao> managers, IRepoConfiguration repoConfiguration) : base(managers, repoConfiguration)
		{
		}

		public List<ExpenseItem> GetDailyExpenses(DateTime date)
		{
			return ManagerForRead.GetDailyExpenses(date);
		}

		public List<IncomeItem> GetMonthlyIncomes(DateTime date)
		{
			return ManagerForRead.GetMonthlyIncomes(date);
		}

		public List<ExpenseItem> GetMonthlyExpenses(DateTime date)
		{
			return ManagerForRead.GetMonthlyExpenses(date);
		}

		public void ReplaceDailyExpenses(IList<ExpenseItem> expenseItems, DateTime date)
		{
			ManagersForWrite.ForEach(dao => dao.ReplaceDailyExpenses(expenseItems, date));
		}

		public void ReplaceMonthlyIncomes(IList<IncomeItem> incomeItems, DateTime date)
		{
			ManagersForWrite.ForEach(dao => dao.ReplaceMonthlyIncomes(incomeItems, date));
		}
	}
}