using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Common;
using Common.Configuration;
using Common.Db.Entities;
using Common.UiModels.WPF;
using Common.Utils;
using Common.Utils.Helpers;
using DAL.DataBase.AdoNet;
using DAL.DataBase.AdoNet.Managers;
using DAL.FileRepo.Managers;
using DAL.RepoCommon;
using DAL.RepoCommon.Managers;
using Localization;

namespace ImportDataToDb
{
	/// <summary>
	/// Imports data from the file repository to the database. 
	/// </summary>
	public class ImportData
	{
		#region Members

		public readonly List<ExpenseItem> ExpenseItems = new List<ExpenseItem>();
		public readonly List<IncomeItem> IncomeItems = new List<IncomeItem>();
		public readonly List<Unit> Units = new List<Unit>();
		public readonly List<Category> Categories = new List<Category>();

		// --

		public readonly IRepoConfiguration LocalConfig;

		public readonly UnitManagerAdoNetBase UnitManagerDb;
		public readonly UnitManagerFileRepo UnitManagerFr;

		public readonly CategoryManagerAdoNetBase CategoryManagerDb;
		public readonly CategoryManagerFileRepo CategoryManagerFr;

		public readonly SummaryItemManagerAdoNetBase SummaryItemManagerDb;
		public readonly TransactionItemManagerFileRepo TransactionItemManagerFr;

		/// <summary>
		/// NOTE: DO NOT USE IT FOR write PURPOSES! At least until the class uses everything
		/// directly, and SaveMode.OnlyToDb is not implemented!
		/// Other way, it will save also into file system, which is not intended when
		/// we intend to import the data from there into the database...
		/// </summary>
		public readonly CategoryManager CategoryManagerLocal;

		/// <summary>
		/// NOTE: DO NOT USE IT FOR write PURPOSES! At least until the class uses everything
		/// directly, and SaveMode.OnlyToDb is not implemented!
		/// Other way, it will save also into file system, which is not intended when
		/// we intend to import the data from there into the database...
		/// </summary>
		public readonly UnitManager UnitManagerLocal;
		
		#endregion

		public ImportData(DbType dbType)
		{
			LocalConfig = new RepoConfiguration()
			{
				// TODO check if it is actual yet
				// FromDb, because caches have to be empty. The reading from FileRepo will be done manually
				ReadMode = ReadMode.FromDb,

				SaveMode = SaveMode.FileAndDb, // TODO only to db (it's ignored yet though because the direct use of the managers)

				// AdoNet, because the db access here is written directly with the AdoNet classes!
				DbAccessMode = DbAccessMode.AdoNet,

				DbType = dbType,
			};

			UnitManagerDb = UnitManagerAdoNetFactory.Create(LocalConfig);
			CategoryManagerDb = CategoryManagerAdoNetFactory.Create(LocalConfig);

			CategoryManagerLocal = new CategoryManager(LocalConfig);
			UnitManagerLocal = new UnitManager(LocalConfig);

			SummaryItemManagerDb = SummaryItemManagerAdoNetFactory.Create(LocalConfig, CategoryManagerLocal);
			TransactionItemManagerFr = new TransactionItemManagerFileRepo();

			UnitManagerFr = new UnitManagerFileRepo();
			CategoryManagerFr = new CategoryManagerFileRepo();

			StaticInitializer.InitAllStatic(CategoryManagerLocal, UnitManagerLocal);
		}

		#region Main

		public void ImportDataFromFileRepoToDb(string[] args = null)
		{


			MessagePresenter.Instance.WriteLine(Localized.Importing_data_from_File_Repository_into___0___DataBase.Formatted(LocalConfig.DbType));
			MessagePresenter.Instance.WriteLine("");

			DoWork(args);

			MessagePresenter.Instance.WriteLine(Localized._end_);
			MessagePresenter.Instance.WriteLine("");
			MessagePresenter.Instance.WriteLine(Localized.Operation_finished_successfully__);
		}

		private void DoWork(string[] args)
		{
			Action clearAllTablesAction = ClearAllTables;
			Action importUnitsAction = ImportUnits;
			Action importCategoriesAction = ImportCategories;
			Action importExpensesAction = () => ImportExpensesAndIncomes(args);
			Action calculateSummaries = CalculateSummaries;

			clearAllTablesAction.ExecuteWithTimeMeasuring(Localized.Creating_or_emptying_tables);
			importUnitsAction.ExecuteWithTimeMeasuring(Localized.Importing_units);
			importCategoriesAction.ExecuteWithTimeMeasuring(Localized.Importing_categories);
			importExpensesAction.ExecuteWithTimeMeasuring(Localized.Importing_expenses_and_incomes);
			calculateSummaries.ExecuteWithTimeMeasuring(Localized.Calculating_summaries);
		}

		#endregion

		#region Imports

		private void ImportUnits()
		{
			var units = UnitManagerFr.GetAll();

			using(var ctx = ExinAdoNetContextFactory.Create(LocalConfig))
			using(new IdentityInsert(ctx, UnitManagerAdoNetBase.TableName))
			{
				foreach(var u in units)
				{
					Units.Add(u);
					UnitManagerDb.Add(u, ctx);
				}
			}
		}

