using System;
using System.Collections.Generic;
using Common;
using Common.DbEntities;
using Common.Log;
using Common.UiModels.WPF.DefaultValues;
using DAL.DataBase.Managers.Factory;
using DAL.FileRepo;
using Localization;
using Config = Common.Config.Config;

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
		Category GetByDisplayName(string displayName, bool nullIfNotFound = false);

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
			var categories = FileRepoManager.GetCategories();
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

		public Category GetByDisplayName(string displayName, bool nullIfNotFound = false)
		{
			var category = _cacheFull.Find(c => c.DisplayName.Equals(displayName, StringComparison.InvariantCultureIgnoreCase));
			if(category == null && !nullIfNotFound)
			{
				var msg = string.Format(Localized.Could_not_find_the_Category__DisplayName_0__in_the_database__FORMAT__, displayName);
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
			var existingWithSameName =
				GetByDisplayName(category.DisplayName, nullIfNotFound: true)
				?? GetByDisplayName(category.Name, nullIfNotFound: true)
				?? GetByName(category.DisplayName, nullIfNotFound: true)
				?? GetByName(category.Name, nullIfNotFound: true);
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
		private static readonly ICategoryManager Manager = ManagerFactory.ICategoryManager;

		public static Category GetDefaultCategory => GetCategoryOthers;
		public static Category GetCategoryOthers => Manager.GetByName("Others");
	    public static Category GetCategoryNone => Manager.Get(0);
	    public static Category GetCategoryFullExpenseSummary => Manager.Get(1);
	    public static Category GetCategoryFullIncomeSummary => Manager.Get(2);

		static CategoryManager()
		{
				DefaultValueProvider.Instance.InitDefaultCategory(() => GetDefaultCategory);
		}

	    public static void ClearCache()
		{
			Manager.ClearCache();
		}

		public static void RefreshCache()
		{
			Manager.RefreshCache();
		}

		public static List<Category> GetAll()
		{
			return Manager.GetAll();
		}

		public static List<Category> GetAllValid()
		{
			return Manager.GetAllValid();
		}

		public static Category Get(int ID)
		{
			return Manager.Get(ID);
		}

		public static Category GetByName(string name, bool nullIfNotFound = false)
		{
			return Manager.GetByName(name, nullIfNotFound);
		}

		public static Category GetByDisplayName(string displayName, bool nullIfNotFound = false)
		{
			return Manager.GetByDisplayName(displayName, nullIfNotFound);
		}

		public static void Add(Category category)
		{
			Manager.Add(category);
		}
	}
}