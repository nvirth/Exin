using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Common;
using Common.Configuration;
using Common.Utils;
using Common.Utils.Helpers;
using Localization;
using AssemblyNames = Common.Configuration.Constants.Resources.AssemblyNames;
using C = Common.Configuration.Constants.TransportData;

namespace DAL.DataBase
{
	public static class SQLiteSpecific
	{
		public static bool IsDbOk(DbType dbType = 0)
		{
			dbType = dbType == 0 ? Config.Repo.Settings.DbType : dbType;

			if(dbType != DbType.SQLite)
				return true;

			if(File.Exists(Config.Repo.Paths.SqliteDbFile) && new FileInfo(Config.Repo.Paths.SqliteDbFile).Length != 0)
				return true;

			return false;
		}

		public static async Task InitSqliteFileIfNeeded(DbType dbType = 0)
		{
			dbType = dbType == 0 ? Config.Repo.Settings.DbType : dbType;

			if (IsDbOk(dbType))
				return;

			MessagePresenter.Instance.WriteError(string.Format(Localized.Could_not_find_the_SQLite_database_file__here__0__FORMAT__, Config.Repo.Paths.SqliteDbFile));
			MessagePresenter.Instance.WriteLine(Localized.The_application_will_now_create_the_database_file__this_can_take_even_a_few_minutes_);
			MessagePresenter.Instance.WriteLine(Localized.Please__wait_until_it_s_done);
			MessagePresenter.Instance.WriteLine();

			var wasSuccessful = await StartTransportDataProcess();

			if (wasSuccessful)
			{
				MessagePresenter.Instance.WriteLine(Localized.The_SQLite_database_file_is_ready);
				MessagePresenter.Instance.WriteLine(Localized.You_can_start_your_work_);
			}
			else
			{
				MessagePresenter.Instance.WriteError(Localized.Error_occured_while_creating_the_SQLite_database_file);
			}

			MessagePresenter.Instance.WriteLineSeparator();
		}

		public static async Task<bool> StartTransportDataProcess()
		{
			return await new TaskFactory<bool>().StartNew(
				() => {
					var lang = Cultures.CurrentCulture.TwoLetterISOLanguageName;
					var commandArgs = "--From FileRepo --To {0} --Lang {1}".Formatted(C.DB_SQLITE, lang);

					var startInfo = new ProcessStartInfo
					{
						//WorkingDirectory = Paths.Play_publicPath,
						FileName = Path.Combine(Config.AppExecDir, AssemblyNames.TransportData_exe),
						Arguments = commandArgs,
						//WindowStyle = ProcessWindowStyle.Hidden,
						//RedirectStandardOutput = true,
						//RedirectStandardError = true,
						//UseShellExecute = false,
					};
					using(var process = new Process { StartInfo = startInfo })
					{
						process.Start();

						//var standardOutput = process.StandardOutput;
						//var outMsg = standardOutput.ReadToEnd();
						//if(!String.IsNullOrEmpty(outMsg))
						//	MessagePresenter.Instance.WriteLine(outMsg);

						//var standardError = process.StandardError;
						//var errorMsg = standardError.ReadToEnd();
						//if(!String.IsNullOrEmpty(errorMsg))
						//	MessagePresenter.Instance.WriteError(errorMsg);

						process.WaitForExit();
						return process.ExitCode == 0;
					}					
				});
		}
	}
}
