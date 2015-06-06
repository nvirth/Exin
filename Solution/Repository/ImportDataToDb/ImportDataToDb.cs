using System;
using System.Diagnostics;
using Common;
using Common.Configuration;
using Common.Utils;
using Common.Utils.Helpers;
using Localization;

namespace ImportDataToDb
{
	/// <summary>
	/// Imports data from the file repository to the database. 
	/// </summary>
	public static class ImportDataToDb
	{
		//TODO database (ms sql/slite) could be set via command line arguments
		//TODO language settings
		public static void Main(string[] args)
		{
			//StartDebugger();
			Helpers.SetDefaultCultureToEnglish();

			MessagePresenterManager.WireToConsole();

			CheckConfig();

			ImportData.ImportDataFromFileRepoToDb(args);

			MessagePresenter.WriteLine(Localized.Press_any_key_to_continue_);
			Console.ReadKey();
		}

		private static void StartDebugger()
		{
			if(!Debugger.IsAttached)
				Debugger.Launch();
			Debugger.Break();
		}

		private static void CheckConfig()
		{
			//TODO the program should set it for itself - permanently
	
			if (Config.DbAccessMode != DbAccessMode.AdoNet)
				throw new Exception(Localized.The_ImportData_project_need_to_be_configured_to_use_AdoNet_to_db_access___);

			if (Config.ReadMode != ReadMode.FromDb)
				throw new Exception(Localized.The_ImportData_project_need_to_be_configured_to_read_data_from_database___);
		}
	}
}
