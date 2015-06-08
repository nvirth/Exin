using System;
using System.Collections.Generic;
using System.Globalization;
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
using DAL.DataBase.Managers;
using Localization;
using Config = Common.Configuration.Config;
using C = Common.Configuration.Constants.XmlTags;

namespace DAL.FileRepo
{
	public interface IFileRepoManager
	{
		IEnumerable<ExpenseItem> ParseDailyExpenseFile(DateTime date, string cachedPath = null);
		List<ExpenseItem> GetDailyExpenses(DateTime date);
		List<ExpenseItem> GetDailyExpenses(DatePaths datePaths);

		List<ExpenseItem> GetMonthlyExpenses(DateTime date);
		List<ExpenseItem> GetMonthlyExpenses(DatePaths datePaths);

		List<IncomeItem> GetMonthlyIncomes(DateTime date);
		List<IncomeItem> GetMonthlyIncomes(DatePaths datePaths);

		List<Unit> GetUnits();
		void AddUnit(Unit unit);

		List<Category> GetCategories();
		void AddCategory(Category category);

		void WriteOutDailyExpenses(ICollection<TransactionItemBase> transactionItems, DatePaths datePaths, Summary summary);
		void WriteOutMonthlyIncomes(ICollection<TransactionItemBase> transactionItems, DatePaths datePaths, Summary summary);
		void WriteOutMonthlySummaries(Summary summary, DatePaths datePaths, IEnumerable<TransactionItemBase> transactionItems);
	}

	public class FileRepoManager : IFileRepoManager
	{
		public static readonly FileRepoManager Instance = new FileRepoManager();

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
				MessagePresenter.WriteLine(msg + Localized.Empty_);
				return new List<ExpenseItem>();
			}

			try
			{
				var dailyExpenses = ParseDailyExpenseFile(datePaths.Date, datePaths.DayFilePath).ToList();
				MessagePresenter.WriteLine(msg + Localized.Read_successfully);

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
				MessagePresenter.WriteLine(Localized.Daily_files_read_successfully);

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
				MessagePresenter.WriteLine(Localized.There_are_no_incomes_in_this_month);
				return new List<IncomeItem>();
			}

			try
			{
				var monthlyIncomes = ParseMonthlyIncomeFile(datePaths.Date, datePaths.MonthlyIncomesFilePath).ToList();
				MessagePresenter.WriteLine(Localized.Monthly_incomes_read_successfully);

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

		#region GetCategories, GetUnits

		public List<Unit> GetUnits()
		{
			var xmlDoc = XElement.Load(RepoPaths.UnitsFile);
			var units = xmlDoc.Elements(C.Unit).Select(xml => new Unit
			{
				ID = ((int) xml.Element(C.ID)),
				Name = ((string) xml.Element(C.Name)).Trim(),
				DisplayNames = ParseLocalizedDisplayNames(xml),
			}).ToList();
			return units;
		}

		public List<Category> GetCategories()
		{
			var xmlDoc = XElement.Load(RepoPaths.CategoriesFile);
			var categories = xmlDoc.Elements(C.Category).Select(xml => new Category
			{
				ID = ((int)xml.Element(C.ID)),
				Name = ((string)xml.Element(C.Name)).Trim(),
				DisplayNames = ParseLocalizedDisplayNames(xml),
			}).ToList();
			return categories;
		}

		private string ParseLocalizedDisplayNames(XElement xml)
		{
			var displayNames =
				xml.Element(C.DisplayNames)
					.Descendants()
					.Select(displayNameXml => "{0}:{1};".Formatted(
						displayNameXml.Name.LocalName, ((string)displayNameXml).Trim()
					))
					.Join("");

			return displayNames;
		}

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

				MessagePresenter.WriteLine(msg + Localized.___Successfully_saved_into_files);
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

				MessagePresenter.WriteLine(msg + Localized.___Successfully_saved_into_files);
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
					from key in CategoryManager.Instance.GetAllValid()
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

				MessagePresenter.WriteLine(Localized.Saving_Monthly_summary_file___Successfully);
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

			foreach(var categoryKey in CategoryManager.Instance.GetAllValid())
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
			foreach(var category in CategoryManager.Instance.GetAllValid())
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

				MessagePresenter.WriteLine(Localized.Copying_summary_file_into_the_summary_directory___OK);
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
			foreach(var expenseCategory in CategoryManager.Instance.GetAllValid())
			{
				try
				{
					var newDataRow = SaveToStatistics_CreateNewDataRow(expenseCategory, summary, actualYearAndMonth);
					SaveToStatistics_InsertIntoStatistics(expenseCategory.Name, newDataRow, actualYearAndMonth);

					MessagePresenter.WriteLine(string.Format(Localized.Saving_0__statistics___OK__FORMAT__, expenseCategory.DisplayName));
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

				if(expenseCategory == CategoryManager.GetCategoryOthers)
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
					statisticsData[dataIndex] = newDataRow;			// If the month already exists, modify it
				else
					statisticsData.Insert(~dataIndex, newDataRow);	// If not, insert it
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

		#region AddCategory, AddUnit

		public void AddCategory(Category category)
		{
			var xmlDoc = XElement.Load(RepoPaths.CategoriesFile);
			xmlDoc.Add(category.ToXml());
			xmlDoc.Save(RepoPaths.CategoriesFile);
		}

		public void AddUnit(Unit unit)
		{
			var xmlDoc = XElement.Load(RepoPaths.UnitsFile);
			xmlDoc.Add(unit.ToXml());
			xmlDoc.Save(RepoPaths.UnitsFile);
		}

		#endregion

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