		private void ImportCategories()
		{
			var categories = CategoryManagerFr.GetAll();

			using(var ctx = ExinAdoNetContextFactory.Create(LocalConfig))
			using(new IdentityInsert(ctx, CategoryManagerAdoNetBase.TableName))
			{
				foreach(var c in categories)
				{
					Categories.Add(c);
					CategoryManagerDb.Add(c, ctx);
				}
			}
		}

		private void ImportExpensesAndIncomes(string[] args)
		{
			//string year = "";
			//string month = "";
			string day = "";
			string searchPattern = "*";
			Regex directoryFilter = new Regex(".*");

			if(args != null && args.Length != 0)
			{
				//TODO ImportData command line arguments
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
					.Where(fi => {
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

					ExpenseItems.AddRange(TransactionItemManagerFr.ParseDailyExpenseFile(actualDate, dayFile.FullName));
				}

				// -- Parse the MonthlyIncome file

				actualDate = new DateTime(actualYear, actualMonth, 1);
				var monthlyIncomesFileName = Path.GetFileNameWithoutExtension(RepoPaths.Names.MonthlyIncomesSum);
				var monthlyIncomesFile = filesInMonthDir
					.FirstOrDefault(fi => fi.Name.StartsWith(monthlyIncomesFileName));

				if(monthlyIncomesFile != null && monthlyIncomesFile.Length > 0)
					IncomeItems.AddRange(TransactionItemManagerFr.ParseMonthlyIncomeFile(actualDate, monthlyIncomesFile.FullName));
			}

			// -- To DataBase

			Action toDbAction = ImportExpensesAndIncomes_ToDb_OneByOne;
			toDbAction.ExecuteWithTimeMeasuring(Localized._from_this__database_);
		}

		private void ImportExpensesAndIncomes_ToDb_OneByOne()
		{
			// Here we aren't using the TransactionItemManager class
			// and aren't using TransactionScope neither
			// 
			// And this section works with MsSql and SQLite db as well
			//

			using(var ctx = ExinAdoNetContextFactory.Create(LocalConfig))
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

				foreach(var transactionItem in IncomeItems.Select(ii => ii.ToTransactionItem(CategoryManagerLocal)))
					ImportExpensesAndIncomes_ToDb_Insert(ctx.Command, transactionItem);

				ctx.Command.Transaction.Commit();
			}
		}

		private void ImportExpensesAndIncomes_ToDb_Insert(DbCommand command, TransactionItem transactionItem)
		{
			command.Parameters.Clear();

			command.Parameters.AddWithValue("@Amount", transactionItem.Amount, LocalConfig.DbType);
			command.Parameters.AddWithValue("@Quantity", transactionItem.Quantity, LocalConfig.DbType);
			command.Parameters.AddWithValue("@Title", transactionItem.Title, LocalConfig.DbType);
			command.Parameters.AddWithValue("@Comment", transactionItem.Comment, LocalConfig.DbType);
			command.Parameters.AddWithValue("@Date", transactionItem.Date, LocalConfig.DbType);

			command.Parameters.AddWithValue("@UnitID", transactionItem.Unit.ID, LocalConfig.DbType);
			command.Parameters.AddWithValue("@CategoryID", transactionItem.Category.ID, LocalConfig.DbType);

			command.Parameters.AddWithValue("@IsExpenseItem", transactionItem.IsExpenseItem, LocalConfig.DbType);
			command.Parameters.AddWithValue("@IsIncomeItem", transactionItem.IsIncomeItem, LocalConfig.DbType);

			command.ExecuteNonQuery();
		}

		private void CalculateSummaries()
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
				SummaryItemManagerDb.ReplaceSummary(summary, grouping.Key, transactionItemType);
			}
		}

		#endregion

		#region Others

		public void ClearAllTables()
		{
			switch(LocalConfig.DbType)
			{
				case DbType.MsSql:
					using(var ctx = ExinAdoNetContextFactory.Create(LocalConfig))
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

					using(var ctx = ExinAdoNetContextFactory.Create(LocalConfig))
					{
						ctx.Command.CommandText = sqliteCreateScript;
						ctx.ExecInTransactionWithCommit();
					}

					break;

				default:
					throw new NotImplementedException(string.Format(Localized.ImportData_ClearAllTables_is_not_implemented_to_this_DbType___0_, LocalConfig.DbType));
			}

			//MessagePresenter.Instance.WriteLine(Localized.The_database_tables_are_ready__have_been_emptied);
			//MessagePresenter.Instance.WriteLine("");

			//UnitManager.RefreshCache();
			//CategoryManager.RefreshCache();

			// We have to clear all the caches as well with the tables
			//UnitManager.ClearCache();
			//CategoryManager.ClearCache();
		}

		#endregion

		private static void WithoutLogging(Action action)
		{
			var wasMuted = MessagePresenter.Instance.IsMuted;
			MessagePresenter.Instance.IsMuted = true;

			action();

			MessagePresenter.Instance.IsMuted = wasMuted;
		}
	}
}
