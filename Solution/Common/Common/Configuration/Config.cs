using System;
using System.Configuration;
using Common.Log;
using Common.Utils.Helpers;
using Localization;
using C = Common.Configuration.Constants.AppSettingsKeys;

namespace Common.Configuration
{
	public interface IRepoConfiguration
	{
		DbType DbType { get; }
		DbAccessMode DbAccessMode { get; }
		ReadMode ReadMode { get; }
		SaveMode SaveMode { get; }
	}

	public class RepoConfiguration : IRepoConfiguration
	{
		public DbType DbType { get; set; } = 0;
		public DbAccessMode DbAccessMode { get; set; } = 0;
		public ReadMode ReadMode { get; set; } = 0;
		public SaveMode SaveMode { get; set; } = 0;
	}

	public static class Config
	{
		public const int CategoryValidFrom = 100;
		public const int UnitValidFrom = 100;
		public static readonly object DbStringNull = ""; // Other choice: DBNull.Value
		public const string FileExtension = "xml";

		public static IRepoConfiguration Repo { get; } = new RepoConfiguration()
		{
			DbAccessMode = EnumHelpers.Parse<DbAccessMode>(ConfigurationManager.AppSettings[C.DbAccessMode], ignoreCase: true),
			DbType = EnumHelpers.Parse<DbType>(ConfigurationManager.AppSettings[C.DbType], ignoreCase: true),
			ReadMode = EnumHelpers.Parse<ReadMode>(ConfigurationManager.AppSettings[C.ReadMode], ignoreCase: true),
			SaveMode = EnumHelpers.Parse<SaveMode>(ConfigurationManager.AppSettings[C.SaveMode], ignoreCase: true),
		};

		static Config()
		{
			if(Repo.ReadMode == ReadMode.FromDb && Repo.SaveMode == SaveMode.OnlyToFile)
			{
				string msg = Localized.Configuration_error__can_t_use_ReadMode_FromDb_and_SaveMode_OnlyToFile_together__;
				ExinLog.ger.LogError(msg);
				throw new Exception(msg);
			}
		}
	}
}
