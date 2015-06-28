using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Common.Configuration;
using Common.Log;
using Common.Utils;
using Common.Utils.Helpers;
using DAL.DataBase.AdoNet;
using Localization;
using SharpCompress.Archive.Zip;
using SharpCompress.Common;
using AssemblyNames = Common.Configuration.Constants.Resources.AssemblyNames;
using C = Common.Configuration.Constants.TransportData;

namespace PatchTo_0_0_3
{
	/// <summary>
	/// Patches the (the non-versioned) Repository to the 0.0.3 version. <para />
	/// Notes: <para />
	/// * It also makes a backup, which is a zip file. This won't contain the empty directories from the old Repository<para />
	/// * It uses Hungarian as the language (can't parametrize), because the non-versioned version of the app weren't localized (to EN) <para />
	/// </summary>
	class ProgramPatchTo_0_0_3
	{
		private static string OldDataCompressed = "OldData-" + DateTime.Now.ToString().ToFriendlyUrl() + ".zip";

		static void Main(string[] args)
		{
			//TODO unhandled exception + msg to restore
			MessagePresenterManager.WireToConsole();
			Cultures.SetToHungarian();
			AppDomain.CurrentDomain.UnhandledException += UnhadledExceptionHandler;

			PromptBackupWarning(OldRepoPaths.RootDir);

			MessagePresenter.Instance.WriteLine("Compressing old repository...");
			var compressedTempPath = CompressOldData();
			MessagePresenter.Instance.WriteLine("Temporarily saved here: " + compressedTempPath);

			MessagePresenter.Instance.WriteLine("Reading SQLite database into memory...");
			var sqliteDbFileContent = File.ReadAllBytes(OldRepoPaths.SqliteDbFile);

			MessagePresenter.Instance.WriteLine("Clearing old repository...");
			Helpers.RecreateDirectory(OldRepoPaths.RootDir);

			MessagePresenter.Instance.WriteLine("Initializing new repository structure...");
			InitRepo();

			MessagePresenter.Instance.WriteLine("Moving (compressed) old data into Backup directiory...");
			File.Move(OldDataCompressed, Path.Combine(RepoPaths.BackupDir, OldDataCompressed));

			MessagePresenter.Instance.WriteLine("Writing out SQLite database into file...");
			using(var fileStream = File.OpenWrite(RepoPaths.SqliteDbFile))
				fileStream.Write(sqliteDbFileContent, 0, sqliteDbFileContent.Length);

			MessagePresenter.Instance.WriteLine("Patching SQLite database...");
			var sqliteUpdateScript = File.ReadAllText("UpadetScript_Category.sql");
			sqliteUpdateScript += Environment.NewLine + File.ReadAllText("UpadetScript_Unit.sql");

			using(var ctx = ExinAdoNetContextFactory.Create(new RepoConfiguration() {
				DbType = DbType.SQLite,
				SaveMode = SaveMode.OnlyToDb,
				ReadMode = ReadMode.FromDb,
				DbAccessMode = DbAccessMode.AdoNet
			}))
			{
				ctx.Command.CommandText = sqliteUpdateScript;
				ctx.ExecInTransactionWithCommit();
			}
			Thread.Sleep(1000);

			MessagePresenter.Instance.WriteLine("Transporting data from SQLite database into File repostitory...");
			StartTransportDataProcess();

			MessagePresenter.Instance.WriteLine("Finished successfully! :)");
			MessagePresenter.Instance.WriteLine("Press any key to exist...");
			Console.ReadKey();
		}


		public static void StartTransportDataProcess()
		{
			var lang = Cultures.CurrentCulture.TwoLetterISOLanguageName;
			var commandArgs = "--From {0} --To FileRepo --Lang {1}".Formatted(C.DB_SQLITE, lang);

			var startInfo = new ProcessStartInfo {
				//WorkingDirectory = Paths.Play_publicPath,
				FileName = AssemblyNames.TransportData_exe,
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
				if(process.ExitCode != 0)
					throw new Exception("Transporting data failed!");
			}
		}

		public static void InitRepo()
		{
			foreach(var dir in DirsToCreate)
			{
				Directory.CreateDirectory(dir);
				MessagePresenter.Instance.WriteLine("  " + dir);
			}
		}

		private static IEnumerable<string> DirsToCreate
		{
			get
			{
				yield return RepoPaths.RootDir;
				yield return RepoPaths.SummariesDir;
				yield return RepoPaths.MonthlySummariesDir;
				yield return RepoPaths.CategorisedSummariesDir;
				yield return RepoPaths.ExpensesAndIncomesDir;
				yield return RepoPaths.DataDir;
				yield return RepoPaths.BackupDir;
			}
		}

		private static string CompressOldData()
		{
			using(var archive = ZipArchive.Create())
			{
				var allFiles = new DirectoryInfo(OldRepoPaths.RootDir).GetFiles("*", SearchOption.AllDirectories);
				// TODO empty directories?
				//var allDirectories = new DirectoryInfo(OldRepoPaths.RootDir).GetDirectories("*", SearchOption.AllDirectories);

				foreach (var fileInfo in allFiles)
				{
					var pathInZip = fileInfo.FullName.Replace(OldRepoPaths.RootDir, "");
					archive.AddEntry(pathInZip, fileInfo);
				}

				using(Stream newStream = File.Create(OldDataCompressed))
				{
					archive.SaveTo(newStream, CompressionType.Deflate);
				}

				return new FileInfo(OldDataCompressed).FullName;
			}
		}

		private static void PromptBackupWarning(string backupMsg)
		{
			MessagePresenter.Instance.WriteLine(Localized.Before_going_further__please_create_a_manual_backup_of_these__);
			MessagePresenter.Instance.WriteLine(backupMsg);
			MessagePresenter.Instance.WriteLine();
			Helpers.ExecuteWithConsoleColor(
				ConsoleColor.Yellow,
				() => MessagePresenter.Instance.WriteLine(Localized.Press_any_key_when_you_are_ready_to_continue_______)
			);
			Console.ReadKey();
			MessagePresenter.Instance.WriteLine();
		}

		private static void UnhadledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
		{
			//if (Debugger.IsAttached)
			//Debugger.Break(); // Stop here while debugging

			var errMsg = "Unhandled Exception occured{0}. ".Formatted(e.IsTerminating ? " (terminating)" : " (NOT terminating)");

			var exception = e.ExceptionObject as Exception;
			if(exception == null)
				ExinLog.ger.LogError(errMsg, e.ExceptionObject);
			else
				ExinLog.ger.LogException(errMsg, exception);

			if(e.IsTerminating)
				Exit(2);
		}

		private static void Exit(int exitCode)
		{
			MessagePresenter.Instance.WriteLine(Localized.Press_any_key_to_exit_____);
			Console.ReadKey();
			Environment.Exit(exitCode);
		}
	}
}
