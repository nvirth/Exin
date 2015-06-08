using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Transactions;
using Common;
using Common.Db.Entities;
using Common.Log;
using Common.UiModels.WPF;
using Common.Utils.Helpers;
using DAL.DataBase.Managers;
using Localization;
using Config = Common.Configuration.Config;
using DbType = Common.DbType;

namespace DAL.DataBase.AdoNet.Managers
{
	public static class SummaryItemManagerAdoNetFactory
	{
		public static SummaryItemManagerAdoNetBase Create()
		{
			switch(Config.DbType)
			{
				case DbType.MsSql:
					return new SummaryItemManagerAdoNetMsSql();

				case DbType.SQLite:
					return new SummaryItemManagerAdoNetSQLite();

				default:
					throw new NotImplementedException(string.Format(Localized.SummaryItemManagerAdoNetBase_is_not_implemented_for_this_DbType__FORMAT__, Config.DbType));
			}

		}
	}

	public abstract class SummaryItemManagerAdoNetBase : SummaryItemManagerCommonBase
	{
		public const string TableName = "SummaryItem";

		#region READ

		public override List<SummaryItem> GetAll()
		{
			using(var ctx = ExinAdoNetContextFactory.Create())
			{
				ctx.Command.CommandText = BuildGetAllQuery();

				ctx.Adapter.SelectCommand = ctx.Command;
				ctx.Adapter.Fill(ctx.DataSet);

				var transactionItems = FetchItems(ctx.DataSet.Tables[0]);
				return transactionItems;
			}
		}

		public override List<SummaryItem> GetInterval_Exec(DateTime fromDate, DateTime toDate)
		{
			using(var ctx = ExinAdoNetContextFactory.Create())
			{
				ctx.Command.CommandText = BuildGetIntervalQuery();

				ctx.Command.Parameters.AddWithValue("@fromDate", fromDate);
				ctx.Command.Parameters.AddWithValue("@toDate", toDate);
				ctx.Adapter.SelectCommand = ctx.Command;
				ctx.Adapter.Fill(ctx.DataSet);

				var transactionItems = FetchItems(ctx.DataSet.Tables[0]);
				return transactionItems;
			}
		}

		#region Build Queries

		// These are general queries

		protected virtual string BuildGetAllQuery()
		{
			return "SELECT * FROM " + TableName;
		}

		protected virtual string BuildGetIntervalQuery()
		{
			return "SELECT * FROM " + TableName + @"
										   WHERE date >= @fromDate 
										   AND date <= @toDate ";
		}

		#endregion

		#region Fetch (from DataTable, DataRow)

		private List<SummaryItem> FetchItems(DataTable dataTable)
		{
			var transactionItems = dataTable.AsEnumerable().Select(FetchItem).ToList();
			return transactionItems;
		}

		private SummaryItem FetchItem(DataRow dataRow)
		{
			var summaryItem = new SummaryItem();

			summaryItem.ID = Convert.ToInt32(dataRow[summaryItem.Property(y => y.ID)]);
			summaryItem.Date = dataRow.Field<DateTime>(summaryItem.Property(x => x.Date));
			summaryItem.CategoryID = Convert.ToInt32(dataRow[summaryItem.Property(y => y.CategoryID)]);
			summaryItem.Category = CategoryManager.Instance.Get(summaryItem.CategoryID);
			summaryItem.Amount = Convert.ToInt32(dataRow[summaryItem.Property(y => y.Amount)]);

			return summaryItem;
		}

		#endregion

		#endregion


		#region Helpers

		/// <summary>
		/// Appends in this format ( after "VALUES" ): "(Amount, CategoryID, Date),"
		/// Do not forget to remove the last semicolon :)
		/// </summary>
		protected void AppendSingleValueToQuery(StringBuilder stringBuilder, int amount, int categoryId, DateTime date)
		{
			stringBuilder.Append("(");
			stringBuilder.Append(amount).Append(",");
			stringBuilder.Append(categoryId).Append(",");
			stringBuilder.AppendDateToQuery(date);
			stringBuilder.Append("),");
		}

