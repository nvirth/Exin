using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Common;
using Common.Config;
using Common.DbEntities;
using Common.UiModels;
using Common.UiModels.WPF;
using Common.Utils;
using DAL;
using DAL.DataBase.AdoNet;
using DAL.DataBase.AdoNet.Managers;
using DAL.DataBase.Managers;
using DAL.FileRepo;
using Localization;
using UtilsShared;

namespace ImportDataToDb
{
	/// <summary>
	/// Imports data from the file repository to the database. 
	/// </summary>
	public static class ImportDataToDb
	{
		//TODO database (ms sql/slite) could be set via command line arguments
		public static void Main(string[] args)
		{
			//StartDebugger();

			MessagePresenterManager.WireToConsole();

			CheckConfig();

			ImportData.ImportDataFromFileRepoToDb(args);

			MessagePresenter.WriteLine(Localized._end_);
			MessagePresenter.WriteLine("");
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
