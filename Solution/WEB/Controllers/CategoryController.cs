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
	public class CategoryController : ApiController
	{
		// GET api/<controller>
		public IEnumerable<SelectListItem> Get()
		{
			//var selectedId = id >= 100 ? id : new ExpenseItem().Category.ID;

			var result = CategoryManager.GetAllValid().Select(c => new SelectListItem
			{
				Text = c.DisplayName,
				Value = c.ID.ToString(CultureInfo.InvariantCulture),
				//Selected = c.ID == selectedId,
			});

			return result;
		}
	}
}