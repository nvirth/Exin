using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Common;
using Common.Config;
using Common.DbEntities;
using Common.UiModels.WPF;
using Common.Utils;
using Common.Utils.Helpers;
using DAL;
using DAL.DataBase.AdoNet;
using DAL.DataBase.AdoNet.Managers;
using DAL.DataBase.Managers;
using DAL.FileRepo;
using Localization;

namespace ImportDataToDb
{
	/// <summary>
	/// Imports data from the file repository to the database. 
	/// </summary>
	public static class ImportData
	{
		#region Members

		public static readonly List<ExpenseItem> ExpenseItems = new List<ExpenseItem>();
		public static readonly List<IncomeItem> IncomeItems = new List<IncomeItem>();
		public static readonly List<Unit> Units = new List<Unit>();
		public static readonly List<Category> Categories = new List<Category>();

		#endregion

		#region Main

		public static void ImportDataFromFileRepoToDb(string[] args = null)
		{
			DoWork(args);
		}

		private static void DoWork(string[] args)
		{
			Action clearAllTablesAction = ClearAllTables;
			Action importUnitsAction = ImportUnits;
			Action importCategoriesAction = ImportCategories;
			Action importExpensesAction = () => ImportExpensesAndIncomes(args);
			Action calculateSummaries = CalculateSummaries;

			clearAllTablesAction.ExecuteWithTimeMeasuring("ClearAllTables");
			importUnitsAction.ExecuteWithTimeMeasuring("ImportUnits");
			importCategoriesAction.ExecuteWithTimeMeasuring("ImportCategories");
			importExpensesAction.ExecuteWithTimeMeasuring("ImportExpensesAndIncomes");
			calculateSummaries.ExecuteWithTimeMeasuring("CalculateSummaries");
		}

		#endregion

		#region Imports

		private static void ImportUnits()
		{
			//var unit = new Unit();
			//XElement xmlDoc = XElement.Load(Paths.UnitResourcesFullPath);
			//var units =
			//	from xmlUnit in xmlDoc.Elements("Unit")
			//	select new Unit
			//	{
			//		ID = ((int)xmlUnit.Element(unit.Property(u => u.ID))),
			//		Name = ((string)xmlUnit.Element(unit.Property(u => u.Name))).Trim(),
			//		DisplayName = ((string)xmlUnit.Element(unit.Property(u => u.DisplayName))).Trim(),
			//	};

			var units = FileRepoManager.GetUnits();
			var unitManager = UnitManagerAdoNetFactory.Create();

			using(var ctx = ExinAdoNetContextFactory.Create())
			using(new IdentityInsert(ctx, UnitManagerAdoNetBase.TableName))
			{
				foreach(var u in units)
				{
					Units.Add(u);
					unitManager.Add(u, ctx);
				}
			}
		}

		private static void ImportCategories()
		{
			//var category = new Category();
			//XElement xmlDoc = XElement.Load(Paths.CategoryResourcesFullPath);
			//var categories =
			//	from xmlCategory in xmlDoc.Elements("Category")
			//	select new Category
			//	{
			//		ID = ((int)xmlCategory.Element(category.Property(u => u.ID))),
			//		Name = ((string)xmlCategory.Element(category.Property(u => u.Name))).Trim(),
			//		DisplayName = ((string)xmlCategory.Element(category.Property(u => u.DisplayName))).Trim(),
			//	};

			var categories = FileRepoManager.GetCategories();
			var categoryManager = CategoryManagerAdoNetFactory.Create();

			using(var ctx = ExinAdoNetContextFactory.Create())
			using(new IdentityInsert(ctx, CategoryManagerAdoNetBase.TableName))
			{
				foreach(var c in categories)
				{
					Categories.Add(c);
					categoryManager.Add(c, ctx);
				}
			}
		}

