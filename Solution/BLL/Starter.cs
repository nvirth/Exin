using System;
using System.Threading.Tasks;
using BLL.WpfManagers;
using Common.Configuration;
using Common.Utils;
using Common.Utils.Helpers;
using DAL;
using Localization;

namespace BLL
{
	public class Starter
	{
		/// <summary>
		/// If (date == null) then DateTime.Now
		/// </summary>
		public static async Task<DailyExpenses> Start(DateTime? date = null)
		{
			var dateTime = date ?? DateTime.Now;
			await CheckSystem();
			return new DailyExpenses(dateTime, /*doWork*/ true);
		}

		/// <summary>
		/// Check if the directories, files exist. 
		/// Initially the Path class is initialized to DateTime.Now
		/// </summary>
		public static async Task CheckSystem()
		{
			ShowConfig();
			InitRootIfNeeded();
			await SQLiteSpecific.InitSqliteFileIfNeeded();
		}

		private static void InitRootIfNeeded()
		{
			if(RepoPaths.DirectoryInfos.Root.Exists)
				return;

			MessagePresenter.WriteError(string.Format(Localized.Could_not_find_the_Exin_s_work_directory_here__0__FORMAT__, RepoPaths.DirectoryInfos.Root));
			MessagePresenter.WriteLine(Localized.The_app_will_now_create_the_necessary_directories);
			MessagePresenter.WriteLine("");

		    RepoPaths.InitRepo();
			
			MessagePresenter.WriteLine("");
			MessagePresenter.WriteLine(Localized.All_created_successfully_);
			MessagePresenter.WriteLineSeparator();
		}

		private static void ShowConfig()
		{
			MessagePresenter.WriteLine(Localized.The_Exin_expenses_incomes_summarizer_application_welcomes_you_);
			MessagePresenter.WriteLine(Localized.The_application_s_configuration_);
			MessagePresenter.WriteLine(" - " + Config.ReadMode.ToLocalizedDescriptionString());
			MessagePresenter.WriteLine(" - " + Config.SaveMode.ToLocalizedDescriptionString());
			MessagePresenter.WriteLine(" - " + Config.DbAccessMode.ToLocalizedDescriptionString());
			MessagePresenter.WriteLine(" - " + Config.DbType.ToLocalizedDescriptionString());
			MessagePresenter.WriteLineSeparator();
		}
	}
}
