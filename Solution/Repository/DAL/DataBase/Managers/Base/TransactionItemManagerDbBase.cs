using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.Configuration;
using Common.Db.Entities;
using Common.UiModels.WPF;
using DAL.RepoCommon;
using DAL.RepoCommon.Interfaces;

namespace DAL.DataBase.Managers.Base
{
	public abstract class TransactionItemManagerDbBase : RepoConfigurableBase, ITransactionItemManagerDb, IDbManagerMarker
	{
		protected readonly ICategoryManager CategoryManagerLocal;
		protected readonly IUnitManager UnitManagerLocal;

		protected TransactionItemManagerDbBase(IRepoConfiguration repoConfiguration,
			ICategoryManager categoryManager, IUnitManager unitManager) : base(repoConfiguration)
		{
			CategoryManagerLocal = categoryManager;
			UnitManagerLocal = unitManager;
		}

		#region READ

		public List<TransactionItem> GetDaily(DateTime date, TransactionItemType transactionItemType)
		{
			if(transactionItemType == TransactionItemType.Income)
				date = new DateTime(date.Year, date.Month, 1);

			var fromDate = date.Date;
			var toDate = date.Date;

			var transactionItems = GetInterval(fromDate, toDate, transactionItemType);
			return transactionItems;
		}

		public List<TransactionItem> GetMontly(DateTime date, TransactionItemType transactionItemType)
		{
			var fromDate = new DateTime(date.Year, date.Month, 1);
			var toDate = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));

			var transactionItems = GetInterval(fromDate, toDate, transactionItemType);
			return transactionItems;
		}

		public List<TransactionItem> GetYearly(DateTime date, TransactionItemType transactionItemType)
		{
			var fromDate = new DateTime(date.Year, 1, 1);
			var toDate = new DateTime(date.Year, 12, 31);

			var transactionItems = GetInterval(fromDate, toDate, transactionItemType);
			return transactionItems;
		}

		#endregion

		#region Abstract

		public abstract List<TransactionItem> GetInterval(DateTime fromDate, DateTime toDate, TransactionItemType transactionItemType);
		public abstract List<TransactionItem> GetAll(TransactionItemType? transactionItemType);
		public abstract void Insert(TransactionItem transactionItem, bool withId = false);
		public abstract void InsertMany(IEnumerable<TransactionItem> transactionItems, bool withId = false, bool forceOneByOne = false);
		public abstract int UpdateFullRecord(TransactionItem transactionItem);
		public abstract int Delete(int id);
		public abstract int ClearDay(DateTime date, TransactionItemType transactionItemType);
		public abstract void ReplaceDailyItems(IEnumerable<TransactionItem> transactionItems, TransactionItemType transactionItemType, DateTime date);

		#endregion

		#region ITransactionItemManagerDao implementation (wrappers)

		public List<ExpenseItem> GetDailyExpenses(DateTime date)
		{
			return GetDaily(date, TransactionItemType.Expense)
				.Select(ti => ti.ToExpenseItem()).ToList();
		}

		public List<IncomeItem> GetMonthlyIncomes(DateTime date)
		{
			//return GetDaily(date, TransactionItemType.Income);
			return GetMontly(date, TransactionItemType.Income)
				.Select(ti => ti.ToIncomeItem()).ToList();
		}

		public List<ExpenseItem> GetMonthlyExpenses(DateTime date)
		{
			return GetMontly(date, TransactionItemType.Expense)
				.Select(ti => ti.ToExpenseItem()).ToList();
		}

		public void ReplaceDailyExpenses(IEnumerable<ExpenseItem> expenseItems, DateTime date)
		{
			var transactionItems = expenseItems.Select(ei => ei.ToTransactionItem());
            ReplaceDailyItems(transactionItems, TransactionItemType.Expense, date);
		}

		public void ReplaceMonthlyIncomes(IEnumerable<IncomeItem> incomeItems, DateTime date)
		{
			date = new DateTime(date.Year, date.Month, 1);
			var transactionItems = incomeItems.Select(ii => ((IncomeItem)ii.WithMonthDate()).ToTransactionItem(CategoryManagerLocal));

			// Monthly incomes in DB are stored at the 1. day of the month
			ReplaceDailyItems(transactionItems, TransactionItemType.Income, date);
		}

		#endregion
	}
}