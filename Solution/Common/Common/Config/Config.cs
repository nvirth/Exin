using System;
using System.Configuration;
using Common.Log;
using Localization;

namespace Common.Config
{
	public static class Config
	{
		public const string SqliteDbFullpathPlaceholder = "#SQLITE_REPO_FULLPATH#";	// In config files
		public const int CategoryValidFrom = 100;
		public const int UnitValidFrom = 100;
		public static readonly object StringNull = ""; // Other choice: DBNull.Value
		public const string FileExtension = "xml";

		public static readonly DbAccessMode DbAccessMode = (DbAccessMode)Enum.Parse(typeof(DbAccessMode), ConfigurationManager.AppSettings["DbAccessMode"], ignoreCase: true);
		public static readonly DbType DbType = (DbType)Enum.Parse(typeof(DbType), ConfigurationManager.AppSettings["DbType"], ignoreCase: true);
		public static readonly ReadMode ReadMode = (ReadMode)Enum.Parse(typeof(ReadMode), ConfigurationManager.AppSettings["ReadMode"], ignoreCase: true);
		public static readonly SaveMode SaveMode = (SaveMode)Enum.Parse(typeof(SaveMode), ConfigurationManager.AppSettings["SaveMode"], ignoreCase: true);

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
