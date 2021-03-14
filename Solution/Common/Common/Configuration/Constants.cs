namespace Common.Configuration
{
	/// <summary>
	/// Solution-level global constants
	/// </summary>
	public static class Constants
	{
		public static class AppSettingsKeys
		{
			public const string RepoRootDir = "RepoRootDir";
			public const string ReadMode = "ReadMode";
			public const string SaveMode = "SaveMode";
			public const string DbAccessMode = "DbAccessMode";
			public const string DbType = "DbType";
		}

		public static class Db
		{
			// ConnenctionString names
			public const string MsSql_AdoNet_ConnStr = "Exin_MsSql_AdoNet_ConnStr";
			public const string MsSql_EF_ConnStr = "Exin_MsSql_EF_ConnStr";
			public const string SQLite_AdoNet_ConnStr = "Exin_SQLite_AdoNet_ConnStr";
			public const string SQLite_EF_ConnStr = "Exin_SQLite_EF_ConnStr";

			// etc
			public const string SqliteDbFullpathPlaceholder = "#SQLITE_REPO_FULLPATH#";
			public const string ExinDataSet = "ExinDataSet";
		}

		public static class Resources
		{
			// Exin\Solution\Repository\DB\ResourcesDefault\Categories.xml
			public static class DefaultCategories
			{
				// Helpers
				public const string None = "None";
				public const string FullExpenseSummary = "FullExpenseSummary";
				public const string FullIncomeSummary = "FullIncomeSummary";
				// Valid expense categories
				public const string Eats = "Eats";
				public const string ConfectionTonic = "ConfectionTonic";
				public const string Booze = "Booze";
				public const string Invoice = "Invoice";
				public const string Train = "Train";
				public const string OtherPublicTransport = "OtherPublicTransport";
				public const string Cigarette = "Cigarette";
				public const string Others = "Others";
				// NOTE! Categories have been extended, but new items did not get hard wired here. Check the above xml for them.
			}

			// Exin\Solution\Repository\DB\ResourcesDefault\Units.xml
			public static class DefaultUnits
			{
				// Helpers
				public const string None = "None";
				// Valid units
				public const string Pc = "Pc";
				public const string Kg = "Kg";
				public const string Dkg = "Dkg";
				public const string Gram = "Gram";
				public const string Liter = "Liter";
			}

			public static class AssemblyNames
			{
				public const string TransportData_exe = "TransportData.exe";
			}
		}

		public static class TransportData
		{
			public const string FILEREPO = "FILEREPO";
			public const string DB_MSSQL = "DB_MSSQL";
			public const string DB_SQLITE = "DB_SQLITE";
			public const string EN = "EN";
			public const string HU = "HU";
		}

		public static class Xml
		{
			public static class CommonTags
			{
				public const string root = "root";
			}

			public static class TransactionItem
			{
				public const string root = "root";

				public const string ExpenseItem = "ExpenseItem";
				public const string IncomeItem = "IncomeItem";

				public const string DailySummary = "DailySummary";
				public const string IncomeSummary = "IncomeSummary";
				public const string MonthlySummary = "MonthlySummary";

				public const string ID = "ID";
				public const string Name = "Name";
				public const string DisplayName = "DisplayName";
				public const string DisplayNames = "DisplayNames";
				public const string Title = "Title";
				public const string Amount = "Amount";
				public const string Quantity = "Quantity";
				public const string Unit = "Unit";
				public const string Category = "Category";
				public const string Comment = "Comment";
			}

			public static class Settings
			{
				// MainSettings.xml
				//
				// root/App
				// --Version
				// --LastInitVersion
				// --Repositories
				// ----Repo
				// ------Name
				// ------RootDir
				// --UserSettings
				// ----AllowsFutureDate
				// ----CopyFormat
				// ----Language
				// ----CurrentRepoNames
				// --Logging
				// ----UiLoggerLevel
				// ----LogLoggerLevel
				//
				// RepoSettings.xml
				// 
				// root/Repo
				// --Version
				// --LastInitVersion
				// --Currency
				// --ReadMode
				// --SaveMode
				// --DbAccessMode
				// --DbType
				// --UserSettings
				// ----StatYAxisMax
				// --MsSqlSettings
				// ----ConnectionStrings
				// ------AdoNet
				// ------EntityFramework
				//
				public const string AdoNet = "AdoNet";
				public const string AllowsFutureDate = "AllowsFutureDate";
				public const string App = "App";
				public const string ConnectionStrings = "ConnectionStrings";
				public const string CopyFormat = "CopyFormat";
				public const string Currency = "Currency";
				public const string CurrentRepoNames = "CurrentRepoNames";
				public const string DbAccessMode = "DbAccessMode";
				public const string DbType = "DbType";
				public const string EntityFramework = "EntityFramework";
				public const string Language = "Language";
				public const string LastInitVersion = "LastInitVersion";
				public const string Logging = "Logging";
				public const string LogLoggerLevel = "LogLoggerLevel";
				public const string MsSqlSettings = "MsSqlSettings";
				public const string Name = "Name";
				public const string ReadMode = "ReadMode";
				public const string Repo = "Repo";
				public const string Repositories = "Repositories";
				public const string RootDir = "RootDir";
				public const string SaveMode = "SaveMode";
				public const string StatYAxisMax = "StatYAxisMax";
				public const string UiLoggerLevel = "UiLoggerLevel";
				public const string UserSettings = "UserSettings";
				public const string Version = "Version";
			}

			public const string EmptyXmlContent = "<?xml version=\"1.0\"?>" + "<" + CommonTags.root + ">" + "</" + CommonTags.root + ">";
		}
	}
}
