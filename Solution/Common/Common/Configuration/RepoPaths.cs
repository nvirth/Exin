using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using Common.Utils;
using Common.Utils.Helpers;
using Localization;
using C = Common.Configuration.Constants;

namespace Common.Configuration
{
	/// <summary>
	/// Provides full paths of the used directories/files. <para/>
	/// The repository's structure looks like this: <para>&nbsp;</para>
	/// 
	/// [Root] <para/>
	/// --Summaries <para/>
	/// ----Categorised <para/>
	/// ----Monthly <para/>
	/// --ExpensesAndIncomes <para/>
	/// --Data <para/>
	/// ----Categories.xml <para/>
	/// ----RepoSettings.xml <para/>
	/// ----Units.xml <para/>
	/// ----Exin.sqlite <para/>
	/// --Backup <para>&nbsp;</para>
	/// 
	/// [AppExecDir] <para/>
	/// --ResourcesDefault <para/>
	/// ----Categories.xml <para/>
	/// ----RepoSettings.xml <para/>
	/// ----Units.xml <para/>
	/// --SQLite full.sql <para/>
	/// </summary>
	public class RepoPaths
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
			public const string RepoSettings = "RepoSettings.xml";
			public const string SqliteDbFile = "Exin.sqlite";
			public const string SqliteDbCreateFile = "SQLite full.sql";

			//-- Files (not singletons)
			public const string MonthlyExpensesSum = "Sum." + Config.FileExtension;
			public const string MonthlyIncomesSum = "Incomes." + Config.FileExtension;
		}

		public class DirectoryInfosClass
		{
			public readonly DirectoryInfo Root;
			public readonly DirectoryInfo Summaries;
			public readonly DirectoryInfo MonthlySummaries;
			public readonly DirectoryInfo CategorisedSummaries;
			public readonly DirectoryInfo ExpensesAndIncomes;
			public readonly DirectoryInfo Data;
			public readonly DirectoryInfo Backup;

			public DirectoryInfosClass(RepoPaths repoPaths)
			{
				Root = new DirectoryInfo(repoPaths.RootDir);
				Summaries = new DirectoryInfo(repoPaths.SummariesDir);
				MonthlySummaries = new DirectoryInfo(repoPaths.MonthlySummariesDir);
				CategorisedSummaries = new DirectoryInfo(repoPaths.CategorisedSummariesDir);
				ExpensesAndIncomes = new DirectoryInfo(repoPaths.ExpensesAndIncomesDir);
				Data = new DirectoryInfo(repoPaths.DataDir);
				Backup = new DirectoryInfo(repoPaths.BackupDir);
			}
		}
		public readonly DirectoryInfosClass DirectoryInfos;

		//-- Static Directories
		public static readonly string AppExecDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
		private static readonly string ResourcesDefaultDir = AppExecDir + "\\" + Names.ResourcesDefault;

		//-- Static Files
		public static readonly string SqliteDbCreateFile = AppExecDir + "\\" + Names.SqliteDbCreateFile;
		private static readonly string CategoriesDefaultFile = ResourcesDefaultDir + "\\" + Names.Categories;
		private static readonly string UnitsDefaultFile = ResourcesDefaultDir + "\\" + Names.Units;
		private static readonly string RepoSettingsDefaultFile = ResourcesDefaultDir + "\\" + Names.RepoSettings;

		//-- Directories
		public readonly string RootDir;

		public readonly string SummariesDir;
		public readonly string MonthlySummariesDir;
		public readonly string CategorisedSummariesDir;
		public readonly string ExpensesAndIncomesDir;
		public readonly string DataDir;
		public readonly string BackupDir;

		//-- Files
		public readonly string CategoriesFile;
		public readonly string UnitsFile;
		public readonly string RepoSettingsFile;
		public readonly string SqliteDbFile;

		// -- Methods

		public RepoPaths(string rootDir)
		{
			//-- Directories
			RootDir = rootDir;
			SummariesDir = RootDir + "\\" + Names.Summaries;
			MonthlySummariesDir = SummariesDir + "\\" + Names.Monthly;
			CategorisedSummariesDir = SummariesDir + "\\" + Names.Categorised;
			ExpensesAndIncomesDir = RootDir + "\\" + Names.ExpensesAndIncomes;
			DataDir = RootDir + "\\" + Names.Data;
			BackupDir = RootDir + "\\" + Names.Backup;

			//-- Files
			CategoriesFile = DataDir + "\\" + Names.Categories;
			UnitsFile = DataDir + "\\" + Names.Units;
			RepoSettingsFile = DataDir + "\\" + Names.RepoSettings;
			SqliteDbFile = DataDir + "\\" + Names.SqliteDbFile;

			//--
			DirectoryInfos = new DirectoryInfosClass(this);
		}

		/// <summary>
		/// For debug purposes
		/// </summary>
		public void PrintRepoStructure()
		{
			MessagePresenter.Instance.WriteLine("The app's directory structure: ");
			DirsToCreate.ForEach(MessagePresenter.Instance.WriteLine);
		}

		/// <summary>
		/// Initializes the repo structure, and copies the default (Unit, Category) resources
		/// to their newly created place. The initialization is not full in case of using
		/// SQLite db, then a call to SQLiteSpecific.InitSqliteFileIfNeeded() is also necessary 
		/// </summary>
		public void InitRepo(bool silent = false)
		{
			foreach(var dir in DirsToCreate)
			{
				Directory.CreateDirectory(dir);
				if(!silent)
					MessagePresenter.Instance.WriteLine(string.Format(Localized.Created___0__FORMAT__, dir));
			}

			if(!File.Exists(UnitsFile))
				File.Copy(UnitsDefaultFile, UnitsFile);

			if(!File.Exists(CategoriesFile))
				File.Copy(CategoriesDefaultFile, CategoriesFile);

			if(!File.Exists(RepoSettingsFile))
				File.Copy(RepoSettingsDefaultFile, RepoSettingsFile);
		}

		public bool CheckRepo()
		{
			if(DirsToCreate.Any(dir => !Directory.Exists(dir)))
				return false;

			if(!File.Exists(CategoriesFile) || !File.Exists(UnitsFile) || !File.Exists(RepoSettingsFile))
				return false;

			return true;
		}

		public void ClearFileRepo()
		{
			foreach(var dirPath in FileRepoDirsToRecreate)
				Helpers.RecreateDirectory(dirPath);


			using(var unitsFile = new StreamWriter(UnitsFile, append: false))
				unitsFile.Write(C.Xml.EmptyXmlContent);

			using(var categoriesFile = new StreamWriter(CategoriesFile, append: false))
				categoriesFile.Write(C.Xml.EmptyXmlContent);

			using(var repoSettingsFile = new StreamWriter(RepoSettingsFile, append: false))
				repoSettingsFile.Write(C.Xml.EmptyXmlContent);
		}

		private IEnumerable<string> DirsToCreate
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

		private IEnumerable<string> FileRepoDirsToRecreate
		{
			get
			{
				yield return SummariesDir; // Order is optimal so
				yield return MonthlySummariesDir;
				yield return CategorisedSummariesDir;
				yield return ExpensesAndIncomesDir;
			}
		}
	}
}