using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.Configuration;
using Common.Db.Entities;
using Common.Log;
using Common.UiModels.WPF;
using Localization;

namespace DAL.DataBase.Managers
{
	public abstract class SummaryItemManagerDbBase : RepoConfigurableBase, ISummaryItemManagerDb
	{
		protected readonly ICategoryManager CategoryManagerLocal;

		protected SummaryItemManagerDbBase(IRepoConfiguration repoConfiguration, ICategoryManager categoryManager) : base(repoConfiguration)
		{
			CategoryManagerLocal = categoryManager;
		}

		#region CREATE, UPDATE

		public void ReplaceSummary(Summary summary, DateTime date, TransactionItemType transactionItemType)
		{
			try
			{
				if(transactionItemType == TransactionItemType.Income)
					date = new DateTime(date.Year, date.Month, 1);

				InserOrUpdateSummary_Exec(summary, date, transactionItemType);
			}
			catch(Exception e)
			{
				ExinLog.ger.LogException(Localized.Could_not_save_into_database_the_daily_expense_statistics, e);
				throw;
			}
		}

		protected List<SummaryItem> CollectSummariesToInsert(Summary summary, DateTime date, bool isExpense)
		{
			var insertItems = new List<SummaryItem>();
			if(isExpense)
			{
				insertItems.Add(new SummaryItem() {
					Amount = summary.SumOut,
					CategoryID = CategoryManagerLocal.GetCategoryFullExpenseSummary.ID,
					Date = date,
				});

				insertItems.AddRange(
					summary.SumOutWithCategories.Select(
						keyValuePair => {
							var category = keyValuePair.Key;
							var amount = keyValuePair.Value;
							return new SummaryItem() {
								Amount = amount,
								CategoryID = category.ID,
								Date = date,
							};
						})
					);
			}
			else //isIncome
			{
				insertItems.Add(new SummaryItem() {
					Amount = summary.SumIn,
					CategoryID = CategoryManagerLocal.GetCategoryFullIncomeSummary.ID,
					Date = date,
				});
			}
			return insertItems;
		}

		protected abstract void InserOrUpdateSummary_Exec(Summary summary, DateTime date, TransactionItemType transactionItemType);

		#endregion

		#region READ

		public List<SummaryItem> GetInterval(DateTime fromDate, DateTime toDate)
		{
			fromDate = fromDate.Date;
			toDate = toDate.Date;

			return GetInterval_Exec(fromDate, toDate);
		}
		
		public abstract List<SummaryItem> GetAll();
		protected abstract List<SummaryItem> GetInterval_Exec(DateTime fromDate, DateTime toDate);

		#endregion
	}
}