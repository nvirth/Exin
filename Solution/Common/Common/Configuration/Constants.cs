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
			public const string MsSql_AdoNet = "ExinConnectionString";
			public const string MsSql_Ef = "ExinEfMsSqlConnStr";
			public const string SQLite_AdoNet = "ExinSQLiteConnectionString";
			public const string SQLite_Ef = "ExinEfSqliteConnStr";

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
			}

			// Exin\Solution\Repository\DB\ResourcesDefault\Units.xml
			public static class DefaultUnits
			{
				// Helpers
				public const string None = "None";
				// Valid units
				public const string Db = "Db";
				public const string Kg = "Kg";
				public const string Dkg = "Dkg";
				public const string Gram = "Gram";
				public const string Liter = "Liter";
			}

			public static class AssemblyNames
			{
				public const string ImportDataToDb_exe = "ImportDataToDb.exe";

			}
		}

		public static class XmlTags
		{
			public const string ExpenseItem = "ExpenseItem";
			public const string IncomeItem = "IncomeItem";

			public const string DailySummary = "DailySummary";
			public const string IncomeSummary = "IncomeSummary";
			public const string MonthlySummary = "MonthlySummary";

			public const string ID = "ID";
			public const string Name = "Name";
			public const string DisplayName = "DisplayName";
			public const string Title = "Title";
			public const string Amount = "Amount";
			public const string Quantity = "Quantity";
			public const string Unit = "Unit";
			public const string Category = "Category";
			public const string Comment = "Comment";

			public const string root = "root";
		}
	}
}
