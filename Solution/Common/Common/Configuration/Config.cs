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
		public const int CategoryValidFrom = 100;
		public const int UnitValidFrom = 100;
		public static readonly object DbStringNull = ""; // Other choice: DBNull.Value
		public const string FileExtension = "xml";

		public static readonly DbAccessMode DbAccessMode = EnumHelpers.Parse<DbAccessMode>(ConfigurationManager.AppSettings[C.DbAccessMode], ignoreCase: true);
		public static readonly DbType DbType = EnumHelpers.Parse<DbType>(ConfigurationManager.AppSettings[C.DbType], ignoreCase: true);
		public static readonly ReadMode ReadMode = EnumHelpers.Parse<ReadMode>(ConfigurationManager.AppSettings[C.ReadMode], ignoreCase: true);
		public static readonly SaveMode SaveMode = EnumHelpers.Parse<SaveMode>(ConfigurationManager.AppSettings[C.SaveMode], ignoreCase: true);

		static Config()
		{
			if(ReadMode == ReadMode.FromDb && SaveMode == SaveMode.OnlyToFile)
			{
				string msg = Localized.Configuration_error__can_t_use_ReadMode_FromDb_and_SaveMode_OnlyToFile_together__;
				ExinLog.ger.LogError(msg);
				throw new Exception(msg);
			}
		}
	}
}
