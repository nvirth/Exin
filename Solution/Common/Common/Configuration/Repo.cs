using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using Common.Utils;
using Common.Utils.Helpers;
using Localization;
using C = Common.Configuration.Constants;

namespace Common.Configuration
{
	public interface IRepo
	{
		RepoPaths Paths { get; }
		RepoSettings Settings { get; }
	}

	public class Repo : RepoConfiguration, IRepo
	{
		public RepoPaths Paths { get; private set; }
		public RepoSettings Settings { get; private set; }

		public Repo(string rootDir)
		{
			Paths = new RepoPaths(rootDir);
			Settings = new RepoSettings(Paths);
		}
	}
}