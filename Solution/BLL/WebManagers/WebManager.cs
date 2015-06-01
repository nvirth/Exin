using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.DbEntities;
using Common.Log;
using Common.UiModels.WEB;
using Common.UiModels.WPF;
using DAL;
using DAL.DataBase.Managers;
using DAL.FileRepo;
using Config = Common.Config.Config;

namespace BLL.WebManagers
{
	public static class WebManager
	{
		public static void SaveTransactionItems(IEnumerable<TransactionItemVM> transactionItemVms, DateTime date, TransactionItemType transactionItemType)
		{
			var isExpense = transactionItemType == TransactionItemType.Expense;
			date = isExpense ? date.Date : new DateTime(date.Year, date.Month, 1);
			var paths = new DatePaths(date);

			if(transactionItemVms == null)
				transactionItemVms = new TransactionItemVM[0];

			var transactionItems = transactionItemVms
				.Select(tiVm => tiVm.ToTransactionItem(transactionItemType, date))
				.ToList();

			var summary = new Summary();
			var transactionItemBases = transactionItems
				.Select(ti =>
						{
							var tib = isExpense
								? (TransactionItemBase)ti.ToExpenseItem()
								: (TransactionItemBase)ti.ToIncomeItem();
							summary.Update(tib);
							return tib;
						})
				.ToList();

			// -- Save to file

			if(isExpense)
				FileRepoManager.WriteOutDailyExpenses(transactionItemBases, paths, summary);
			else //isIncome
				FileRepoManager.WriteOutMonthlyIncomes(transactionItemBases, paths, summary);

			if(Config.SaveMode == SaveMode.FileAndDb)
			{
				// -- Save to db
				TransactionItemManager.ReplaceDailyItems(transactionItems, transactionItemType, date);

				// -- Save summary item (to db)
				SummaryItemManager.InsertOrUpdateSummary(summary, date, transactionItemType);
			}
		}

		public static List<TransactionItemVM> GetTransactionItems(DateTime date, TransactionItemType type)
		{
			switch(type)
			{
				case TransactionItemType.Expense:
					return GetDailyExpenses(date);
					break;
				case TransactionItemType.Income:
					return GetMonthlyIncomes(date);
					break;
				default:
					var msg = "WebManager.GetTransactionItems method's type parameter invalid: " + type.ToString();
					ExinLog.ger.LogError(msg);
					throw new InvalidOperationException(msg);
			}
		}

		public static List<TransactionItemVM> GetDailyExpenses(DateTime date)
		{
			List<TransactionItemVM> result;

			switch(Config.ReadMode)
			{
				case ReadMode.FromFile:
					result = FileRepoManager.GetDailyExpenses(date)
						.Select(ei => ei.ToTransactionItemVm())
						.ToList();
					break;

				case ReadMode.FromDb:
					result = TransactionItemManager.GetDaily(date, TransactionItemType.Expense)
						.Select(ti => ti.ToTransactionItemVm())
						.ToList();
					break;

				default:
					var msg = "WebManager.GetDailyExpenses method is not implemented for ReadMode: " + Config.ReadMode;
					ExinLog.ger.LogError(msg);
					throw new NotImplementedException(msg);
			}

			return WithSort(result);
		}

		public static List<TransactionItemVM> GetMonthlyIncomes(DateTime date)
		{
			List<TransactionItemVM> result;

			switch(Config.ReadMode)
			{
				case ReadMode.FromFile:
					result = FileRepoManager.GetMonthlyIncomes(date)
						.Select(ii => ii.ToTransactionItemVm())
						.ToList();
					break;

				case ReadMode.FromDb:
					result = TransactionItemManager.GetDaily(date, TransactionItemType.Income)
						.Select(ti => ti.ToTransactionItemVm())
						.ToList();
					break;

				default:
					var msg = "WebManager.GetMonthlyIncomes method is not implemented for ReadMode: " + Config.ReadMode;
					ExinLog.ger.LogError(msg);
					throw new NotImplementedException(msg);
			}

			return WithSort(result);
		}

		public static List<TransactionItemVM> GetMonthlySummaries(DateTime date, bool writeOutSummaeries = true)
		{
			Summary summary;
			return GetMonthlySummaries(date, out summary, writeOutSummaeries);
		}

		public static List<TransactionItemVM> GetMonthlySummaries(DateTime date, out Summary summary, bool writeOutSummaeries = true)
		{
			List<TransactionItemVM> result;
			List<ExpenseItem> monthlyExpenses;
			var summaryLocal = new Summary();

			switch(Config.ReadMode)
			{
				case ReadMode.FromFile:
					monthlyExpenses = FileRepoManager.GetMonthlyExpenses(date);
					result = monthlyExpenses
						.Select(ei =>
								{
									summaryLocal.Update(ei);
									return ei.ToTransactionItemVm();
								})
						.ToList();

					result.AddRange(FileRepoManager.GetMonthlyIncomes(date)
						.Select(ii =>
								{
									summaryLocal.Update(ii);
									return ii.ToTransactionItemVm();
								}));

					//FileRepoManager.GetMonthlyIncomes(date) // Incomes later, usually thiner
					//	.ForEach(ii => UtilsSharedPortable.Helpers.InsertIntoSorted(result, ii.ToTransactionItemVm()));
					break;

				case ReadMode.FromDb:
					var expenseItems = new List<ExpenseItem>();
					result = TransactionItemManager.GetMontly(date, TransactionItemType.Both)
						.Select(ti =>
								{
									if(ti.IsExpenseItem)
										expenseItems.Add(ti.ToExpenseItem());
									summaryLocal.Update(ti);
									return ti.ToTransactionItemVm();
								})
						.ToList();
					monthlyExpenses = expenseItems;
					break;

				default:
					var msg = "WebManager.GetMonthlySummaries method is not implemented for ReadMode: " + Config.ReadMode;
					ExinLog.ger.LogError(msg);
					throw new NotImplementedException(msg);
			}

			if(writeOutSummaeries)
				new TaskFactory().StartNew( // in a new thread, to speed up the response
					() =>
					{
						monthlyExpenses.Sort((a, b) => -1 * a.CompareTo(b));
						FileRepoManager.WriteOutMonthlySummaries(summaryLocal, new DatePaths(date), monthlyExpenses);
					});

			summary = summaryLocal;
			return WithSort(result);
		}

		private static List<TransactionItemVM> WithSort(List<TransactionItemVM> list)
		{
			list.Sort((a, b) => -1 * a.CompareTo(b));
			return list;
		}
	}
}
