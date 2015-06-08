using System;
using System.Collections.Generic;
using Common;
using Common.Db;
using Common.Db.Entities;
using Common.Log;
using DAL.DataBase.Managers.Factory;
using DAL.FileRepo;
using Localization;
using Config = Common.Configuration.Config;
using C = Common.Configuration.Constants.Resources.DefaultCategories;

namespace DAL.DataBase.Managers
{
	public interface ICategoryManager
	{
		void RefreshCache();
		void ClearCache();

		List<Category> GetAll();
		List<Category> GetAllValid();
		Category Get(int ID);
		Category GetByName(string name, bool nullIfNotFound = false);

		void Add(Category category);
	}

	public abstract class CategoryManagerCommonBase : ICategoryManager
	{
		protected CategoryManagerCommonBase()
		{
			RefreshCache();
		}

		#region Cache

		private readonly List<Category> _cacheFull = new List<Category>();
		private readonly List<Category> _cacheValid = new List<Category>();

		public void ClearCache()
		{
			_cacheValid.Clear();
			_cacheFull.Clear();
		}

		public void RefreshCache()
		{
			switch(Config.ReadMode)
			{
				case ReadMode.FromFile:
					RefreshCache_FromFile();
					break;
				case ReadMode.FromDb:
					RefreshCache_FromDb();
					break;
				default:
					var msg = Localized.CategoryManagerCommonBase_RefreshCache_is_not_implemented_for_ReadMode__ + Config.ReadMode;
					ExinLog.ger.LogError(msg);
					throw new NotImplementedException(msg);
			}
		}

		private void RefreshCache_FromFile()
		{
			var categories = FileRepoManager.Instance.GetCategories();
			RefreshCache_Refresh(categories);
		}

		protected void RefreshCache_Refresh(IEnumerable<Category> categories)
		{
			ClearCache();

			foreach(var c in categories)
			{
				_cacheFull.Add(c);
				if(c.ID >= Config.CategoryValidFrom)
					_cacheValid.Add(c);
			}
		}

		protected abstract void RefreshCache_FromDb();

		#endregion

		#region READ

		public List<Category> GetAll()
		{
			if(_cacheFull != null && _cacheFull.Count != 0)
				return _cacheFull;

			RefreshCache();
			return _cacheFull;
		}

		public List<Category> GetAllValid()
		{
			if(_cacheValid != null && _cacheValid.Count != 0)
				return _cacheValid;

			RefreshCache();
			return _cacheValid;
		}

		public Category Get(int ID)
		{
			var category = _cacheFull.Find(c => c.ID == ID);
			if(category == null)
			{
				var msg = string.Format(Localized.Could_not_find_the_Category__ID_0__in_the_database__FORMAT__, ID);
				var e = new Exception(msg);
				ExinLog.ger.LogException(msg, e);
				throw e;
			}

			return category;
		}

		public Category GetByName(string name, bool nullIfNotFound = false)
		{
			var category = _cacheFull.Find(c => c.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
			if(category == null && !nullIfNotFound)
			{
				var msg = string.Format(Localized.Could_not_find_the_Category__Name_0__in_the_database__FORMAT__, name);
				var e = new Exception(msg);
				ExinLog.ger.LogException(msg, e);
				throw e;
			}

			return category;
		}

		#endregion

		#region CREATE

		public abstract void Add(Category category);

		protected void CheckExistsInCache(Category category)
		{
			var existingWithSameName = GetByName(category.Name, nullIfNotFound: true);
			if(existingWithSameName != null)
			{
				string msg = Localized.Category_already_exists_with_the_specified_name;
				var e = new Exception(msg);
				ExinLog.ger.LogException(msg, e);
				throw e;
			}
		}

		protected void AddToCache(Category category)
		{
			_cacheFull.Add(category);
			if(category.ID >= Config.CategoryValidFrom)
				_cacheValid.Add(category);
		}

		#endregion
	}
	
	public static class CategoryManager
	{
		public static readonly ICategoryManager Instance = ManagerFactory.ICategoryManager;

		public static Category GetDefaultCategory => GetCategoryOthers;
		public static Category GetCategoryOthers => Instance.GetByName(C.Others);
	    public static Category GetCategoryNone => Instance.GetByName(C.None);
	    public static Category GetCategoryFullExpenseSummary => Instance.GetByName(C.FullExpenseSummary);
	    public static Category GetCategoryFullIncomeSummary => Instance.GetByName(C.FullIncomeSummary);

		static CategoryManager()
		{
				ManagersRelief.CategoryManager.InitDefaultCategory(() => GetDefaultCategory);
				ManagersRelief.CategoryManager.InitGetByName(Instance.GetByName);
		}
	}
}