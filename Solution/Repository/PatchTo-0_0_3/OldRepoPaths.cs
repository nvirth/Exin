using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using Common.Configuration;
using Common.Utils;
using Common.Utils.Helpers;
using Localization;
using C = Common.Configuration.Constants;

namespace PatchTo_0_0_3
{

	public static class OldRepoPaths
	{
		/// <summary>
		/// Provides the concrete file/dir names
		/// </summary>
		public static class Names
		{
			//-- Directories
			public const string Monthly = "0_Összesen";
			public const string Categorised = "0_Összesen - Kategóriák";

			//-- Files
			public const string Units = "Units.xml";
			public const string Categories = "Categories.xml";
			public const string SqliteDbFile = "Exin.sqlite";
		}

		//-- Directories
		public static readonly string AppExecDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
		public static readonly string RootDir = SetupRootDir();

		public static readonly string SummariesDir = RootDir;
		public static readonly string MonthlySummariesDir = SummariesDir + "\\" + Names.Monthly;
		public static readonly string CategorisedSummariesDir = SummariesDir + "\\" + Names.Categorised;
		public static readonly string ExpensesAndIncomesDir = RootDir;
		public static readonly string DataDir = RootDir;

		//-- Files
		public static readonly string UnitsFile = DataDir + "\\" + Names.Units;
		public static readonly string CategoriesFile = DataDir + "\\" + Names.Categories;
		public static readonly string SqliteDbFile = DataDir + "\\" + Names.SqliteDbFile;

		// -- Methods
		private static string SetupRootDir()
		{
			var rootDir = ConfigurationManager.AppSettings[C.AppSettingsKeys.RepoRootDir];
			if(!Path.IsPathRooted(rootDir))
				throw new ConfigurationErrorsException(Localized.The__RootDir__config_entry_either_have_to_be_empty_or_contain_a_full_path__);

			return rootDir;
		}
	}
}