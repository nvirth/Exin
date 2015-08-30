using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Common.UiModels.WPF;
using Common.Utils;
using Common.Utils.Helpers;
using DAL.DataBase.Managers;
using DAL.RepoCommon.Managers;
using Localization;

namespace BLL.WpfManagers
{
	public class DailyExpenses : SummaryEngineBase
	{
		#region Ctors

		public DailyExpenses(bool doWork = true) : this(DateTime.Now, doWork)
		{ }
		public DailyExpenses(DateTime dateTime, bool doWork = true) : this(new DatePaths(dateTime), doWork)
		{ }
		public DailyExpenses(DatePaths datePaths, bool doWork = true) : base(datePaths, doWork)
		{ }

		#endregion

		#region Methods

		protected override void ReadDataMessage()
		{
			MessagePresenter.Instance.WriteLineSeparator();

			if(DatePaths.Date.Date == DateTime.Today)
				MessagePresenter.Instance.WriteLine(Localized.Today_s_expenses_loading__);
			else
				MessagePresenter.Instance.WriteLine(string.Format(Localized.__0__loading_daily_expenses__FORMAT__, 
					DatePaths.Date.ToLocalizedShortDateString()));
		}

		protected override void ReadData()
		{
			TransactionItemManager.Instance.GetDailyExpenses(DatePaths.Date).ForEach(Add);
		}

		protected override void WriteData()
		{
			ReplaceDailyExpenses();
			TaskManager.Run((Action)ReplaceSummary);
		}

		private void ReplaceDailyExpenses()
		{
			try
			{
				var expenseItems = TransactionItems.Cast<ExpenseItem>().ToList();
				TransactionItemManager.Instance.ReplaceDailyExpenses(expenseItems, DatePaths.Date);

				MessagePresenter.Instance.Write(DatePaths.Date.ToLocalizedShortDateString());
				MessagePresenter.Instance.WriteLine(Localized._daily_expenses_successfully_saved_);
			}
			catch (Exception e)
			{
				throw ExinLog.ger.LogException(Localized.The_saving_of_the_daily_expenses_was_unsuccessful_, e);
			}
		}

		private void ReplaceSummary()
		{
			try
			{
				SummaryItemManager.Instance.ReplaceSummary(Summary, DatePaths.Date, TransactionItemType.Expense);

				MessagePresenter.Instance.Write(DatePaths.Date.ToLocalizedShortDateString());
				MessagePresenter.Instance.WriteLine(Localized._expense_statistics_successfully_saved_);
			}
			catch(Exception e)
			{
				throw ExinLog.ger.LogException(Localized.The_saving_of_expense_statistics_was_unsuccessful_, e);
			}
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