		/// <summary>
		/// Appends in this format ( after "VALUES" ): "(Amount, CategoryID, Date),"
		/// </summary>
		protected void AppendAllValuesToQuery(StringBuilder stringBuilder, Summary summary, DateTime date, bool isExpense)
		{
			var summaries = CollectSummariesToInsert(summary, date, isExpense);
			summaries.ForEach(si => AppendSingleValueToQuery(stringBuilder, si.Amount, si.CategoryID, si.Date));
			stringBuilder.Remove(stringBuilder.Length - 1, 1); // Remove last semicilon ','
		}

		#endregion
	}

	public class SummaryItemManagerAdoNetMsSql : SummaryItemManagerAdoNetBase
	{
		protected override void InserOrUpdateSummary_Exec(Summary summary, DateTime date, TransactionItemType transactionItemType)
		{
			var query = BuildMergeQuery(summary, date, transactionItemType);

			using(var ctx = ExinAdoNetContextFactory.Create())
			using(var transactionScope = new TransactionScope())
			{
				try
				{
					ctx.Command.CommandText = query;
					ctx.Command.ExecuteNonQuery();
					transactionScope.Complete();
				}
				catch(Exception e)
				{
					ExinLog.ger.LogException(Localized.SummaryItem_InsertOrUpdate_failed__MS_SQL_, e, query);
					throw;
				}
			}
		}

		private string BuildMergeQuery(Summary summary, DateTime date, TransactionItemType transactionItemType)
		{
			var isExpense = transactionItemType == TransactionItemType.Expense;
			var stringBuilder = new StringBuilder();

			stringBuilder.Append(@"
					MERGE INTO [dbo].[SummaryItem] as [target]
					USING
					(
						VALUES ");

			AppendAllValuesToQuery(stringBuilder, summary, date, isExpense);

			// "USING ( ..." + " ) as [source]..." // <-- We are at '+'
			stringBuilder.Append(@"
					) as [source]([Amount],[CategoryID],[Date])
					ON [target].[Date] = [source].[Date] 
						AND [target].[CategoryID] = [source].[CategoryID]
					WHEN MATCHED THEN
						UPDATE
						SET [Amount] = [source].[Amount],
							[CategoryID] = [source].[CategoryID],
							[Date] = [source].[Date]
					WHEN NOT MATCHED BY TARGET THEN
						INSERT ([Amount],[CategoryID],[Date])
						VALUES ([source].[Amount],[source].[CategoryID],[source].[Date]) ");

			if(isExpense) // Delete another expense summaries from this day
			{
				stringBuilder.Append(@"
					WHEN NOT MATCHED BY SOURCE 
						AND [target].[Date] = ").AppendDateToQuery(date).Append(@"
						AND [target].[CategoryID] != ").Append(CategoryManager.GetCategoryFullIncomeSummary.ID)
					.Append(" THEN DELETE ");
			}
			stringBuilder.Append(";");

			return stringBuilder.ToString();
		}
	}

	public class SummaryItemManagerAdoNetSQLite : SummaryItemManagerAdoNetBase
	{
		protected override void InserOrUpdateSummary_Exec(Summary summary, DateTime date, TransactionItemType transactionItemType)
		{
			var isExpense = transactionItemType == TransactionItemType.Expense;
			var stringBuilder = new StringBuilder();

			using(var ctx = ExinAdoNetContextFactory.Create())
			{
				try
				{
					// -- Delete all summaries from this day

					stringBuilder.Append(" DELETE FROM ").Append(TableName)
						.Append(" WHERE date = ").AppendDateToQuery(date)
						.Append(" AND CategoryID ").Append(isExpense ? "!=" : "=")
						.Append(CategoryManager.GetCategoryFullIncomeSummary.ID) // isExpense != 2; isIncome = 2
						.Append(";");

					// -- Insert new data

					stringBuilder.Append(" INSERT INTO ").Append(TableName)
						.Append("([Amount],[CategoryID],[Date])")
						.Append(" VALUES ");

					AppendAllValuesToQuery(stringBuilder, summary, date, isExpense);

					ctx.Command.CommandText = stringBuilder.ToString();

					ctx.ExecInTransactionWithCommit();
				}
				catch(Exception e)
				{
					ExinLog.ger.LogException(Localized.SummaryItem_InsertOrUpdate_failed_SQLite_, e, stringBuilder.ToString());
					throw;
				}
			}
		}
	}
}
