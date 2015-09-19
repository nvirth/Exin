using System;
using System.Collections.Generic;
using Common;
using Common.Db.Entities;
using Common.UiModels.WPF;

namespace DAL.RepoCommon.Interfaces
{
	public interface ITransactionItemManagerDao
	{
		/// <summary>
		/// Returns the first transaction item's date. <para />
		/// It will throw Exception if the Repo is empty yet. <para />
		///  <para />
		/// NOTE: FileRepo implementation is not precise, only the year and month. <para />
		/// </summary>
		DateTime GetFirstDate();

		/// <summary>
		/// Returns the last transaction item's date. <para />
		/// It will throw Exception if the Repo is empty yet. <para />
		///  <para />
		/// NOTE: FileRepo implementation is not precise, only the year and month. <para />
		/// </summary>
		DateTime GetLastDate();

		List<ExpenseItem> GetDailyExpenses(DateTime date);
		List<IncomeItem> GetMonthlyIncomes(DateTime date);
		List<ExpenseItem> GetMonthlyExpenses(DateTime date);

		void ReplaceDailyExpenses(IEnumerable<ExpenseItem> expenseItems, DateTime date);
		void ReplaceMonthlyIncomes(IEnumerable<IncomeItem> incomeItems, DateTime date);
	}

	public interface ITransactionItemManagerDb : ITransactionItemManagerDao
	{
		List<TransactionItem> GetDaily(DateTime date, TransactionItemType transactionItemType);
		List<TransactionItem> GetMonthly(DateTime date, TransactionItemType transactionItemType);
		List<TransactionItem> GetYearly(DateTime date, TransactionItemType transactionItemType);
		List<TransactionItem> GetInterval(DateTime fromDate, DateTime toDate, TransactionItemType transactionItemType);
		List<TransactionItem> GetAll(TransactionItemType? transactionItemType);

		void Insert(TransactionItem transactionItem, bool? withId = null);
		void InsertMany(IEnumerable<TransactionItem> transactionItems, bool? withId = null, bool forceOneByOne = false);

		int UpdateFullRecord(TransactionItem transactionItem);

		int Delete(int id);
		int ClearDay(DateTime date, TransactionItemType transactionItemType);

		void ReplaceDailyItems(IEnumerable<TransactionItem> transactionItems, TransactionItemType transactionItemType, DateTime date);
	}

	public interface ITransactionItemManager : ITransactionItemManagerDao
	{
	}
}