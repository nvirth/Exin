using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Common;
using Common.Config;
using Common.DbEntities;
using Common.Log;
using Common.UiModels.WPF;
using Common.Utils;
using Common.Utils.Helpers;
using DAL.DataBase.Managers;
using Localization;
using Config = Common.Config.Config;

namespace DAL.FileRepo
{
	public interface IFileRepoManager
	{
		/// <summary>
		/// !!! The given 'date' parameter's value will be the ExpenseItems Date 
		/// (independently of their real date, what comes from the 'path' param 
		/// [from the structure of directories and the file name]) !!! 
		/// This function is faster, than the overload without the 'string path' parameter, 
		/// but so unsafer
		/// </summary>
		IEnumerable<ExpenseItem> ParseDailyExpenseFile(string path, DateTime date);

		/// <summary>
		/// This function is slower than the overload with the 'string path' parameter, 
		/// but also safer; the ExpenseItems' Date property can not be invalid
		/// </summary>
		IEnumerable<ExpenseItem> ParseDailyExpenseFile(DateTime date);

		/// <summary>
		/// !!! The given 'date' parameter's value will be the ExpenseItems Date 
		/// (independently of their real date, what comes from the 'path' param 
		/// [from the structure of directories and the file name]) !!! 
		/// This function is faster, than the overload without the 'string path' parameter, 
		/// but so unsafer
		/// </summary>
		void ParseDailyExpenseFile(string path, DateTime date, Action<ExpenseItem> toDoWithEach);

		/// <summary>
		/// This function is slower than the overload with the 'string path' parameter, 
		/// but also safer; the ExpenseItems' Date property can not be invalid
		/// </summary>
		void ParseDailyExpenseFile(DateTime date, Action<ExpenseItem> toDoWithEach);

		List<ExpenseItem> GetDailyExpenses(DateTime date);
		List<ExpenseItem> GetDailyExpenses(DatePaths datePaths);
		void GetDailyExpenses(DateTime date, Action<ExpenseItem> toDoWithEach);
		void GetDailyExpenses(DatePaths datePaths, Action<ExpenseItem> toDoWithEach);
		List<ExpenseItem> GetMonthlyExpenses(DateTime date);
		List<ExpenseItem> GetMonthlyExpenses(DatePaths datePaths);
		void GetMonthlyExpenses(DateTime date, Action<ExpenseItem> toDoWithEach);
		void GetMonthlyExpenses(DatePaths datePaths, Action<ExpenseItem> toDoWithEach);
		List<IncomeItem> GetMonthlyIncomes(DateTime date);
		List<IncomeItem> GetMonthlyIncomes(DatePaths datePaths);
		void GetMonthlyIncomes(DatePaths datePaths, Action<IncomeItem> toDoWithEach);
		void ParseMonthlyIncomeFile(string path, DateTime date, Action<IncomeItem> toDoWithEach);
		List<Unit> GetUnits();
		List<Category> GetCategories();
		void WriteOutDailyExpenses(ICollection<TransactionItemBase> transactionItems, DatePaths datePaths, Summary summary);
		void WriteOutMonthlyIncomes(ICollection<TransactionItemBase> transactionItems, DatePaths datePaths, Summary summary);
		void WriteOutMonthlySummaries(Summary summary, DatePaths datePaths, IEnumerable<TransactionItemBase> transactionItems);
		void AddCategory(Category category);
		void AddUnit(Unit unit);
	}

	public class FileRepoManagerCore : IFileRepoManager
	{
		#region READ

		#region ParseDailyExpenseFile

		/// <summary>
		/// !!! The given 'date' parameter's value will be the ExpenseItems Date 
		/// (independently of their real date, what comes from the 'path' param 
		/// [from the structure of directories and the file name]) !!! 
		/// This function is faster, than the overload without the 'string path' parameter, 
		/// but so unsafer
		/// </summary>
		public IEnumerable<ExpenseItem> ParseDailyExpenseFile(string path, DateTime date)
		{
			XElement xmlDoc = XElement.Load(path);
			var items = ParseDailyExpenseFile(xmlDoc, date);
			return items;
		}
		/// <summary>
		/// This function is slower than the overload with the 'string path' parameter, 
		/// but also safer; the ExpenseItems' Date property can not be invalid
		/// </summary>
		public IEnumerable<ExpenseItem> ParseDailyExpenseFile(DateTime date)
		{
			return ParseDailyExpenseFile(new DatePaths(date).DayFilePath, date);
		}

