using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Helpers;
using System.Web.Mvc;
using BLL.WebManagers;
using Common;
using Common.Log;
using Common.UiModels.WEB;
using Common.UiModels.WPF;
using Common.Utils.Helpers;
using DAL;
using DAL.DataBase.Managers;
using UtilsLocal.Validation;
using UtilsShared;

namespace WEB.Controllers
{
	public class HomeController : Controller
	{
		[HttpGet]
		public ActionResult MonthlySummariesChart(DateTime? date)
		{
			var dateLocal = date.HasValue ? date.Value : DateTime.Today;
			Summary summary;
			WebManager.GetMonthlySummaries(dateLocal, out summary, writeOutSummaeries: false);

			return BuildChartImage(summary);
		}

		private ActionResult BuildChartImage(Summary summary)
		{
			var xValues = new List<string>();
			var yValuesOut = new List<int>();

			xValues.Add("Összes");
			yValuesOut.Add(summary.SumOut);

			foreach(var category in CategoryManager.GetAllValid())
			{
				xValues.Add(category.DisplayName);
				yValuesOut.Add(
					summary.SumOutWithCategories.ContainsKey(category)
					? summary.SumOutWithCategories[category]
					: 0);
			}

			var yValuesIn = new int[yValuesOut.Count];
			yValuesIn[0] = summary.SumIn;

			const string transparentBgTheme = @"
				<Chart BackColor=""Transparent"" >
					<ChartAreas>
						<ChartArea Name=""Default"" BackColor=""Transparent""></ChartArea>
					</ChartAreas>
				</Chart>";

			var myChart = new Chart(width: 800, height: 600, theme: transparentBgTheme)
				.AddTitle("Összesítő")
				.AddSeries(xValue: xValues, yValues: yValuesIn, name: "Bevételek")
				.AddSeries(xValue: xValues, yValues: yValuesOut, name: "Kiadások")
				.AddLegend()
				.GetBytes("png");

			return File(myChart, "image/png");
		}

		//[HttpGet]
		//public ActionResult MonthlySummariesChart(DateTime? date)
		//{
		//	var dateLocal = date.HasValue ? date.Value : DateTime.Today;
		//	Summary summary;
		//	WebManager.GetMonthlySummaries(dateLocal, out summary, writeOutSummaeries: false);

		//	return View(summary);
		//}

		[HttpGet]
		public ActionResult Index()
		{
			return RedirectToActionPermanent("DailyExpenses");
		}

		#region DailyExpenses, MonthlyIncomes --- Transactional sections

		[HttpGet]
		public ActionResult DailyExpenses(DateTime? date)
		{
			var dateLocal = date.HasValue ? date.Value : DateTime.Today;
			var transactionItems = GetTransactionItems(dateLocal, TransactionItemType.Expense);

			return TransactionItemView(dateLocal, transactionItems, isExpense: true);
		}

		[HttpGet]
		public ActionResult MonthlyIncomes(DateTime? date)
		{
			var dateLocal = date.HasValue ? date.Value : DateTime.Today;
			var transactionItems = GetTransactionItems(dateLocal, TransactionItemType.Income);

			return TransactionItemView(dateLocal, transactionItems, isExpense: false);
		}

		[HttpPost]
		public ActionResult DailyExpenses(List<TransactionItemVM> transactionItemVms, DateTime dateFromHidden)
		{
			return TransactionItemPost(transactionItemVms, dateFromHidden, TransactionItemType.Expense);
		}

		[HttpPost]
		public ActionResult MonthlyIncomes(List<TransactionItemVM> transactionItemVms, DateTime dateFromHidden)
		{
			return TransactionItemPost(transactionItemVms, dateFromHidden, TransactionItemType.Income);
		}

		#region Helpers

		private List<TransactionItemVM> GetTransactionItems(DateTime dateLocal, TransactionItemType type)
		{
			var transactionItems = WebManager.GetTransactionItems(dateLocal, type);

			if(!transactionItems.Any())
				transactionItems.Add(new TransactionItemVM()
				{
					Type = type,
					Date = dateLocal,
					UnitID = UnitManager.GetUnitDb.ID,
					CategoryID = CategoryManager.GetCategoryOthers.ID,
				});

			return transactionItems;
		}

		private ActionResult TransactionItemPost(List<TransactionItemVM> transactionItemVms, DateTime dateFromHidden, TransactionItemType transactionItemType)
		{
			const string errorMessage = "Nem sikerült menteni a változásokat. ";

			bool isExpense = transactionItemType == TransactionItemType.Expense;
			if(ModelState.IsValid)
			{
				if(!new DateUpToToday().IsValid(dateFromHidden))
				{
					ModelState.AddModelError("dateFromHidden", "Jövőbeni dátum nem választható. ");
					return TransactionItemView(dateFromHidden, transactionItemVms, isExpense);
				}

				try
				{
					WebManager.SaveTransactionItems(transactionItemVms, dateFromHidden, transactionItemType);

					ViewBag.Success = "Változások sikeresen elmentve! ";
					return TransactionItemView(dateFromHidden, transactionItemVms, isExpense);
				}
				catch(Exception e)
				{
					ExinLog.ger.LogException(errorMessage, e, transactionItemVms);
					ModelState.AddModelError("", errorMessage);
					return TransactionItemView(dateFromHidden, transactionItemVms, isExpense);
				}
			}

			var logData = transactionItemVms.Concat<object>(ModelState.CopyModelErrors());
			ExinLog.ger.LogError(errorMessage + " - ModelState invalid", logData);
			ModelState.AddModelError("", errorMessage);
			return TransactionItemView(dateFromHidden, transactionItemVms, isExpense);
		}

		private ActionResult TransactionItemView(DateTime date, List<TransactionItemVM> transactionItems, bool isExpense)
		{
			var siteTitle = isExpense ? "Napi kiadások" : "Havi bevételek";
			var actionName = isExpense ? "DailyExpenses" : "MonthlyIncomes";
			return TransactionItemView(transactionItems, date, siteTitle, actionName);
		}

		private ActionResult TransactionItemView(List<TransactionItemVM> transactionItems, DateTime date, string siteTitle, string actionName)
		{
			ViewBag.Date = date;
			ViewBag.SiteTitle = siteTitle;
			ViewBag.ActionName = actionName;
			return View(transactionItems); // DailyExpenses || MonthlyIncomes || MonthlySummaries
		}

		#endregion

		#endregion

		[HttpGet]
		public ActionResult MonthlySummaries(DateTime? date)
		{
			var dateLocal = date.HasValue ? date.Value : DateTime.Today;
			var transactionItems = WebManager.GetMonthlySummaries(dateLocal);

			return TransactionItemView(transactionItems, dateLocal, "Havi összesítések", "MonthlySummaries");
		}

	}
}
