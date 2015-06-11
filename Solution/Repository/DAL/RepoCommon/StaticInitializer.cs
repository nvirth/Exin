using Common.Db;
using Common.Db.Entities;
using DAL.DataBase.EntityFramework;
using DAL.DataBase.EntityFramework.EntitiesMsSql;
using DAL.DataBase.EntityFramework.EntitiesSqlite;
using DAL.RepoCommon.Interfaces;
using DAL.RepoCommon.Managers;

namespace DAL.RepoCommon
{
	public static class StaticInitializer
	{
		/// <summary>
		/// Makes static class' initialization (more) determinable
		/// </summary>
		public static void InitAllStatic(ICategoryManager categoryManager = null, IUnitManager unitManager = null)
		{
			categoryManager = categoryManager ?? CategoryManager.Instance;
			unitManager = unitManager ?? UnitManager.Instance;

			ManagersRelief.UnitManager.InitDefaultUnit(() => unitManager.GetDefaultUnit);
			ManagersRelief.UnitManager.InitGetByName(unitManager.GetByName);
			ManagersRelief.CategoryManager.InitDefaultCategory(() => categoryManager.GetDefaultCategory);
			ManagersRelief.CategoryManager.InitGetByName(categoryManager.GetByName);

			// These are for AutoMapper initialization
			InitAutoMapperForEf.Init<Category_Sqlite, Category>();
			InitAutoMapperForEf.Init<Category, Category_Sqlite>();
			InitAutoMapperForEf.Init<SummaryItem_Sqlite, SummaryItem>();
			InitAutoMapperForEf.Init<SummaryItem, SummaryItem_Sqlite>();
			InitAutoMapperForEf.Init<TransactionItem_Sqlite, TransactionItem>();
			InitAutoMapperForEf.Init<TransactionItem, TransactionItem_Sqlite>();
			InitAutoMapperForEf.Init<Unit_Sqlite, Unit>();
			InitAutoMapperForEf.Init<Unit, Unit_Sqlite>();

			InitAutoMapperForEf.Init<Category_MsSql, Category>();
			InitAutoMapperForEf.Init<Category, Category_MsSql>();
			InitAutoMapperForEf.Init<SummaryItem_MsSql, SummaryItem>();
			InitAutoMapperForEf.Init<SummaryItem, SummaryItem_MsSql>();
			InitAutoMapperForEf.Init<TransactionItem_MsSql, TransactionItem>();
			InitAutoMapperForEf.Init<TransactionItem, TransactionItem_MsSql>();
			InitAutoMapperForEf.Init<Unit_MsSql, Unit>();
			InitAutoMapperForEf.Init<Unit, Unit_MsSql>();
		}
	}
}
