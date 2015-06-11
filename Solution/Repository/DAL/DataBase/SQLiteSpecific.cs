using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Common;
using Common.Configuration;
using Common.Utils;
using Localization;
using C = Common.Configuration.Constants.Resources.AssemblyNames;

namespace DAL.DataBase
{
	public static class SQLiteSpecific
	{
		public static bool IsDbOk(DbType dbType = 0)
		{
			dbType = dbType == 0 ? Config.Repo.DbType : dbType;

			if(dbType != DbType.SQLite)
				return true;

			if(File.Exists(RepoPaths.SqliteDbFile) && new FileInfo(RepoPaths.SqliteDbFile).Length != 0)
				return true;

			return false;
		}

		public static async Task InitSqliteFileIfNeeded(DbType dbType = 0)
		{
			if (IsDbOk(dbType == 0 ? Config.Repo.DbType : dbType))
				return;

			MessagePresenter.Instance.WriteError(string.Format(Localized.Could_not_find_the_SQLite_database_file__here__0__FORMAT__, RepoPaths.SqliteDbFile));
			MessagePresenter.Instance.WriteLine(Localized.The_application_will_now_create_the_database_file__this_can_take_even_a_few_minutes_);
			MessagePresenter.Instance.WriteLine(Localized.Please__wait_until_it_s_done);
			MessagePresenter.Instance.WriteLine("");

			var wasSuccessful = await StartImportDataProcess();

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

		public static async Task<bool> StartImportDataProcess()
		{
			return await new TaskFactory<bool>().StartNew(
				() =>
				{
					var startInfo = new ProcessStartInfo
					{
						//WorkingDirectory = Paths.Play_publicPath,
						FileName = C.ImportDataToDb_exe,
						//Arguments = command.ToString(),
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
