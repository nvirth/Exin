using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Common.Configuration;
using Common.Db.Entities;
using DAL.DataBase.Managers;
using DAL.FileRepo.Base;
using C = Common.Configuration.Constants.XmlTags;

namespace DAL.FileRepo
{
	public class CategoryManagerFileRepo : FileRepoManagerBase, ICategoryManagerDao
	{
		public List<Category> GetAll()
		{
			var xmlDoc = XElement.Load(RepoPaths.CategoriesFile);
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
			var xmlDoc = XElement.Load(RepoPaths.CategoriesFile);
			xmlDoc.Add(category.ToXml());
			xmlDoc.Save(RepoPaths.CategoriesFile);
		}
	}
}