		/// <summary>
		/// !!! The given 'date' parameter's value will be the ExpenseItems Date 
		/// (independently of their real date, what comes from the 'path' param 
		/// [from the structure of directories and the file name]) !!! 
		/// This function is faster, than the overload without the 'string path' parameter, 
		/// but so unsafer
		/// </summary>
		public void ParseDailyExpenseFile(string path, DateTime date, Action<ExpenseItem> toDoWithEach)
		{
			XElement xmlDoc = XElement.Load(path);
			ParseDailyExpenseFile(xmlDoc, date, toDoWithEach);
		}
		/// <summary>
		/// This function is slower than the overload with the 'string path' parameter, 
		/// but also safer; the ExpenseItems' Date property can not be invalid
		/// </summary>
		public void ParseDailyExpenseFile(DateTime date, Action<ExpenseItem> toDoWithEach)
		{
			ParseDailyExpenseFile(new DatePaths(date).DayFilePath, date, toDoWithEach);
		}

		private void ParseDailyExpenseFile(XElement xmlDoc, DateTime date, Action<ExpenseItem> toDoWithEach)
		{
			if(toDoWithEach != null)
			{
				var items = ParseDailyExpenseFile(xmlDoc, date);
				foreach(var expenseItem in items)
					toDoWithEach(expenseItem);
			}
		}
		private IEnumerable<ExpenseItem> ParseDailyExpenseFile(XElement xmlDoc, DateTime date)
		{
			return xmlDoc.Elements("ExpenseItem").Select(xmlEi => ParseDailyExpenseFile_FetchOne(date, xmlEi));
		}
		private ExpenseItem ParseDailyExpenseFile_FetchOne(DateTime date, XElement xmlEi)
		{
			var unitString = (string)xmlEi.Element("Unit");
			var unit = UnitManager.GetByDisplayName(unitString, nullIfNotFound: true)
				?? UnitManager.GetByName(unitString, nullIfNotFound: false);

			var categoryString = (string)xmlEi.Element("Category");
			var category = CategoryManager.GetByDisplayName(categoryString, nullIfNotFound: true)
				?? CategoryManager.GetByName(categoryString, nullIfNotFound: false);

			var expenseItem = new ExpenseItem
			{
				Amount = ((int)xmlEi.Element("Amount")),
				Quantity = ((int)xmlEi.Element("Quantity")),
				Title = ((string)xmlEi.Element("Title")).Trim(),
				Comment = ((string)xmlEi.Element("Comment") ?? "").Trim(),
				Unit = unit,
				Category = category,
				Date = date,
			};
			return expenseItem;
		}

		#endregion

		#region GetDailyExpenses

		public List<ExpenseItem> GetDailyExpenses(DateTime date)
		{
			var list = new List<ExpenseItem>();
			GetDailyExpenses(date, list.Add);

			return list;
		}
		public List<ExpenseItem> GetDailyExpenses(DatePaths datePaths)
		{
			var list = new List<ExpenseItem>();
			GetDailyExpenses(datePaths, list.Add);

			return list;
		}
		public void GetDailyExpenses(DateTime date, Action<ExpenseItem> toDoWithEach)
		{
			GetDailyExpenses(new DatePaths(date), toDoWithEach);
		}
		public void GetDailyExpenses(DatePaths datePaths, Action<ExpenseItem> toDoWithEach)
		{
			Action parserAction =
				() => ParseDailyExpenseFile(datePaths.DayFilePath, datePaths.Date, toDoWithEach);

			GetDailyExpenses_Core_WithMessages(datePaths.DayFileName, datePaths.DayFile.Length, parserAction);
		}
		private void GetDailyExpenses_Core_WithMessages(string fileName, long fileLength, Action parserAction)
		{
			var msg = fileName + " - ";

			if(fileLength == 0)
			{
				MessagePresenter.WriteLine(msg + Localized.Empty_);
				return;
			}

			try
			{
				parserAction();

				MessagePresenter.WriteLine(msg + Localized.Read_successfully);
			}
			catch(Exception e)
			{
				ExinLog.ger.LogException(msg + Localized.Reading_error_, e);
				throw;
			}
		}

