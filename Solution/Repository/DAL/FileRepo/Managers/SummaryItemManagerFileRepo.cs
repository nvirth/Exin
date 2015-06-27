using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Common;
using Common.Configuration;
using Common.Db.Entities;
using Common.Log;
using Common.UiModels.WPF;
using Common.Utils;
using Common.Utils.Helpers;
using DAL.FileRepo.Managers.Base;
using DAL.RepoCommon.Interfaces;
using Localization;
using C = Common.Configuration.Constants.Xml.Tags;

namespace DAL.FileRepo.Managers
{
	public class SummaryItemManagerFileRepo : FileRepoManagerBase, ISummaryItemManagerDao
	{
		private readonly ITransactionItemManager _transactionItemManager;
		private readonly ICategoryManager _categoryManager;

		public SummaryItemManagerFileRepo(ITransactionItemManager transactionItemManager, ICategoryManager categoryManager)
		{
			_transactionItemManager = transactionItemManager;
			_categoryManager = categoryManager;
		}

		public void ReplaceSummary(Summary summary, DateTime date, TransactionItemType transactionItemType)
		{
			// Monthly incomes already saved with the file (FileRepo works a bit other way then DB)
			if(transactionItemType != TransactionItemType.Expense)
				return;

			// In the db, this is works as a replace-a-daily-item fn
			// The DB works with daily summary items, so when a daily item changes, we change
			// a daily one in the DB. In other words, the DB does not store monthly summaries.
			// But the FileRepo does, so when a daily item changes, we have to refresh the monthly 
			// summaries in the FileRepo
			//
			var monthlyExpenses = _transactionItemManager.GetMonthlyExpenses(date);

			// We mustn't use the summary argument, we have to recalculate it for our monthly summary
			summary = Summary.Summarize(monthlyExpenses);

			WriteOutMonthlySummaries(summary, new DatePaths(date), monthlyExpenses);
		}

		public List<SummaryItem> GetInterval(DateTime fromDate, DateTime toDate)
		{
            MessagePresenter.Instance.WriteError(
				Localized.Reading_statistics_from_File_Repository_is_not_implemented___ETC_MSG +
				RepoPaths.SummariesDir);

			return new List<SummaryItem>();
		}
	
		#region WriteOutMonthlySummaries

