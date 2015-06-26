using System;
using System.Diagnostics;
using Common.Configuration;
using Common.Utils;
using Localization;

namespace TransportData
{
	/// <summary>
	/// Imports data from the file repository to the database. 
	/// </summary>
	public static class TransportData
	{
		//TODO database (ms sql/slite) could be set via command line arguments
		//TODO language settings
		public static void Main(string[] args)
		{
			//StartDebugger();
			Cultures.SetToEnglish();

			MessagePresenterManager.WireToConsole();

			new TransportData_FromFile_ToDb(Config.Repo.DbType).DoWork(args);

			MessagePresenter.Instance.WriteLine(Localized.Press_any_key_to_continue_);
			Console.ReadKey();
		}

		private static void StartDebugger()
		{
			if(!Debugger.IsAttached)
				Debugger.Launch();
			Debugger.Break();
		}
	}
}
