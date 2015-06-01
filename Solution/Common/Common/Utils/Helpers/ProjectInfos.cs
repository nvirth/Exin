using System;
using System.IO;
using System.Reflection;
using Localization;

namespace Common.Utils.Helpers
{
	public static class ProjectInfos
	{
		public static DirectoryInfo GetSolutionsRootDircetory(string solutionName)
		{
			try
			{
				//var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
				var directoryPath = (new Uri(Assembly.GetExecutingAssembly().CodeBase)).AbsolutePath;
				var directory = new DirectoryInfo(directoryPath).Parent;

				while(directory.Name != solutionName)
				{
					directory = directory.Parent;
				}

				return directory;
			}
			catch (Exception e)
			{
				throw new Exception(Localized.Could_not_find_the_Solution_s_root_directory_, e);
			}
		}
	}
}
