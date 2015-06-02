using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using Common.Utils;
using Common.Utils.Helpers;
using Localization;

namespace Common.Config
{
	/// <summary>
	/// Provides full paths of the used directories/files. <para/>
	/// The repository's structure looks like this: <para>&nbsp;</para>
	/// 
	/// [Root] <para/>
	/// --Summaries <para/>
	/// ----Monthly <para/>
	/// ----Categorised <para/>
	/// --ExpensesAndIncomes <para/>
	/// --Data <para/>
	/// ----Units.xml <para/>
	/// ----Categories.xml <para/>
	/// ----Exin.sqlite <para/>
	/// --Backup <para>&nbsp;</para>
	/// 
	/// [AppExecDir] <para/>
	/// --ResourcesDefault <para/>
	/// ----Units.xml <para/>
	/// ----Categories.xml <para/>
	/// --SQLite full.sql <para/>
	/// </summary>
	public static class RepoPaths
	{
		/// <summary>
		/// Provides the concrete file/dir names
		/// </summary>
		public static class Names
		{
			//-- Directories
			public const string Summaries = "Summaries";
			public const string Monthly = "Monthly";
			public const string Categorised = "Categorised";
			public const string ExpensesAndIncomes = "ExpensesAndIncomes";
			public const string Data = "Data";
			public const string Backup = "Backup";

			internal const string ResourcesDefault = "ResourcesDefault";

			//-- Files
			public const string Units = "Units.xml";
			public const string Categories = "Categories.xml";
			public const string SqliteDbFile = "Exin.sqlite";
			public const string SqliteDbCreateFile = "SQLite full.sql";

			//-- Files (not singletons)
			public const string MonthlyExpensesSum = "Sum." + Config.FileExtension;
			public const string MonthlyIncomesSum = "Incomes." + Config.FileExtension;
		}

		public static class DirectoryInfos
		{
			public static readonly DirectoryInfo Root = new DirectoryInfo(RootDir);
			public static readonly DirectoryInfo Summaries = new DirectoryInfo(SummariesDir);
			public static readonly DirectoryInfo MonthlySummaries = new DirectoryInfo(MonthlySummariesDir);
			public static readonly DirectoryInfo CategorisedSummaries = new DirectoryInfo(CategorisedSummariesDir);
			public static readonly DirectoryInfo ExpensesAndIncomes = new DirectoryInfo(ExpensesAndIncomesDir);
			public static readonly DirectoryInfo Data = new DirectoryInfo(DataDir);
			public static readonly DirectoryInfo Backup = new DirectoryInfo(BackupDir);
		}

		//-- Directories
		public static readonly string AppExecDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
		public static readonly string RootDir = SetupRootDir();

		public static readonly string SummariesDir = RootDir + "\\" + Names.Summaries;
		public static readonly string MonthlySummariesDir = SummariesDir + "\\" + Names.Monthly;
		public static readonly string CategorisedSummariesDir = SummariesDir + "\\" + Names.Categorised;
		public static readonly string ExpensesAndIncomesDir = RootDir + "\\" + Names.ExpensesAndIncomes;
		public static readonly string DataDir = RootDir + "\\" + Names.Data;
		public static readonly string BackupDir = RootDir + "\\" + Names.Backup;

		private static readonly string ResourcesDefaultDir = AppExecDir + "\\" + Names.ResourcesDefault;

		//-- Files
		public static readonly string UnitsFile = DataDir + "\\" + Names.Units;
		public static readonly string CategoriesFile = DataDir + "\\" + Names.Categories;
		public static readonly string SqliteDbFile = DataDir + "\\" + Names.SqliteDbFile;

		public static readonly string SqliteDbCreateFile = AppExecDir + "\\" + Names.SqliteDbCreateFile;

		private static readonly string CategoriesDefaultFile = ResourcesDefaultDir + "\\" + Names.Categories;
		private static readonly string UnitsDefaultFile = ResourcesDefaultDir + "\\" + Names.Units;

		// -- Methods
		private static string SetupRootDir()
		{
			var rootDir = ConfigurationManager.AppSettings["RootDir"];
			if(string.IsNullOrWhiteSpace(rootDir))
				rootDir = AppExecDir;
			else if(!Path.IsPathRooted(rootDir))
				throw new ConfigurationErrorsException(Localized.The__RootDir__config_entry_either_have_to_be_empty_or_contain_a_full_path__);

			return rootDir;
		}

		/// <summary>
		/// For debug purposes
		/// </summary>
		public static void PrintRepoStructure()
		{
			MessagePresenter.WriteLine("The app's directory structure: ");
			DirsToCreate.ForEach(MessagePresenter.WriteLine);
		}

		/// <summary>
		/// Initializes the repo structure, and copies the default (Unit, Category) resources
		/// to their newly created place. The initialization is not full in case of using
		/// SQLite db, then a call to SQLiteSpecific.InitSqliteFileIfNeeded() is also necessary
		/// </summary>
		public static void InitRepo()
		{
			foreach(var dir in DirsToCreate)
			{
				Directory.CreateDirectory(dir);
				MessagePresenter.WriteLine(string.Format(Localized.Created___0__FORMAT__, dir));
			}

			File.Copy(UnitsDefaultFile, UnitsFile);
			File.Copy(CategoriesDefaultFile, CategoriesFile);
		}

		private static IEnumerable<string> DirsToCreate
		{
			get
			{
				yield return RootDir;
				yield return SummariesDir;
				yield return MonthlySummariesDir;
				yield return CategorisedSummariesDir;
				yield return ExpensesAndIncomesDir;
				yield return DataDir;
				yield return BackupDir;
			}
		}
	}
}