		public void WriteOutMonthlySummaries(Summary summary, DatePaths datePaths, IEnumerable<TransactionItemBase> transactionItems)
		{
			string actualYearAndMonth;

			SaveToFile(summary, datePaths, transactionItems);
			CopySummaryFile(out actualYearAndMonth, datePaths, summary);
			SaveToStatistics(summary, actualYearAndMonth);
		}
		private void SaveToFile(Summary summary, DatePaths datePaths, IEnumerable<TransactionItemBase> transactionItems)
		{
			// fixme - stringBuilder section is duplicated (daily...) + what is below that's also!

			try
			{
				// -- Data to write out

				var cData = new XCData(CreateOldSummaryCData(summary, transactionItems));
				var sumOut = new XElement(C.MonthlySummary, summary.SumOut);

				var categorySummariesXml =
					from key in _categoryManager.GetAllValid()
					where summary.SumOutWithCategories.ContainsKey(key)
					select new XElement(key.Name, summary.SumOutWithCategories[key]);

				// -- Building the output string

				var stringBuilder = new StringBuilder();

				stringBuilder.AppendLine("<?xml version=\"1.0\"?>");
				stringBuilder.Append("<").Append(C.root).AppendLine(">");

				stringBuilder.AppendLine(cData.ToString());
				stringBuilder.AppendLine(sumOut.ToString());

				foreach(var xElement in categorySummariesXml)
					stringBuilder.AppendLine(xElement.ToString());

				stringBuilder.Append("</").Append(C.root).AppendLine(">");

				// -- Writing out the output string

				FileInfo oldFile;
				FileInfo newFile;
				datePaths.CalculateMonthlyFileNames(summary.SumOut, out oldFile, out newFile);

				Helpers.CreateNewFileDeleteOld(newFile, oldFile, stringBuilder);

				MessagePresenter.Instance.WriteLine(Localized.Saving_Monthly_summary_file___Successfully);
			}
			catch(Exception e)
			{
				ExinLog.ger.LogException(Localized.Saving_Monthly_summary_file___Error_, e);
				throw;
			}
		}
		private string CreateOldSummaryCData(Summary summary, IEnumerable<TransactionItemBase> transactionItems)
		{
			const int padRight = 23;
			const int padLeft = 9;
			var stringBuilder = new StringBuilder();

			stringBuilder.AppendLine().AppendLine();
			stringBuilder.Append(Localized.Sum_.PadRight(padRight));
			//stringBuilder.AppendLine(SummaryItem.SumOut.ToString("N0", new CultureInfo("is-IS")).PadLeft(padLeft));
			stringBuilder.Append(summary.SumOut.ToExinStringInFile().PadLeft(padLeft));
			stringBuilder.Append(' ').AppendRepeat("█", summary.SumOut / 5000);
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("------------------------------------------------------");

			foreach(var categoryKey in _categoryManager.GetAllValid())
			{
				if(summary.SumOutWithCategories.ContainsKey(categoryKey))
				{
					var actualSum = summary.SumOutWithCategories[categoryKey];

					stringBuilder.Append((categoryKey.DisplayName + ':').PadRight(padRight));
					stringBuilder.Append(actualSum.ToExinStringInFile().PadLeft(padLeft));
					stringBuilder.Append(' ').AppendRepeat("█", actualSum / 5000);
					stringBuilder.AppendLine();
				}
				else
				{
					stringBuilder.AppendLine();
				}
			}

			var groupedExpenses = transactionItems.Cast<ExpenseItem>().GroupBy(ei => ei.Category).ToList();
			foreach(var category in _categoryManager.GetAllValid())
			{
				var localizedCategory = category;
				foreach(var groupedExpense in groupedExpenses.Where(group => group.Key == localizedCategory))
				{
					stringBuilder.Append("------------------------------------------------------");
					stringBuilder.Append(' ').AppendLine(category.DisplayName);

					foreach(var expenseItem in groupedExpense)
						expenseItem.ToOldFormat(stringBuilder);
				}
			}

			stringBuilder.AppendLine();
			return stringBuilder.ToString();
		}
		private void CopySummaryFile(out string actualYearAndMonth, DatePaths datePaths, Summary summary)
		{
			string actualYearAndMonthLocal = null;

			try
			{
				var stringBuilder = new StringBuilder();
				stringBuilder.Append(datePaths.Date.Year);
				stringBuilder.Append(" - ");
				stringBuilder.Append(datePaths.Date.Month.ToString().PadLeft(2, '0'));

				actualYearAndMonthLocal = stringBuilder.ToString(); // oldFileStart

				stringBuilder.Append(" - ");
				stringBuilder.Append(summary.SumOut.ToExinStringMonthlyFileName());
				stringBuilder.Append(" - ");
				stringBuilder.AppendRepeat("█", summary.SumOut / 5000);
				stringBuilder.Append('.');
				stringBuilder.Append(Config.FileExtension); // "xml"

				var newFileName = stringBuilder.ToString();

				//var s = Paths.Date.Year + " - " + Paths.Date.Month.ToString().PadLeft(2, '0') + " - ";
				//s += SummaryItem.SumOut.ToExinStringMonthlyFileName() + " - ";
				//s += "█".Repeat(SummaryItem.SumOut / 5000) + "." + Config.FileExtension;

				var oldFile = RepoPaths.DirectoryInfos.MonthlySummaries.EnumerateFiles()
					.FirstOrDefault(file => file.Name.StartsWith(actualYearAndMonthLocal));
				var destinationFilePath = Path.Combine(RepoPaths.MonthlySummariesDir, newFileName);

				Helpers.CreateNewFileDeleteOld(destinationFilePath, oldFile, datePaths.MonthlyExpensesFile);

				MessagePresenter.Instance.WriteLine(Localized.Copying_summary_file_into_the_summary_directory___OK);
			}
			catch(Exception e)
			{
				ExinLog.ger.LogException(Localized.Copying_summary_file_into_the_summary_directory___Error_, e);
				throw;
			}
			actualYearAndMonth = actualYearAndMonthLocal;
		}
		private void SaveToStatistics(Summary summary, string actualYearAndMonth)
		{
			foreach(var expenseCategory in _categoryManager.GetAllValid())
			{
				try
				{
					var newDataRow = SaveToStatistics_CreateNewDataRow(expenseCategory, summary, actualYearAndMonth);
					SaveToStatistics_InsertIntoStatistics(expenseCategory.Name, newDataRow, actualYearAndMonth);

					MessagePresenter.Instance.WriteLine(string.Format(Localized.Saving_0__statistics___OK__FORMAT__, expenseCategory.DisplayName));
				}
				catch(Exception e)
				{
					ExinLog.ger.LogException(string.Format(Localized.Saving_0__statistics___Error__FORMAT__, expenseCategory.DisplayName), e);
					throw;
				}
			}
		}
		private string SaveToStatistics_CreateNewDataRow(Category expenseCategory, Summary summary, string actualYearAndMonth)
		{
			// There were expenses in this month in the specified category
			if(summary.SumOutWithCategories.ContainsKey(expenseCategory))
			{
				int sumOutInCategory = summary.SumOutWithCategories[expenseCategory];

				var stringBuilder = new StringBuilder();
				stringBuilder.Append(actualYearAndMonth);
				stringBuilder.Append(" - ");
				stringBuilder.Append(sumOutInCategory.ToExinStringInStatistics());
				stringBuilder.Append(" ");

				if(expenseCategory == _categoryManager.GetCategoryOthers)
					stringBuilder.AppendRepeat("▒", sumOutInCategory / 3000);
				else
					stringBuilder.AppendRepeat("█", sumOutInCategory / 1000);

				return stringBuilder.ToString();
			}
			else // There wasn't any expense in this month
			{
				return actualYearAndMonth + " -     000,- ";
			}
		}
		private void SaveToStatistics_InsertIntoStatistics(string expenseCategory, string newDataRow, string actualYearAndMonth)
		{
			string statisticsFilePath;
			List<String> statisticsData;

			try
			{
				var statisticsFile = RepoPaths.DirectoryInfos.CategorisedSummaries.EnumerateFiles()
					.First(file => file.Name.Contains(expenseCategory));

				statisticsFilePath = statisticsFile.FullName;
				statisticsData = Helpers.ReadInFileList(statisticsFilePath);
				var dataIndex = statisticsData.BinarySearch(actualYearAndMonth, new StaticsDataComparer());

				if(dataIndex >= 0)
					statisticsData[dataIndex] = newDataRow;         // If the month already exists, modify it
				else
					statisticsData.Insert(~dataIndex, newDataRow);  // If not, insert it
			}
			catch(Exception) // Did not exist statistics file
			{
				var lastFile = RepoPaths.DirectoryInfos.CategorisedSummaries.EnumerateFiles().LastOrDefault();

				string start;
				if(lastFile == null)
				{
					start = "01";
				}
				else
				{
					var i = int.Parse(lastFile.Name.Substring(0, 2));
					i++;
					start = i.ToString().PadLeft(2, '0');
				}

				var statisticsFileName = start + " - " + expenseCategory;
				statisticsFilePath = Path.Combine(RepoPaths.CategorisedSummariesDir, statisticsFileName);
				statisticsData = new List<string> { newDataRow };
			}

			// Write to file
			using(var streamWriter = new StreamWriter(statisticsFilePath))
			{
				foreach(var dataRow in statisticsData)
				{
					streamWriter.WriteLine(dataRow);
				}
			}
		}

		#endregion

		#region Helpers

		class StaticsDataComparer : IComparer<string>
		{
			public int Compare(string x, string y)
			{
				if(x.Contains(y) || y.Contains(x))
					return 0;
				else
					return string.Compare(x, y, StringComparison.InvariantCulture);
				//return x.CompareTo(y);
			}
		}

		#endregion
	}
}
