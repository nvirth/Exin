using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Common;
using Common.Log;
using Common.UiModels.WPF;
using Common.Utils;
using Common.Utils.Helpers;
using DAL.DataBase.Managers;
using DAL.FileRepo.Base;
using Localization;
using C = Common.Configuration.Constants.XmlTags;

namespace DAL.FileRepo
{
	public class TransactionItemManagerFileRepo : FileRepoManagerBase, ITransactionItemManagerDao
	{
		#region READ

		#region GetDailyExpenses

		public List<ExpenseItem> GetDailyExpenses(DateTime date)
		{
			return GetDailyExpenses(new DatePaths(date));
		}
		public List<ExpenseItem> GetDailyExpenses(DatePaths datePaths)
		{
			var msg = datePaths.DayFileName + " - ";

			if(datePaths.DayFile.Length == 0)
			{
				MessagePresenter.Instance.WriteLine(msg + Localized.Empty_);
				return new List<ExpenseItem>();
			}

			try
			{
				var dailyExpenses = ParseDailyExpenseFile(datePaths.Date, datePaths.DayFilePath).ToList();
				MessagePresenter.Instance.WriteLine(msg + Localized.Read_successfully);

				return dailyExpenses;
			}
			catch(Exception e)
			{
				ExinLog.ger.LogException(msg + Localized.Reading_error_, e);
				throw;
			}
		}

		#region ParseDailyExpenseFile

		/// <param name="date">The DailyExpenseFile's date, which uniquely identificates it. </param>
		/// <param name="cachedPath">
		///		The path to the file. Only provide if you have it calculated already.
		///		Default value: new DatePaths(date).DayFilePath
		/// </param>
		public IEnumerable<ExpenseItem> ParseDailyExpenseFile(DateTime date, string cachedPath = null)
		{
			cachedPath = cachedPath ?? new DatePaths(date).DayFilePath;

			XElement xmlDoc = XElement.Load(cachedPath);
			var items = xmlDoc.Elements(C.ExpenseItem).Select(xmlEi => ExpenseItem.FromXml(date, xmlEi));
			return items;
		}

		#endregion

		#endregion

		#region GetMonthlyExpenses

		public List<ExpenseItem> GetMonthlyExpenses(DateTime date)
		{
			return GetMonthlyExpenses(new DatePaths(date));
		}
		public List<ExpenseItem> GetMonthlyExpenses(DatePaths datePaths)
		{
			try
			{
				var monthlyExpenses = Enumerable.Empty<ExpenseItem>();

				for(int i = 1; i <= datePaths.DaysInMonth; i++)
				{
					var actualDay = new DateTime(datePaths.Date.Year, datePaths.Date.Month, i);
					datePaths.Date = actualDay;

					//TODO have we every time logged for the user?
					monthlyExpenses = monthlyExpenses.Union(GetDailyExpenses(datePaths));
				}
				var res = monthlyExpenses.ToList();
				MessagePresenter.Instance.WriteLine(Localized.Daily_files_read_successfully);

				return res;
			}
			catch(Exception e)
			{
				ExinLog.ger.LogException(Localized.There_was_error_while_reading_the_daily_files_, e);
				throw;
			}
		}

		#endregion

		#region GetMonthlyIncomes

		public List<IncomeItem> GetMonthlyIncomes(DateTime date)
		{
			var dateSafe = new DateTime(date.Year, date.Month, 1);
			return GetMonthlyIncomes(new DatePaths(dateSafe));
		}
		public List<IncomeItem> GetMonthlyIncomes(DatePaths datePaths)
		{
			if(!datePaths.MonthlyIncomesFile.Exists || datePaths.MonthlyIncomesFile.Length == 0)
			{
				MessagePresenter.Instance.WriteLine(Localized.There_are_no_incomes_in_this_month);
				return new List<IncomeItem>();
			}

			try
			{
				var monthlyIncomes = ParseMonthlyIncomeFile(datePaths.Date, datePaths.MonthlyIncomesFilePath).ToList();
				MessagePresenter.Instance.WriteLine(Localized.Monthly_incomes_read_successfully);

				return monthlyIncomes;
			}
			catch(Exception e)
			{
				ExinLog.ger.LogException(Localized.Reading_error_, e);
				throw;
			}
		}

		#region ParseMonthlyIncomeFile

		/// <param name="date">The MonthlyIncomeFile's date, which uniquely identificates it. </param>
		/// <param name="cachedPath">
		///		The path to the file. Only provide if you have it calculated already.
		///		Default value: new DatePaths(date).MonthlyIncomesFilePath
		/// </param>
		public IEnumerable<IncomeItem> ParseMonthlyIncomeFile(DateTime date, string cachedPath = null)
		{
			cachedPath = cachedPath ?? new DatePaths(date).MonthlyIncomesFilePath;

			var xmlDoc = XElement.Load(cachedPath);
			var items = xmlDoc.Elements(C.IncomeItem).Select(xmlEi => IncomeItem.FromXml(date, xmlEi));
			return items;
		}

