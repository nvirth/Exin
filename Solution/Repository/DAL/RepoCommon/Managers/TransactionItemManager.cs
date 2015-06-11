using System;
using System.Collections.Generic;
using Common.Configuration;
using Common.UiModels.WPF;
using DAL.DataBase.Managers.Factory;
using DAL.RepoCommon.Interfaces;

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
			_core = new ManagerDaoFactory(repoConfiguration).GetTransactionItemManager(categoryManager, unitManager);
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

		public void ReplaceDailyExpenses(IList<ExpenseItem> expenseItems, DateTime date)
		{
			_core.ReplaceDailyExpenses(expenseItems, date);
		}

		public void ReplaceMonthlyIncomes(IList<IncomeItem> incomeItems, DateTime date)
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
