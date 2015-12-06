namespace Common.Configuration.Settings
{
	public interface IRepo
	{
		RepoPaths Paths { get; }
		RepoSettings Settings { get; }
	}

	public class Repo : IRepo
	{
		public RepoPaths Paths { get; private set; }
		public RepoSettings Settings { get; private set; }

		public Repo(string rootDir)
		{
			Paths = new RepoPaths(rootDir);
			Settings = RepoSettings.Read(Paths.RepoSettingsFile);
		}
	}
}