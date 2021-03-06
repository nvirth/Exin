﻿using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using Common.Configuration.Settings;
using Exin.Common.Logging;
using Common.Utils.Helpers;
using Localization;
using C = Common.Configuration.Constants.AppSettingsKeys;

namespace Common.Configuration
{
	public static class Config
	{
#if DEBUG
		private static string _fileRepoDeveloperPath;
		public static string FileRepoDeveloperPath
		{
			get
			{
				if (_fileRepoDeveloperPath == null)
				{
					var solutionRoot = ProjectInfos.GetSolutionsRootDircetory(AppName);
					_fileRepoDeveloperPath = Path.Combine(solutionRoot.FullName, @"Solution\Repository\FileRepoDeveloper");
				}
				return _fileRepoDeveloperPath;
			}
		}
#endif
		public static readonly string AppExecDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
		public static readonly string MainSettingsFilePath = Path.Combine(AppExecDir, @"Config\MainSettings.xml");
		public static readonly string WebDir = Path.Combine(AppExecDir, @"Web\");
		public static readonly string WebChartingDir = Path.Combine(WebDir, @"Charting\");
		public static readonly string WebChartingHtmlPath = Path.Combine(WebChartingDir, @"charting.html");

		public const string AppName = "Exin";
		public const int CategoryValidFrom = 100;
		public const int UnitValidFrom = 100;
		public static readonly object DbStringNull = ""; // Other choice: DBNull.Value
		public const string FileExtension = "xml";

		//--

		public static readonly MainSettings MainSettings = MainSettings.Read(MainSettingsFilePath);
		public static string FirstRepoRootPath => MainSettings.Repositories[0].RootDirAbs;
		public static IRepo Repo; // TODO implement multiple repos...

		public static void InitRepo()
		{
			// We have to delay this init, because of starting problem. If the repo does not exists,
			// does it mean that the user deleted it, or just starts the app the first time?

			Repo = new Repo(FirstRepoRootPath);
		}
	}
}
