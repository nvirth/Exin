using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using Common.Log;
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
		public static readonly string MainSettingsFilePath = Path.Combine(AppExecDir, @".\Config\MainSettings.xml");
		public const string AppName = "Exin";
		public const int CategoryValidFrom = 100;
		public const int UnitValidFrom = 100;
		public static readonly object DbStringNull = ""; // Other choice: DBNull.Value
		public const string FileExtension = "xml";

		//--

		public static readonly MainSettings MainSettings = MainSettings.Read(MainSettingsFilePath);
		public static string FirstRepoRootPath => MainSettings.Repositories[0].RootDir;
		public static IRepo Repo; // TODO implement multiple repos...

		public static void InitRepo()
		{
			// We have to delay this init, because of starting problem. If the repo not exists,
			// does it mean that the user deleted it, or just starts the app the first time?

			Repo = new Repo(FirstRepoRootPath);
		}
	}
}
