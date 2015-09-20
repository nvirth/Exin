using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Common.Configuration;
using Common.Db.Entities;
using DAL.DataBase.Managers;
using DAL.FileRepo.Managers.Base;
using DAL.RepoCommon;
using DAL.RepoCommon.Interfaces;
using C = Common.Configuration.Constants.Xml.TransactionItem;

namespace DAL.FileRepo.Managers
{
	public class CategoryManagerFileRepo : FileRepoManagerBase, ICategoryManagerDao
	{
		public List<Category> GetAll()
		{
			var xmlDoc = XElement.Load(Config.Repo.Paths.CategoriesFile);
			var categories = xmlDoc.Elements(C.Category).Select(xml => new Category
			{
				ID = ((int)xml.Element(C.ID)),
				Name = ((string)xml.Element(C.Name)).Trim(),
				DisplayNames = xml.ParseLocalizedDisplayNames(),
			}).ToList();
			return categories;
		}

		public void Add(Category category)
		{
			var xmlDoc = XElement.Load(Config.Repo.Paths.CategoriesFile);
			xmlDoc.Add(category.ToXml());
			xmlDoc.Save(Config.Repo.Paths.CategoriesFile);
		}
	}
}
