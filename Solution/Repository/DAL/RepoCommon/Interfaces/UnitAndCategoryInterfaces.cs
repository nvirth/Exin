using System.Collections.Generic;
using Common.Db.Entities;

namespace DAL.RepoCommon.Interfaces
{
	public interface IUnitOrCategoryManagerDao<T>
	{
		List<T> GetAll();
		void Add(T item);
	}

	public interface IUnitManagerDao : IUnitOrCategoryManagerDao<Unit>
	{
	}

	public interface ICategoryManagerDao : IUnitOrCategoryManagerDao<Category>
	{
	}

	public interface ICachedManager<T> : IUnitOrCategoryManagerDao<T>
	{
		void RefreshCache();
		void ClearCache();

		List<T> GetAllValid();
		T Get(int ID);
		T GetByName(string name, bool nullIfNotFound = false);
	}

	public interface IUnitManager : ICachedManager<Unit>
	{
		Unit GetDefaultUnit { get; }
		Unit GetUnitPc { get; }
		Unit GetUnitNone { get; }
	}

	public interface ICategoryManager : ICachedManager<Category>
	{
		Category GetDefaultCategory { get; }
		Category GetCategoryOthers { get; }
		Category GetCategoryNone { get; }
		Category GetCategoryFullExpenseSummary { get; }
		Category GetCategoryFullIncomeSummary { get; }
	}
}
