using System;
using System.Collections.Generic;
using System.Linq;
using BLL;
using Common.DbEntities;
using Common.UiModels.WPF;
using Common.UiModels.WPF.Base;
using Common.Utils.Helpers;
using DAL.DataBase.Managers;

namespace WPF.ViewModels
{
	public class StatisticsViewmodel : ChainedCommonBase
	{
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }

		public void SetDateToMonthly(DateTime date)
		{
			StartDate = new DateTime(date.Year, date.Month, 1);
			EndDate = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));

			OnPropertyChanged(this.Property(x => x.IncomesAndExpenses));
		}

		public void Refresh()
		{
			SummaryManager.RefreshCache(StartDate, EndDate);

			OnPropertyChanged(this.Property(x => x.IncomesAndExpenses));
		}

		private Summary Summary
		{
			get { return SummaryManager.GetInterval(StartDate, EndDate); }
		}

		private IEnumerable<KeyValuePair<Category, int>> Incomes
		{
			get
			{
				yield return new KeyValuePair<Category, int>(
					CategoryManager.GetCategoryFullIncomeSummary, Summary.SumIn
					);
			}
		}

		private IEnumerable<KeyValuePair<Category, int>> Expenses
		{
			get
			{
				yield return new KeyValuePair<Category, int>(
					CategoryManager.GetCategoryFullExpenseSummary, Summary.SumOut
					);

				foreach(var category in CategoryManager.GetAllValid())
				{
					int sum = Summary.SumOutWithCategories.ContainsKey(category)
						? Summary.SumOutWithCategories[category]
						: 0;

					yield return new KeyValuePair<Category, int>(category, sum);
				}
			}
		}

		public IEnumerable<KeyValuePair<Category, int>> IncomesAndExpenses
		{
			get
			{
				var res = Incomes.Concat(Expenses);
				return res;
			}
		}

	}
}
