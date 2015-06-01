using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using Common.UiModels.WPF;
using DAL.DataBase.Managers;
using UtilsShared;

namespace WEB.Controllers
{
	public class UnitController : ApiController
	{
		// GET api/<controller>
		public IEnumerable<SelectListItem> Get()
		{
			var result = UnitManager.GetAllValid().Select(c => new SelectListItem
			{
				Text = c.DisplayName,
				Value = c.ID.ToString(CultureInfo.InvariantCulture),
			});

			return result;
		}
	}
}