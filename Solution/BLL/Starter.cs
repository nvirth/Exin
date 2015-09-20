using System;
using System.Threading.Tasks;
using BLL.WpfManagers;
using Common.Configuration;
using Common.Utils;
using Common.Utils.Helpers;
using DAL;
using DAL.DataBase;
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
			if(Config.Repo.Paths.CheckRepo())
				return;

			MessagePresenter.Instance.WriteError(string.Format(Localized.Could_not_find_the_Exin_s_work_directory_here__0__FORMAT__, Config.Repo.Paths.DirectoryInfos.Root));
			MessagePresenter.Instance.WriteLine(Localized.The_app_will_now_create_the_necessary_directories);
			MessagePresenter.Instance.WriteLine();

			Config.Repo.Paths.InitRepo();
			
			MessagePresenter.Instance.WriteLine();
			MessagePresenter.Instance.WriteLine(Localized.All_created_successfully_);
			MessagePresenter.Instance.WriteLineSeparator();
		}

		private static void ShowConfig()
		{
			MessagePresenter.Instance.WriteLine(Localized.The_Exin_expenses_incomes_summarizer_application_welcomes_you_);
			MessagePresenter.Instance.WriteLine(Localized.The_application_s_configuration_);
			MessagePresenter.Instance.WriteLine(" - " + Config.Repo.Settings.ReadMode.ToLocalizedDescriptionString());
			MessagePresenter.Instance.WriteLine(" - " + Config.Repo.Settings.SaveMode.ToLocalizedDescriptionString());
			MessagePresenter.Instance.WriteLine(" - " + Config.Repo.Settings.DbAccessMode.ToLocalizedDescriptionString());
			MessagePresenter.Instance.WriteLine(" - " + Config.Repo.Settings.DbType.ToLocalizedDescriptionString());
			MessagePresenter.Instance.WriteLineSeparator();
		}
	}
}