		#endregion

		#region GetMonthlyExpenses

		public List<ExpenseItem> GetMonthlyExpenses(DateTime date)
		{
			var list = new List<ExpenseItem>();
			GetMonthlyExpenses(date, list.Add);

			return list;
		}
		public List<ExpenseItem> GetMonthlyExpenses(DatePaths datePaths)
		{
			var list = new List<ExpenseItem>();
			GetMonthlyExpenses(datePaths, list.Add);

			return list;
		}
		public void GetMonthlyExpenses(DateTime date, Action<ExpenseItem> toDoWithEach)
		{
			GetMonthlyExpenses(new DatePaths(date), toDoWithEach);
		}
		public void GetMonthlyExpenses(DatePaths datePaths, Action<ExpenseItem> toDoWithEach)
		{
			Action parserAction =
				() => GetDailyExpenses(datePaths, toDoWithEach);
			//() => ParseDailyExpenseFile(paths.DayFilePath, paths.Date, toDoWithEach);

			GetMonthlyExpenses_Core_WithMessages(datePaths, parserAction);
		}
		private void GetMonthlyExpenses_Core_WithMessages(DatePaths datePaths, Action parserAction)
		{
			try
			{
				for(int i = 1; i <= datePaths.DaysInMonth; i++)
				{
					var actualDay = new DateTime(datePaths.Date.Year, datePaths.Date.Month, i);
					datePaths.Date = actualDay;

					parserAction();
				}
				MessagePresenter.WriteLine(Localized.Daily_files_read_successfully);
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
			var incomeItems = new List<IncomeItem>();
			GetMonthlyIncomes(datePaths, incomeItems.Add);
			return incomeItems;
		}
		public void GetMonthlyIncomes(DatePaths datePaths, Action<IncomeItem> toDoWithEach)
		{
			Action parserAction =
				() => ParseMonthlyIncomeFile(datePaths.MonthlyIncomesFilePath, datePaths.Date, toDoWithEach);

			GetMonthlyIncomes_Core_WithMessages(datePaths.MonthlyIncomesFile, parserAction);
		}
		private void GetMonthlyIncomes_Core_WithMessages(FileInfo fileInfo, Action parserAction)
		{
			if(!fileInfo.Exists || fileInfo.Length == 0)
			{
				MessagePresenter.WriteLine(Localized.There_are_no_incomes_in_this_month);
				return;
			}

			try
			{
				parserAction();

				MessagePresenter.WriteLine(Localized.Monthly_incomes_read_successfully);
			}
			catch(Exception e)
			{
				ExinLog.ger.LogException(Localized.Reading_error_, e);
				throw;
			}
		}
		public void ParseMonthlyIncomeFile(string path, DateTime date, Action<IncomeItem> toDoWithEach)
		{
			var xmlDoc = XElement.Load(path);

			if(toDoWithEach != null)
			{
				var items = ParseMonthlyIncomeFile(xmlDoc, date);
				foreach(var incomeItem in items)
					toDoWithEach(incomeItem);
			}
		}
		private IEnumerable<IncomeItem> ParseMonthlyIncomeFile(XElement xmlDoc, DateTime date)
		{
			return xmlDoc.Elements("IncomeItem").Select(xmlEi => ParseMonthlyIncomeFile_FetchOne(date, xmlEi));
		}
		private IncomeItem ParseMonthlyIncomeFile_FetchOne(DateTime date, XElement xmlEi)
		{
			date = new DateTime(date.Year, date.Month, 1);
			var incomeItem = new IncomeItem
			{
				Title = ((string)xmlEi.Element("Title")).Trim(),
				Amount = ((int)xmlEi.Element("Amount")),
				Comment = ((string)xmlEi.Element("Comment") ?? "").Trim(),
				Date = date,
			};
			return incomeItem;
		}

		#endregion

		#region GetCategories, GetUnits

		public List<Unit> GetUnits()
		{
			var xmlDoc = XElement.Load(RepoPaths.UnitsFile);
			var units = xmlDoc.Elements("Unit").Select(xml => new Unit
			{
				ID = ((int)xml.Element("ID")),
				Name = ((string)xml.Element("Name")).Trim(),
				DisplayName = ((string)xml.Element("DisplayName")).Trim(),
			}).ToList();
			return units;
		}

		public List<Category> GetCategories()
		{
			var xmlDoc = XElement.Load(RepoPaths.CategoriesFile);
			var categories = xmlDoc.Elements("Category").Select(xml => new Category
			{
				ID = ((int)xml.Element("ID")),
				Name = ((string)xml.Element("Name")).Trim(),
				DisplayName = ((string)xml.Element("DisplayName")).Trim(),
			}).ToList();
			return categories;
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
				var sumOut = isExpense ? new XElement("DailySummary", summary.SumOut) : new XElement("IncomeSummary", summary.SumIn);
				var expenseItemsXml = transactionItems.Select(tib => tib.ToXml());

				// -- Building the output string

				stringBuilder.AppendLine("<?xml version=\"1.0\"?>");
				stringBuilder.AppendLine("<root>");

				stringBuilder.AppendLine(cData.ToString());
				stringBuilder.AppendLine(sumOut.ToString());

				foreach(var xElement in expenseItemsXml)
					stringBuilder.AppendLine(xElement.ToString());

				stringBuilder.AppendLine("</root>");
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
				var sumOut = new XElement("MonthlySummary", summary.SumOut);

				var categorySummariesXml =
					from key in CategoryManager.GetAllValid()
					where summary.SumOutWithCategories.ContainsKey(key)
					select new XElement(key.Name, summary.SumOutWithCategories[key]);

				// -- Building the output string

				var stringBuilder = new StringBuilder();

				stringBuilder.AppendLine("<?xml version=\"1.0\"?>");
				stringBuilder.AppendLine("<root>");

				stringBuilder.AppendLine(cData.ToString());
				stringBuilder.AppendLine(sumOut.ToString());

				foreach(var xElement in categorySummariesXml)
					stringBuilder.AppendLine(xElement.ToString());

				stringBuilder.AppendLine("</root>");

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

			foreach(var categoryKey in CategoryManager.GetAllValid())
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
			foreach(var category in CategoryManager.GetAllValid())
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
				//s += "█".Repeat(SummaryItem.SumOut / 5000) + ".xml";

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
			foreach(var expenseCategory in CategoryManager.GetAllValid())
			{
				try
				{
					var newDataRow = SaveToStatistics_CreateNewDataRow(expenseCategory, summary, actualYearAndMonth);
					SaveToStatistics_InsertIntoStatistics(expenseCategory.DisplayName, newDataRow, actualYearAndMonth);

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
			catch(Exception e) // Did not exist statistics file
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

	public static class FileRepoManager
	{
		private static readonly FileRepoManagerCore Manager = new FileRepoManagerCore();

		#region Helpers

		public static void ToOldFormat(this TransactionItemBase tib, StringBuilder stringBuilder)
		{
			if(tib.Quantity != 1 || (tib.Unit != UnitManager.GetUnitDb && tib.Unit != UnitManager.GetUnitNone))
				stringBuilder.Append(tib.Quantity).Append(' ')
					.Append(tib.Unit.DisplayName).Append(' ');

			stringBuilder.Append(tib.Title);

			if(!string.IsNullOrWhiteSpace(tib.Comment))
				stringBuilder.Append(" (").Append(tib.Comment).Append(')');

			stringBuilder.Append(": ").AppendLine(tib.Amount.ToExinStringInFile());

			//stringBuilder.Append(": ")
			//	.Append(tib.Amount.ToString("N0", new CultureInfo("is-IS")))
			//	.AppendLine(",-");
		}

		#endregion

		#region Delegated members

		public static IEnumerable<ExpenseItem> ParseDailyExpenseFile(string path, DateTime date)
		{
			return Manager.ParseDailyExpenseFile(path, date);
		}

		public static IEnumerable<ExpenseItem> ParseDailyExpenseFile(DateTime date)
		{
			return Manager.ParseDailyExpenseFile(date);
		}

		public static void ParseDailyExpenseFile(string path, DateTime date, Action<ExpenseItem> toDoWithEach)
		{
			Manager.ParseDailyExpenseFile(path, date, toDoWithEach);
		}

		public static void ParseDailyExpenseFile(DateTime date, Action<ExpenseItem> toDoWithEach)
		{
			Manager.ParseDailyExpenseFile(date, toDoWithEach);
		}

		public static List<ExpenseItem> GetDailyExpenses(DateTime date)
		{
			return Manager.GetDailyExpenses(date);
		}

		public static List<ExpenseItem> GetDailyExpenses(DatePaths datePaths)
		{
			return Manager.GetDailyExpenses(datePaths);
		}

		public static void GetDailyExpenses(DateTime date, Action<ExpenseItem> toDoWithEach)
		{
			Manager.GetDailyExpenses(date, toDoWithEach);
		}

		public static void GetDailyExpenses(DatePaths datePaths, Action<ExpenseItem> toDoWithEach)
		{
			Manager.GetDailyExpenses(datePaths, toDoWithEach);
		}

		public static List<ExpenseItem> GetMonthlyExpenses(DateTime date)
		{
			return Manager.GetMonthlyExpenses(date);
		}

		public static List<ExpenseItem> GetMonthlyExpenses(DatePaths datePaths)
		{
			return Manager.GetMonthlyExpenses(datePaths);
		}

		public static void GetMonthlyExpenses(DateTime date, Action<ExpenseItem> toDoWithEach)
		{
			Manager.GetMonthlyExpenses(date, toDoWithEach);
		}

		public static void GetMonthlyExpenses(DatePaths datePaths, Action<ExpenseItem> toDoWithEach)
		{
			Manager.GetMonthlyExpenses(datePaths, toDoWithEach);
		}

		public static List<IncomeItem> GetMonthlyIncomes(DateTime date)
		{
			return Manager.GetMonthlyIncomes(date);
		}

		public static List<IncomeItem> GetMonthlyIncomes(DatePaths datePaths)
		{
			return Manager.GetMonthlyIncomes(datePaths);
		}

		public static void GetMonthlyIncomes(DatePaths datePaths, Action<IncomeItem> toDoWithEach)
		{
			Manager.GetMonthlyIncomes(datePaths, toDoWithEach);
		}

		public static void ParseMonthlyIncomeFile(string path, DateTime date, Action<IncomeItem> toDoWithEach)
		{
			Manager.ParseMonthlyIncomeFile(path, date, toDoWithEach);
		}

		public static List<Unit> GetUnits()
		{
			return Manager.GetUnits();
		}

		public static List<Category> GetCategories()
		{
			return Manager.GetCategories();
		}

		public static void WriteOutDailyExpenses(ICollection<TransactionItemBase> transactionItems, DatePaths datePaths, Summary summary)
		{
			Manager.WriteOutDailyExpenses(transactionItems, datePaths, summary);
		}

		public static void WriteOutMonthlyIncomes(ICollection<TransactionItemBase> transactionItems, DatePaths datePaths, Summary summary)
		{
			Manager.WriteOutMonthlyIncomes(transactionItems, datePaths, summary);
		}

		public static void WriteOutMonthlySummaries(Summary summary, DatePaths datePaths, IEnumerable<TransactionItemBase> transactionItems)
		{
			Manager.WriteOutMonthlySummaries(summary, datePaths, transactionItems);
		}

		public static void AddCategory(Category category)
		{
			Manager.AddCategory(category);
		}

		public static void AddUnit(Unit unit)
		{
			Manager.AddUnit(unit);
		}

		#endregion
	}
}