		private static void ImportExpensesAndIncomes(string[] args)
		{
			//string year = "";
			//string month = "";
			string day = "";
			string searchPattern = "*";
			Regex directoryFilter = new Regex(".*");

			if(args != null && args.Length != 0)
			{
				//TODO
				//var searchPattern = year;
				//searchPattern += month == null ? "" : "_" + month;
				//searchPattern += "*";

				throw new NotImplementedException(Localized.Command_line_arguments_are_not_implemented_yet);
			}
			else // if there is no cmd line argument, we need all the month directories
			{
				directoryFilter = new Regex(@"^\d{4}_\d{2} .*$");
			}

			var monthDirectories = RepoPaths.DirectoryInfos.Root
				.EnumerateDirectories(searchPattern)
				.Where(di => directoryFilter.IsMatch(di.Name));

			var dayFileFilter = new Regex(@"^\d{2}.*$");
			foreach(var monthDirectory in monthDirectories)
			{
				int actualYear = int.Parse(monthDirectory.Name.Substring(0, 4));
				int actualMonth = int.Parse(monthDirectory.Name.Substring(5, 2));

				var filesInMonthDir = monthDirectory.GetFiles();
				var dayFiles = filesInMonthDir
					.Where(fi =>
					{
						// Filtering to command line "day" argument
						if(!string.IsNullOrEmpty(day))
							return fi.Name.StartsWith(day.PadLeft(2, "0".ToCharArray()[0]));
						else
							return true;
					})
					.Where(fi => fi.Length > 0 && dayFileFilter.IsMatch(fi.Name)); // DayFile and not empty

				DateTime actualDate;
				foreach(var dayFile in dayFiles)
				{
					int actualDay = int.Parse(dayFile.Name.Substring(0, 2));
					actualDate = new DateTime(actualYear, actualMonth, actualDay);

					FileRepoManager.ParseDailyExpenseFile(dayFile.FullName, actualDate, ExpenseItems.Add);
				}

				// -- Parse the MonthlyIncome file

				actualDate = new DateTime(actualYear, actualMonth, 1);
				var monthlyIncomesFileName = Path.GetFileNameWithoutExtension(RepoPaths.Names.MonthlyIncomesSum);
				var monthlyIncomesFile = filesInMonthDir
					.FirstOrDefault(fi => fi.Name.StartsWith(monthlyIncomesFileName));

				if(monthlyIncomesFile != null && monthlyIncomesFile.Length > 0)
					FileRepoManager.ParseMonthlyIncomeFile(monthlyIncomesFile.FullName, actualDate, IncomeItems.Add);
			}

			// -- To DataBase

			Action toDbAction = ImportExpensesAndIncomes_ToDb_OneByOne;
			toDbAction.ExecuteWithTimeMeasuring("  DB");
		}

		private static void ImportExpensesAndIncomes_ToDb_OneByOne()
		{
			// Here we aren't using the TransactionItemManager class
			// and aren't using TransactionScope neither
			// 
			// And this section works with MsSql and SQLite db as well
			//

			using(var ctx = ExinAdoNetContextFactory.Create())
			using(ctx.WithTransaction())
			{
				const string insertSql = @"INSERT INTO [TransactionItem]
								   (
								   [Amount]
								   ,[Quantity]
								   ,[UnitID]
								   ,[Title]
								   ,[Comment]
								   ,[Date]
								   ,[CategoryID]
								   ,[IsExpenseItem]
								   ,[IsIncomeItem])
							 VALUES
								   (
								   @Amount
								   ,@Quantity
								   ,@UnitID
								   ,@Title
								   ,@Comment
								   ,@Date
								   ,@CategoryID
								   ,@IsExpenseItem
								   ,@IsIncomeItem)";

				ctx.Command.CommandText = insertSql;

				foreach(var transactionItem in ExpenseItems.Select(ei => ei.ToTransactionItem()))
					ImportExpensesAndIncomes_ToDb_Insert(ctx.Command, transactionItem);

				foreach(var transactionItem in IncomeItems.Select(ii => ii.ToTransactionItem()))
					ImportExpensesAndIncomes_ToDb_Insert(ctx.Command, transactionItem);

				ctx.Command.Transaction.Commit();
			}
		}

