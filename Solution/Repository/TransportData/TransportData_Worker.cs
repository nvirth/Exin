using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Common;
using Common.Configuration;
using Common.Db.Entities;
using Common.UiModels.WPF;
using Common.Utils;
using Common.Utils.Helpers;
using DAL.DataBase;
using DAL.DataBase.AdoNet;
using DAL.RepoCommon;
using DAL.RepoCommon.Interfaces;
using DAL.RepoCommon.Managers;
using DAL.RepoCommon.Managers.Factory;
using Localization;

namespace TransportData
{
	public class TransportData_Worker
	{
		#region Members

		public readonly List<Unit> Units = new List<Unit>();
		public readonly List<Category> Categories = new List<Category>();
		public readonly List<ExpenseItem> ExpenseItems = new List<ExpenseItem>();
		public readonly List<IncomeItem> IncomeItems = new List<IncomeItem>();

		public IEnumerable<IGrouping<DateTime, TransactionItemBase>> IncomeItemsByDate;
		public IEnumerable<IGrouping<DateTime, TransactionItemBase>> ExpenseItemsByDate;

		// --

		public readonly IRepoConfiguration LocalConfig;

		public readonly SummaryItemManager SummaryItemManagerLocal;
		public readonly TransactionItemManager TransactionItemManagerLocal;

		public readonly CategoryManager CategoryManagerLocalRead;
		public readonly ICategoryManagerDao CategoryManagerLocalWrite; // We need another instance for write because the caching

		public readonly UnitManager UnitManagerLocalRead;
		public readonly IUnitManagerDao UnitManagerLocalWrite;  // We need another instance for write because the caching

		#endregion

		public TransportData_Worker(IRepoConfiguration localConfig)
		{
			LocalConfig = localConfig;
			
			CategoryManagerLocalRead = new CategoryManager(LocalConfig);
			UnitManagerLocalRead = new UnitManager(LocalConfig);
			TransactionItemManagerLocal = new TransactionItemManager(LocalConfig, CategoryManagerLocalRead, UnitManagerLocalRead);
			SummaryItemManagerLocal = new SummaryItemManager(LocalConfig, TransactionItemManagerLocal, CategoryManagerLocalRead);

			CategoryManagerLocalWrite = new ManagerDaoFactory(LocalConfig).CategoryManager;
			UnitManagerLocalWrite = new ManagerDaoFactory(LocalConfig).UnitManager;

			StaticInitializer.InitAllStatic(CategoryManagerLocalRead, UnitManagerLocalRead);
		}

		public void DoWork()
		{
			// TODO messages
			MessagePresenter.Instance.WriteLine(Localized.Importing_data_from_File_Repository_into___0___DataBase.Formatted(LocalConfig.DbType));
			MessagePresenter.Instance.WriteLine();

			PrepareDestinationRepo();
			Helpers.ExecuteWithTimeMeasuring(TransportUnits, Localized.Importing_units);
			Helpers.ExecuteWithTimeMeasuring(TransportCategories, Localized.Importing_categories);
			TransportTransactions();
			CalculateSummaries();

			MessagePresenter.Instance.WriteLine(Localized._end_);
			MessagePresenter.Instance.WriteLine();
			MessagePresenter.Instance.WriteLine(Localized.Operation_finished_successfully__);
		}

		// --

		private void TransportUnits()
		{
			var units = UnitManagerLocalRead.GetAll();

			foreach(var u in units)
			{
				Units.Add(u);
				UnitManagerLocalWrite.Add(u);
			}
		}

		private void TransportCategories()
		{
			var categories = CategoryManagerLocalRead.GetAll();

			foreach(var u in categories)
			{
				Categories.Add(u);
				CategoryManagerLocalWrite.Add(u);
			}
		}

