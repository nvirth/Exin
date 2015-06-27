using System;
using System.Collections.Generic;
using Common.Configuration;
using Common.UiModels.WPF;
using DAL.RepoCommon.Interfaces;
using DAL.RepoCommon.Managers.Factory;

namespace DAL.RepoCommon.Managers
{
	public class TransactionItemManager : RepoConfigurableBase, ITransactionItemManager
	{
		public static readonly ITransactionItemManager Instance = new TransactionItemManager();
		private readonly ITransactionItemManagerDao _core;

		public TransactionItemManager(
			IRepoConfiguration repoConfiguration = null,
			ICategoryManager categoryManager = null,
			IUnitManager unitManager = null
			) : base(repoConfiguration)
		{
			_core = new ManagerDaoFactory(LocalConfig).GetTransactionItemManager(categoryManager, unitManager);
		}

		#region Delegated members from DAO (_core)

		public List<IncomeItem> GetMonthlyIncomes(DateTime date)
		{
			return _core.GetMonthlyIncomes(date);
		}

		public List<ExpenseItem> GetMonthlyExpenses(DateTime date)
		{
			return _core.GetMonthlyExpenses(date);
		}

		public void ReplaceDailyExpenses(IEnumerable<ExpenseItem> expenseItems, DateTime date)
		{
			_core.ReplaceDailyExpenses(expenseItems, date);
		}

		public void ReplaceMonthlyIncomes(IEnumerable<IncomeItem> incomeItems, DateTime date)
		{
			_core.ReplaceMonthlyIncomes(incomeItems, date);
		}

		public List<ExpenseItem> GetDailyExpenses(DateTime date)
		{
			return _core.GetDailyExpenses(date);
		}

		#endregion


	}
}
