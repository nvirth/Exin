using Common.Utils;
using UtilsShared;
using ImportDataInit = ImportDataToDb.ImportData;

namespace UnitTests
{
	public static class Common
	{
		public static void InitClass()
		{
			InitDataBase();
			MessagePresenterManager.WireToConsole();
		}
		public static void InitDataBase()
		{
			ImportDataInit.ImportDataFromFileRepoToDb(); // Imports all data from TestResources 

			// ImportData is a static class, wich meant to use only once per run
			ImportDataInit.Units.Clear();
			ImportDataInit.Categories.Clear();
			ImportDataInit.ExpenseItems.Clear();
		}
	}
}