		private void TransportTransactions()
		{
			// TODO implement GetFirst or smg
			// TODO also implement than GetLast...

			// TODO logging to UI properly

			WithLoggingOnlyErrors(() => {
				var startDate = new DateTime(2009, 01, 01);
				var endDate = DateTime.Today;

				var actualDate = startDate;
				while(actualDate <= endDate)
				{
					// -- Incomes (read & write)
					var monthlyIncomes = TransactionItemManagerLocal.GetMonthlyIncomes(actualDate);
					IncomeItems.AddRange(monthlyIncomes);

					// have to be called for every month, even for the empty ones; other way
					// no Summaries.xml will be created
					TransactionItemManagerLocal.ReplaceMonthlyIncomes(monthlyIncomes, actualDate);

					// -- Expenses (read)
					var monthlyExpenses = TransactionItemManagerLocal.GetMonthlyExpenses(actualDate);
					ExpenseItems.AddRange(monthlyExpenses);

					actualDate = actualDate.AddMonths(1);
				}

				ExpenseItemsByDate = ExpenseItems.GroupBy(ei => ei.Date);
				IncomeItemsByDate = IncomeItems.GroupBy(ii => ii.Date);

				// -- Expenses (write)
				foreach(IGrouping<DateTime, TransactionItemBase> groupByDate in ExpenseItemsByDate)
				{
					// TODO test this
					TransactionItemManagerLocal.ReplaceDailyExpenses(
						expenseItems: groupByDate.Cast<ExpenseItem>(),
						date: groupByDate.Key
						);
				}
			})
			.ExecuteWithTimeMeasuring(Localized.Transporting_expenses_and_incomes);
		}

		private void CalculateSummaries()
		{
			WithLoggingOnlyErrors(() => {
				var expensesAndIncomes = ExpenseItemsByDate.Concat(IncomeItemsByDate);
				foreach(IGrouping<DateTime, TransactionItemBase> groupByDate in expensesAndIncomes)
				{
					var summary = Summary.Summarize(groupByDate);

					var transactionItemType = summary.SumIn == 0 ? TransactionItemType.Expense : TransactionItemType.Income;
					SummaryItemManagerLocal.ReplaceSummary(summary, groupByDate.Key, transactionItemType);
				}
			})
			.ExecuteWithTimeMeasuring(Localized.Calculating_summaries);
		}

		#region PrepareDestinationRepo

		public void PrepareDestinationRepo()
		{
			// TODO Backup when implemented
			switch(LocalConfig.SaveMode)
			{
				case SaveMode.OnlyToDb:
					ClearAllTables();
					break;
				case SaveMode.OnlyToFile:
					ClearFileRepo();
					break;
				default:
                    throw new NotImplementedException("TransportData_Worker.PrepareDestinationRepo is not implemented to this DbType: {0}".Formatted(LocalConfig.SaveMode));
			}
		}

		#region ClearAllTables

		private void ClearAllTables()
		{
			switch (LocalConfig.DbType)
			{
				case DbType.MsSql:
					ClearAllMsSqlTables();
					break;

				case DbType.SQLite:
					ClearAllSqliteTables();
					break;

				default:
					throw new NotImplementedException(string.Format(Localized.TransportDataFromFileToDb_ClearAllTables_is_not_implemented_to_this_DbType___0_, LocalConfig.DbType));
			}
		}

		private void ClearAllMsSqlTables()
		{
			PromptBackupWarning(" * MS SQL database with connection string: " + new ExinConnectionString(LocalConfig).Get);

			Helpers.ExecuteWithTimeMeasuring(() => {
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
			}, Localized.Creating_or_emptying_tables);
		}

		private void ClearAllSqliteTables()
		{
			PromptBackupWarning(" * SQLite database: " + RepoPaths.SqliteDbFile);

			Helpers.ExecuteWithTimeMeasuring(() => {
				using (var dbFileStream = File.OpenWrite(RepoPaths.SqliteDbFile))
					dbFileStream.SetLength(0);

				var sqliteCreateScript = File.ReadAllText(RepoPaths.SqliteDbCreateFile);

				using (var ctx = ExinAdoNetContextFactory.Create(LocalConfig))
				{
					ctx.Command.CommandText = sqliteCreateScript;
					ctx.ExecInTransactionWithCommit();
				}
			}, Localized.Creating_or_emptying_tables);
		}

