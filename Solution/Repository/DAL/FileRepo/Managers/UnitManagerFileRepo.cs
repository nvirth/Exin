using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Common.Configuration;
using Common.Db.Entities;
using DAL.DataBase.Managers;
using DAL.FileRepo.Managers.Base;
using DAL.RepoCommon;
using DAL.RepoCommon.Interfaces;
using C = Common.Configuration.Constants.XmlTags;

namespace DAL.FileRepo.Managers
{
	public class UnitManagerFileRepo : FileRepoManagerBase, IUnitManagerDao
	{
		public List<Unit> GetAll()
		{
			var xmlDoc = XElement.Load(RepoPaths.UnitsFile);
			var units = xmlDoc.Elements(C.Unit).Select(xml => new Unit
			{
				ID = ((int) xml.Element(C.ID)),
				Name = ((string) xml.Element(C.Name)).Trim(),
				DisplayNames = xml.ParseLocalizedDisplayNames(),
			}).ToList();
			return units;
		}

		public void Add(Unit unit)
		{
			var xmlDoc = XElement.Load(RepoPaths.UnitsFile);
			xmlDoc.Add(unit.ToXml());
			xmlDoc.Save(RepoPaths.UnitsFile);
		}
	}
}