		#endregion

		#endregion

		#endregion

		#region WRITE

		#region WriteOutDailyExpenses, WriteOutMonthlyIncomes

		public void WriteOutDailyExpenses(ICollection<TransactionItemBase> transactionItems, DatePaths datePaths, Summary summary)
		{
			var msg = datePaths.Date.ToShortDateString() + Localized._daily_expenses;

			try
			{
				WriteOut_DailyExpenses_or_MonthlyIncomes(transactionItems, datePaths, summary, isExpense: true);

				MessagePresenter.Instance.WriteLine(msg + Localized.___Successfully_saved_into_files);
			}
			catch(Exception e)
			{
				ExinLog.ger.LogException(msg + Localized.___Save_error_, e);
				throw;
			}
		}
		public void WriteOutMonthlyIncomes(ICollection<TransactionItemBase> transactionItems, DatePaths datePaths, Summary summary)
		{
			var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(datePaths.Date.Month);
			var msg = datePaths.Date.ToString(Localized.DateFormat_year_) + monthName + Localized._monthly_incomes;

			try
			{
				WriteOut_DailyExpenses_or_MonthlyIncomes(transactionItems, datePaths, summary, isExpense: false);

				MessagePresenter.Instance.WriteLine(msg + Localized.___Successfully_saved_into_files);
			}
			catch(Exception e)
			{
				ExinLog.ger.LogException(msg + Localized.___Save_error_, e);
				throw;
			}
		}

		private void WriteOut_DailyExpenses_or_MonthlyIncomes(ICollection<TransactionItemBase> transactionItems, DatePaths datePaths, Summary summary, bool isExpense)
		{
			var stringBuilder = new StringBuilder();

			if(transactionItems.Any())
			{
				// -- Data to write out

				var cData = new XCData(CreateOldSummaryCData(transactionItems, summary, isExpense));
				var sumOut = isExpense ? new XElement(C.DailySummary, summary.SumOut) : new XElement(C.IncomeSummary, summary.SumIn);
				var expenseItemsXml = transactionItems.Select(tib => tib.ToXml());

				// -- Building the output string

				stringBuilder.AppendLine("<?xml version=\"1.0\"?>");
				stringBuilder.Append("<").Append(C.root).AppendLine(">");

				stringBuilder.AppendLine(cData.ToString());
				stringBuilder.AppendLine(sumOut.ToString());

				foreach(var xElement in expenseItemsXml)
					stringBuilder.AppendLine(xElement.ToString());

				stringBuilder.Append("</").Append(C.root).AppendLine(">");
			}

			// -- Writing out the output string

			FileInfo oldFile;
			FileInfo newFile;
			if(isExpense)
				datePaths.CalculateDailyFileNames(summary.SumOut, out oldFile, out newFile);
			else // isIncome
				datePaths.CalculateMonthlyIncomesFileNames(summary.SumIn, out oldFile, out newFile);

			Helpers.CreateNewFileDeleteOld(newFile, oldFile, stringBuilder);
		}
		private string CreateOldSummaryCData(IEnumerable<TransactionItemBase> transactionItems, Summary summary, bool isExpense)
		{
			var stringBuilder = new StringBuilder();
			//var sumOutString = SummaryItem.SumOut.ToString("N0", new CultureInfo("is-IS"));
			var sumAmountString = isExpense ? summary.SumOut.ToExinStringInFile() : summary.SumIn.ToExinStringInFile();

			stringBuilder.AppendLine().AppendLine();
			stringBuilder.AppendLine(sumAmountString);
			stringBuilder.AppendLine("---");
			foreach(var expenseItem in transactionItems)
			{
				expenseItem.ToOldFormat(stringBuilder);
			}
			stringBuilder.AppendLine();

			return stringBuilder.ToString();
		}

		#endregion

		#endregion

		#region ITransactionItemManagerDao (remaining) implementation (wrappers)

		// TODO test summary in TransactionItemManagerFileRepo ReplaceDailyExpenses
		public void ReplaceDailyExpenses(IList<ExpenseItem> expenseItems, DateTime date)
		{
			var summary = Summary.Summarize(expenseItems);
			var transactionItems = expenseItems.Cast<TransactionItemBase>().ToList();

			WriteOutDailyExpenses(transactionItems, new DatePaths(date), summary);
		}

		public void ReplaceMonthlyIncomes(IList<IncomeItem> incomeItems, DateTime date)
		{
			date = new DateTime(date.Year, date.Month, 1);

			var summary = Summary.Summarize(incomeItems);
			var transactionItems = incomeItems
				.Select(ii => ((IncomeItem)ii.WithMonthDate()))
				.Cast<TransactionItemBase>().ToList();

			WriteOutMonthlyIncomes(transactionItems, new DatePaths(date), summary);
		}

		#endregion
	}
}