		#endregion

		private void ClearFileRepo()
		{
			var sb = new StringBuilder();
			sb.AppendLine(" * {0}: {1}".Formatted(RepoPaths.Names.Units, RepoPaths.UnitsFile));
			sb.AppendLine(" * {0}: {1}".Formatted(RepoPaths.Names.Categories, RepoPaths.CategoriesFile));
			sb.AppendLine(" * {0} directory: {1}".Formatted(RepoPaths.Names.ExpensesAndIncomes, RepoPaths.ExpensesAndIncomesDir));
			sb.AppendLine(" * {0} directory: {1}".Formatted(RepoPaths.Names.Summaries, RepoPaths.SummariesDir));

			PromptBackupWarning(sb.ToString());

			Helpers.ExecuteWithTimeMeasuring(RepoPaths.ClearFileRepo, Localized.Clearing_file_repository);
		}

		private void PromptBackupWarning(string backupMsg)
		{
			MessagePresenter.Instance.WriteLine("Before going further, please create a manual backup of these: ");
			MessagePresenter.Instance.WriteLine(backupMsg);
			MessagePresenter.Instance.WriteLine();
			Helpers.ExecuteWithConsoleColor(
				ConsoleColor.Yellow,
				() => MessagePresenter.Instance.WriteLine("Press any key when you are ready to continue . . . ")
			);
			Console.ReadKey();
			MessagePresenter.Instance.WriteLine();
		}

		#endregion

		private static void ExecuteWithLoggingOnlyErrors(Action action)
		{
			var muteLevel = MessagePresenter.Instance.MuteLevel;
			MessagePresenter.Instance.MuteLevel = MuteLevel.WriteOnlyErrors;

			action();

			MessagePresenter.Instance.MuteLevel = muteLevel;
		}

		private static Action WithLoggingOnlyErrors(Action action)
		{
			return () => {
				var muteLevel = MessagePresenter.Instance.MuteLevel;
				MessagePresenter.Instance.MuteLevel = MuteLevel.WriteOnlyErrors;

				action();

				MessagePresenter.Instance.MuteLevel = muteLevel;
			};
		}
	}
}

// TODO remove
//private void TransportTransactions()
//{
//	// TODO implement GetFirst or smg
//	// TODO also implement than GetLast...

//	// TODO logging to UI properly
//	var muteLevel = MessagePresenter.Instance.MuteLevel;
//	MessagePresenter.Instance.MuteLevel = MuteLevel.WriteOnlyErrors;

//	var startDate = new DateTime(2009, 01, 01);
//	var endDate = DateTime.Today;

//	var actualDate = startDate;
//	while(actualDate <= endDate)
//	{
//		// -- Incomes
//		var monthlyIncomes = TransactionItemManagerLocal.GetMonthlyIncomes(actualDate);
//		IncomeItems.AddRange(monthlyIncomes);
//		TransactionItemManagerLocal.ReplaceMonthlyIncomes(monthlyIncomes, actualDate);

//		// -- Expenses
//		var monthlyExpenses = TransactionItemManagerLocal.GetMonthlyExpenses(actualDate);
//		ExpenseItems.AddRange(monthlyExpenses);

//		var actualDayDate = actualDate;
//		while(actualDayDate.Month == actualDate.Month) // iterate thruogh this month
//		{
//			var day = actualDayDate.Day;

//			TransactionItemManagerLocal.ReplaceDailyExpenses( // TODO test this
//				monthlyExpenses.Where(ei => ei.Date.Day == day),
//				actualDayDate
//			);

//			actualDayDate = actualDayDate.AddDays(1);
//		}

//		actualDate = actualDate.AddMonths(1);
//	}

//	MessagePresenter.Instance.MuteLevel = muteLevel;
//}