		private static void ImportExpensesAndIncomes_ToDb_Insert(DbCommand command, TransactionItem transactionItem)
		{
			command.Parameters.Clear();

			command.Parameters.AddWithValue("@Amount", transactionItem.Amount);
			command.Parameters.AddWithValue("@Quantity", transactionItem.Quantity);
			command.Parameters.AddWithValue("@Title", transactionItem.Title);
			command.Parameters.AddWithValue("@Comment", transactionItem.Comment);
			command.Parameters.AddWithValue("@Date", transactionItem.Date);

			command.Parameters.AddWithValue("@UnitID", transactionItem.Unit.ID);
			command.Parameters.AddWithValue("@CategoryID", transactionItem.Category.ID);

			command.Parameters.AddWithValue("@IsExpenseItem", transactionItem.IsExpenseItem);
			command.Parameters.AddWithValue("@IsIncomeItem", transactionItem.IsIncomeItem);

			command.ExecuteNonQuery();
		}

		private static void CalculateSummaries()
		{
			var ExpensesAndIncomes = ExpenseItems.GroupBy(ei => ei.Date)
					.Concat<IGrouping<DateTime, TransactionItemBase>>(
						IncomeItems.GroupBy(ii => ii.Date));

			foreach(IGrouping<DateTime, TransactionItemBase> grouping in ExpensesAndIncomes)
			{
				var summary = new Summary();

				foreach(var expenseItem in grouping)
					summary.Update(expenseItem);

				var transactionItemType = summary.SumIn == 0 ? TransactionItemType.Expense : TransactionItemType.Income;
				SummaryItemManager.InsertOrUpdateSummary(summary, grouping.Key, transactionItemType);
			}
		}

		#endregion

		#region Others

		public static void ClearAllTables()
		{
			switch(Config.DbType)
			{
				case DbType.MsSql:
					using(var ctx = ExinAdoNetContextFactory.Create())
					{
						ctx.Command.CommandText = "EXEC sp_MSForEachTable 'DISABLE TRIGGER ALL ON ?'";
						ctx.Command.ExecuteNonQuery();
						ctx.Command.CommandText = "EXEC sp_MSForEachTable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL'";
						ctx.Command.ExecuteNonQuery();
						ctx.Command.CommandText = "EXEC sp_MSForEachTable 'DELETE FROM ?'";
						ctx.Command.ExecuteNonQuery();
						ctx.Command.CommandText = "EXEC sp_MSForEachTable 'ALTER TABLE ? CHECK CONSTRAINT ALL'";
						ctx.Command.ExecuteNonQuery();
						ctx.Command.CommandText = "EXEC sp_MSForEachTable 'ENABLE TRIGGER ALL ON ?'";
						ctx.Command.ExecuteNonQuery();
					}

					break;

				case DbType.SQLite:
					using(var dbFileStream = File.OpenWrite(RepoPaths.SqliteDbFile))
					{
						dbFileStream.SetLength(0);
					}

					var sqliteCreateScript = File.ReadAllText(RepoPaths.SqliteDbCreateFile);

					using(var ctx = ExinAdoNetContextFactory.Create())
					{
						ctx.Command.CommandText = sqliteCreateScript;
						ctx.ExecInTransactionWithCommit();
					}

					break;

				default:
					throw new NotImplementedException(string.Format(Localized.ImportData_ClearAllTables_is_not_implemented_to_this_DbType___0_, Config.DbType));
			}

			MessagePresenter.WriteLine(Localized.The_database_tables_are_ready__have_been_emptied);
			MessagePresenter.WriteLine("");

			//UnitManager.RefreshCache();
			//CategoryManager.RefreshCache();

			// We have to clear all the caches as well with the tables
			//UnitManager.ClearCache();
			//CategoryManager.ClearCache();
		}

		#endregion
	}
}
