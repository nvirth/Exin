using System;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Common.UiModels.WPF;
using Common.Utils;
using DAL;
using DAL.DataBase.Managers;
using DAL.FileRepo;
using Localization;

namespace BLL.WpfManagers
{
	public class DailyExpenses : SummaryEngineBase
	{
		#region Ctors

		public DailyExpenses(bool doWork = true) : this(DateTime.Now, doWork) { }
		public DailyExpenses(DateTime dateTime, bool doWork = true) : this(new DatePaths(dateTime), doWork) { }
		public DailyExpenses(DatePaths datePaths, bool doWork = true) : base(datePaths, doWork) { }

		#endregion

		#region Methods

		protected override void ReadDataMessage()
		{
			MessagePresenter.Instance.WriteLineSeparator();

			if(DatePaths.Date.Date == DateTime.Now.Date)
				MessagePresenter.Instance.WriteLine(Localized.Today_s_expenses_loading__);
			else
				MessagePresenter.Instance.WriteLine(string.Format(Localized.__0__loading_daily_expenses__FORMAT__, DatePaths.Date.ToString(Localized.DateFormat_full)));
		}

		protected override void ReadDataFromDb()
		{
			var transactionItems = TransactionItemManager.Instance.GetDaily(DatePaths.Date, TransactionItemType.Expense);
			foreach(var transactionItem in transactionItems)
			{
				Add(transactionItem.ToExpenseItem());
			}
		}

		protected override void ReadDataFromFile()
		{
			FileRepoManager.Instance.GetDailyExpenses(DatePaths).ForEach(Add);
		}

		protected override void SaveToDb()
		{
			try
			{
				var transactionItems = TransactionItems.Cast<ExpenseItem>().Select(ei => ei.ToTransactionItem()).ToList();
				TransactionItemManager.Instance.ReplaceDailyItems(transactionItems, TransactionItemType.Expense, DatePaths.Date);
				MessagePresenter.Instance.Write(DatePaths.Date.ToString(Localized.DateFormat_full));
				MessagePresenter.Instance.WriteLine(Localized._daily_expenses_successfully_saved_into_database);
			}
			catch(Exception e)
			{
				ExinLog.ger.LogException(Localized.The_saving_of_the_daily_expenses_into_database_was_unsuccessful, e);
				throw;
			}
		}

		protected override void SaveSummariesToDb()
		{
			new TaskFactory().StartNew(SaveSummariesToDb_Work);
		}

		private void SaveSummariesToDb_Work()
		{
			try
			{
				SummaryItemManager.Instance.InsertOrUpdateSummary(Summary, DatePaths.Date, TransactionItemType.Expense);
				MessagePresenter.Instance.Write(DatePaths.Date.ToString(Localized.DateFormat_full));
				MessagePresenter.Instance.WriteLine(Localized._daily_expense_statistics_successfully_saved_into_database);
			}
			catch(Exception e)
			{
				ExinLog.ger.LogException(Localized.The_saving_of_daily_expense_statistics_into_database_was_unsuccessful, e);
				throw;
			}
		}

		protected override void SaveToFile()
		{
			FileRepoManager.Instance.WriteOutDailyExpenses(TransactionItems, DatePaths, Summary);
		}

		public ExpenseItem GetTheEqual(ExpenseItem expenseItem)
		{
			var equalTib = TransactionItems.FirstOrDefault(
				tib => ExpenseItem.DefaultExpenseItemComparer.Equals(tib as ExpenseItem, expenseItem));

			return equalTib as ExpenseItem;

			//var equalTib = base.GetTheEqual(expenseItem);
			//if(equalTib == null)
			//	return null;

			//var equalExpenseItem = equalTib as ExpenseItem;
			//if (equalExpenseItem == null)
			//	return null;

			//if (equalExpenseItem.Category == expenseItem.Category)
			//	return equalExpenseItem;
			//else
			//	return null;
		}

		#endregion
	}
}
