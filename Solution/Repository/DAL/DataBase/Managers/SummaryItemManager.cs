using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.Db.Entities;
using Common.Log;
using Common.UiModels.WPF;
using DAL.DataBase.Managers.Factory;
using Localization;

namespace DAL.DataBase.Managers
{
	public interface ISummaryItemManager
	{
		void InsertOrUpdateDailyExpenseSummary(DateTime date);
		void InsertOrUpdateSummary(Summary summary, DateTime date, TransactionItemType transactionItemType);

		List<SummaryItem> GetDaily(DateTime date);
		List<SummaryItem> GetMontly(DateTime date);
		List<SummaryItem> GetYearly(DateTime date);
		List<SummaryItem> GetInterval(DateTime fromDate, DateTime toDate);
	}

	public abstract class SummaryItemManagerCommonBase : ISummaryItemManager
	{
		#region CREATE, UPDATE

		public void InsertOrUpdateMonthlyIncomeSummary(DateTime date)
		{
			date = new DateTime(date.Year, date.Month, 1);

			var monthlyIncomes = TransactionItemManager
				.GetMontly(date, TransactionItemType.Income)
				.Select(transactionItem => transactionItem.ToIncomeItem())
				.ToList();

			var summary = new Summary();
			foreach(var incomeItem in monthlyIncomes)
				summary.Update(incomeItem);

			InsertOrUpdateSummary(summary, date, TransactionItemType.Income);
		}

		public void InsertOrUpdateDailyExpenseSummary(DateTime date)
		{
			var dailyExpenses = TransactionItemManager
				.GetDaily(date, TransactionItemType.Expense)
				.Select(transactionItem => transactionItem.ToExpenseItem())
				.ToList();

			var summary = new Summary();
			foreach(var expenseItem in dailyExpenses)
				summary.Update(expenseItem);

			InsertOrUpdateSummary(summary, date, TransactionItemType.Expense);
		}

		public void InsertOrUpdateSummary(Summary summary, DateTime date, TransactionItemType transactionItemType)
		{
			try
			{
				if(transactionItemType == TransactionItemType.Income)
					date = new DateTime(date.Year, date.Month, 1);

				InserOrUpdateSummary_Exec(summary, date, transactionItemType);
			}
			catch(Exception e)
			{
				ExinLog.ger.LogException(Localized.Could_not_save_into_database_the_daily_expense_statistics, e);
				throw;
			}
		}

		protected static List<SummaryItem> CollectSummariesToInsert(Summary summary, DateTime date, bool isExpense)
		{
			var insertItems = new List<SummaryItem>();
			if(isExpense)
			{
				insertItems.Add(new SummaryItem()
				{
					Amount = summary.SumOut,
					CategoryID = CategoryManager.GetCategoryFullExpenseSummary.ID,
					Date = date,
				});

				insertItems.AddRange(
					summary.SumOutWithCategories.Select(
						keyValuePair =>
						{
							var category = keyValuePair.Key;
							var amount = keyValuePair.Value;
							return new SummaryItem()
							{
								Amount = amount,
								CategoryID = category.ID,
								Date = date,
							};
						})
					);
			}
			else //isIncome
			{
				insertItems.Add(new SummaryItem()
				{
					Amount = summary.SumIn,
					CategoryID = CategoryManager.GetCategoryFullIncomeSummary.ID,
					Date = date,
				});
			}
			return insertItems;
		}


		protected abstract void InserOrUpdateSummary_Exec(Summary summary, DateTime date, TransactionItemType transactionItemType);

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

		public List<SummaryItem> GetInterval(DateTime fromDate, DateTime toDate)
		{
			fromDate = fromDate.Date;
			toDate = toDate.Date;

			return GetInterval_Exec(fromDate, toDate);
		}
		
		public abstract List<SummaryItem> GetAll();
		public abstract List<SummaryItem> GetInterval_Exec(DateTime fromDate, DateTime toDate);

		#endregion
	}

	public static class SummaryItemManager
	{
		#region ITransactionItemManager singleton

		// --
		private static readonly ISummaryItemManager Manager = ManagerFactory.ISummaryItemManager;
		// --

		public static void InsertOrUpdateDailyExpenseSummary(DateTime date)
		{
			Manager.InsertOrUpdateDailyExpenseSummary(date);
		}

		public static void InsertOrUpdateSummary(Summary summary, DateTime date, TransactionItemType transactionItemType)
		{
			Manager.InsertOrUpdateSummary(summary, date, transactionItemType);
		}

		public static List<SummaryItem> GetDaily(DateTime date)
		{
			return Manager.GetDaily(date);
		}

		public static List<SummaryItem> GetMontly(DateTime date)
		{
			return Manager.GetMontly(date);
		}

		public static List<SummaryItem> GetYearly(DateTime date)
		{
			return Manager.GetYearly(date);
		}

		public static List<SummaryItem> GetInterval(DateTime fromDate, DateTime toDate)
		{
			return Manager.GetInterval(fromDate, toDate);
		}

		#endregion
	}
}
