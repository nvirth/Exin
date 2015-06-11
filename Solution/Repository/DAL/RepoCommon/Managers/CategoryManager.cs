using Common.Configuration;
using Common.Db;
using Common.Db.Entities;
using DAL.DataBase.Managers.Factory;
using DAL.RepoCommon.Interfaces;
using DAL.RepoCommon.Managers.Base;
using Localization;
using C = Common.Configuration.Constants.Resources.DefaultCategories;

namespace DAL.RepoCommon.Managers
{
	public class CategoryManager : CachedManagerBase<Category>, ICategoryManager
	{
		public static readonly ICategoryManager Instance = new CategoryManager();
		private readonly ICategoryManagerDao _core;

		protected override int ValidFrom => Config.CategoryValidFrom;
		protected override string LocalizedTypeName => Localized.Category;
		protected override string LocalizedTypeNameLowercase => Localized.Category_lowercase;

		public CategoryManager(IRepoConfiguration repoConfiguration = null) : base(repoConfiguration)
		{
			_core = new ManagerDaoFactory(repoConfiguration).CategoryManager;
		}

		// -- ICategoryManager implementation

		public Category GetDefaultCategory => GetCategoryOthers;
		public Category GetCategoryOthers => GetByName(C.Others);
		public Category GetCategoryNone => GetByName(C.None);
		public Category GetCategoryFullExpenseSummary => GetByName(C.FullExpenseSummary);
		public Category GetCategoryFullIncomeSummary => GetByName(C.FullIncomeSummary);

		public override void RefreshCache()
		{
			var Categorys = _core.GetAll();
			RefreshCache(Categorys);
		}

		public override void Add(Category Category)
		{
			CheckExistsInCache(Category);
			_core.Add(Category);
			AddToCache(Category);
		}

		// --

		//TODO static CategoryManager()
		static CategoryManager()
		{
			InitManagerRelief();
		}

		public static void InitManagerRelief()
		{
			ManagersRelief.CategoryManager.InitDefaultCategory(() => Instance.GetDefaultCategory);
			ManagersRelief.CategoryManager.InitGetByName(Instance.GetByName);
		}
	}
}
