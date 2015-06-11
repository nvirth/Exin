using Common.Db;
using DAL.RepoCommon.Interfaces;
using DAL.RepoCommon.Managers;

namespace DAL.RepoCommon
{
	public static class StaticInitializer
	{
		// TODO Are there more?
		public static void InitAllStatic(ICategoryManager categoryManager = null, IUnitManager unitManager = null)
		{
			categoryManager = categoryManager ?? CategoryManager.Instance;
			unitManager = unitManager ?? UnitManager.Instance;

			ManagersRelief.UnitManager.InitDefaultUnit(() => unitManager.GetDefaultUnit);
			ManagersRelief.UnitManager.InitGetByName(unitManager.GetByName);
			ManagersRelief.CategoryManager.InitDefaultCategory(() => categoryManager.GetDefaultCategory);
			ManagersRelief.CategoryManager.InitGetByName(categoryManager.GetByName);

			// These are for AutoMapper initialization
			var useLess1 = categoryManager.GetCategoryNone;
			var useLess2 = unitManager.GetUnitNone;
		}
	}
}
