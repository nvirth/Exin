using System;
using System.Collections.Generic;
using Common;
using Common.Db.Entities;
using Common.UiModels.WPF;

namespace DAL.RepoCommon.Interfaces
{
	public interface ITransactionItemManagerDao
	{
		List<ExpenseItem> GetDailyExpenses(DateTime date);
		List<IncomeItem> GetMonthlyIncomes(DateTime date);
		List<ExpenseItem> GetMonthlyExpenses(DateTime date);

		void ReplaceDailyExpenses(IList<ExpenseItem> expenseItems, DateTime date);
		void ReplaceMonthlyIncomes(IList<IncomeItem> incomeItems, DateTime date);
	}

	public interface ITransactionItemManagerDb : ITransactionItemManagerDao
	{
		List<TransactionItem> GetDaily(DateTime date, TransactionItemType transactionItemType);
		List<TransactionItem> GetMontly(DateTime date, TransactionItemType transactionItemType);
		List<TransactionItem> GetYearly(DateTime date, TransactionItemType transactionItemType);
		List<TransactionItem> GetInterval(DateTime fromDate, DateTime toDate, TransactionItemType transactionItemType);
		List<TransactionItem> GetAll(TransactionItemType? transactionItemType);

		void Insert(TransactionItem transactionItem, bool withId = false);
		void InsertMany(IList<TransactionItem> transactionItems, bool withId = false, bool forceOneByOne = false);

		int UpdateFullRecord(TransactionItem transactionItem);

		int Delete(int id);
		int ClearDay(DateTime date, TransactionItemType transactionItemType);

		void ReplaceDailyItems(IList<TransactionItem> transactionItems, TransactionItemType transactionItemType, DateTime date);
	}

	public interface ITransactionItemManager : ITransactionItemManagerDao
	{
	}
}