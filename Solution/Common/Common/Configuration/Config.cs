using System;
using System.Configuration;
using Common.Log;
using Common.Utils.Helpers;
using Localization;
using C = Common.Configuration.Constants.AppSettingsKeys;

namespace Common.Configuration
{
	public static class Config
	{
		public const string AppName = "Exin";
		public const string MainSettingsFilePath = @".\Config\MainSettings.xml";
#if DEBUG
		public const string FileRepoDeveloperPath = @"..\..\..\Repository\FileRepoDeveloper";
#endif

		public const int CategoryValidFrom = 100;
		public const int UnitValidFrom = 100;
		public static readonly object DbStringNull = ""; // Other choice: DBNull.Value
		public const string FileExtension = "xml";

		//--

		public static MainSettings MainSettings = MainSettings.Read(MainSettingsFilePath);

		public static IRepoConfiguration Repo { get; } = new RepoConfiguration()
		{
			DbAccessMode = EnumHelpers.Parse<DbAccessMode>(ConfigurationManager.AppSettings[C.DbAccessMode], ignoreCase: true),
			DbType = EnumHelpers.Parse<DbType>(ConfigurationManager.AppSettings[C.DbType], ignoreCase: true),
			ReadMode = EnumHelpers.Parse<ReadMode>(ConfigurationManager.AppSettings[C.ReadMode], ignoreCase: true),
			SaveMode = EnumHelpers.Parse<SaveMode>(ConfigurationManager.AppSettings[C.SaveMode], ignoreCase: true),
		};

		static Config()
		{
			// TODO it would be better if only SaveMode.FileAndDb could be used in ExinWPF
			CheckConfiguration(Repo);
		}

		public static void CheckConfiguration(IRepoConfiguration repoConfiguration)
		{
			if(repoConfiguration.ReadMode == ReadMode.FromDb && repoConfiguration.SaveMode == SaveMode.OnlyToFile)
			{
				string msg = Localized.Configuration_error__can_t_use_ReadMode_FromDb_and_SaveMode_OnlyToFile_together__;
				ExinLog.ger.LogError(msg);
				throw new Exception(msg);
			}
			if(repoConfiguration.ReadMode == ReadMode.FromFile && repoConfiguration.SaveMode == SaveMode.OnlyToDb)
			{
				string msg = Localized.Configuration_error__can_t_use_ReadMode_FromFile_and_SaveMode_OnlyToDb_together__;
				ExinLog.ger.LogError(msg);
				throw new Exception(msg);
			}
		}
	}
}
