using DAL.RepoCommon.Managers;

namespace DAL.RepoCommon
{
	public class StaticInitializer
	{
		// TODO Are there more?
		// TODO this should take an IRepoConfiguration
		public static void InitAllStatic()
		{
			CategoryManager.InitManagerRelief();
			UnitManager.InitManagerRelief();
		}
	}
}
