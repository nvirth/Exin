using System;
using System.Linq;
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
	public class MonthlyIncomes : SummaryEngineBase
	{
		public MonthlyIncomes(bool doWork = true) : this(DateTime.Now, doWork) { }
		public MonthlyIncomes(DateTime dateTime, bool doWork = true) : this(new DatePaths(dateTime), doWork) { }
		public MonthlyIncomes(DatePaths datePaths, bool doWork = true) : base(datePaths, doWork) { }

		protected override void ReadDataMessage()
		{
			var isThisMonth = DatePaths.Date.Year == DateTime.Today.Year && DatePaths.Date.Month == DateTime.Today.Month;
			var msgStart = isThisMonth ? Localized.Instant_ : DatePaths.Date.ToString(Localized.DateFormat_year_month) + Localized._monthly_;
			MessagePresenter.WriteLineSeparator();
			MessagePresenter.WriteLine(msgStart + Localized.incomes_loading__);
		}

		protected override void ReadDataFromDb()
		{
			var transactionItems = TransactionItemManager.GetDaily(DatePaths.Date, TransactionItemType.Income);
			foreach(var transactionItem in transactionItems)
			{
				Add(transactionItem.ToIncomeItem());
			}
		}

		protected override void ReadDataFromFile()
		{
			FileRepoManager.GetMonthlyIncomes(DatePaths, Add);
		}

		protected override void SaveToDb()
		{
			try
			{
				var transactionItems = TransactionItems.Cast<IncomeItem>().Select(ii => ((IncomeItem)ii.WithMonthDate()).ToTransactionItem()).ToList();
				var date = new DateTime(DatePaths.Date.Year, DatePaths.Date.Month, 1);

				TransactionItemManager.ReplaceDailyItems(transactionItems, TransactionItemType.Income, date);

				MessagePresenter.WriteLine(Localized.Monthly_incomes_successfully_saved_into_database);
			}
			catch(Exception e)
			{
				ExinLog.ger.LogException(Localized.The_saving_of_the_monthly_incomes_into_database_was_unsuccessful, e);
				throw;
			}
		}

		protected override void SaveSummariesToDb()
		{
			try
			{
				SummaryItemManager.InsertOrUpdateSummary(Summary, DatePaths.Date, TransactionItemType.Income);
				MessagePresenter.WriteLine(Localized.Monthly_income_statistics_successfully_saved_into_database);
			}
			catch(Exception e)
			{
				ExinLog.ger.LogException(Localized.The_saving_of_the_monthly_income_statistics_into_database_was_unsuccessful, e);
				throw;
			}
		}

		protected override void SaveToFile()
		{
			FileRepoManager.WriteOutMonthlyIncomes(TransactionItems, DatePaths, Summary);
		}


		public override void Add(TransactionItemBase item)
		{
			base.Add(item.WithMonthDate());
		}
	}
}
