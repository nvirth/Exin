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
#if DEBUG
		public const string FileRepoDeveloperPath = @"..\..\..\Repository\FileRepoDeveloper";
#endif
		public const string MainSettingsFilePath = @".\Config\MainSettings.xml";
		public const string AppName = "Exin";
		public const int CategoryValidFrom = 100;
		public const int UnitValidFrom = 100;
		public static readonly object DbStringNull = ""; // Other choice: DBNull.Value
		public const string FileExtension = "xml";

		//--

		public static readonly MainSettings MainSettings = MainSettings.Read(MainSettingsFilePath);
		public static readonly IRepo Repo = new Repo(MainSettings.Repositories[0].RootDir); // TODO implement multiple repos...
	}
}
