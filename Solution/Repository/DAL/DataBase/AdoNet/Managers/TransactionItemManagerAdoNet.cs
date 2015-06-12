using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Transactions;
using Common;
using Common.Configuration;
using Common.Db.Entities;
using Common.Log;
using Common.Utils.Helpers;
using DAL.DataBase.Managers;
using DAL.DataBase.Managers.Base;
using DAL.RepoCommon;
using DAL.RepoCommon.Interfaces;
using Localization;
using Config = Common.Configuration.Config;
using DbType = Common.DbType;

namespace DAL.DataBase.AdoNet.Managers
{
	public static class TransactionItemManagerAdoNetFactory
	{
		public static TransactionItemManagerAdoNetBase Create(IRepoConfiguration repoConfiguration,
			ICategoryManager categoryManager, IUnitManager unitManager)
		{
			switch(repoConfiguration.DbType)
			{
				case DbType.MsSql:
					return new TransactionItemManagerAdoNetMsSql(repoConfiguration, categoryManager, unitManager);

				case DbType.SQLite:
					return new TransactionItemManagerAdoNetSQLite(repoConfiguration, categoryManager, unitManager);

				default:
					throw new NotImplementedException(string.Format(Localized.TransactionItemManagerAdoNetFactory_is_not_implemented_for_this_DbType__FORMAT__, repoConfiguration.DbType));
			}
		}
	}

	public abstract class TransactionItemManagerAdoNetBase : TransactionItemManagerDbBase
	{
		protected TransactionItemManagerAdoNetBase(IRepoConfiguration repoConfiguration,
            ICategoryManager categoryManager, IUnitManager unitManager) : base(repoConfiguration, categoryManager, unitManager)
		{
		}

		public const string TableName = "TransactionItem";

		#region READ

		public override List<TransactionItem> GetInterval(DateTime fromDate, DateTime toDate, TransactionItemType transactionItemType)
		{
			fromDate = fromDate.Date;
			toDate = toDate.Date;

			using(var ctx = ExinAdoNetContextFactory.Create(LocalConfig))
			{
				BuildGetIntervalQuery(transactionItemType, ctx);

				ctx.Command.Parameters.AddWithValue("@fromDate", fromDate, LocalConfig.DbType);
				ctx.Command.Parameters.AddWithValue("@toDate", toDate, LocalConfig.DbType);
				ctx.Adapter.SelectCommand = ctx.Command;
				ctx.Adapter.Fill(ctx.DataSet);

				var transactionItems = FetchItems(ctx.DataSet.Tables[0]);
				return transactionItems;
			}
		}

		public override List<TransactionItem> GetAll(TransactionItemType? transactionItemType = null)
		{
			using(var ctx = ExinAdoNetContextFactory.Create(LocalConfig))
			{
				BuildGetAllQuery(transactionItemType, ctx);

				ctx.Adapter.SelectCommand = ctx.Command;
				ctx.Adapter.Fill(ctx.DataSet);

				var transactionItems = FetchItems(ctx.DataSet.Tables[0]);
				return transactionItems;
			}
		}

		protected virtual void BuildGetIntervalQuery(TransactionItemType transactionItemType, ExinAdoNetContextBase ctx)
		{
			ctx.Command.CommandText = "SELECT * FROM " + TableName + @"
										   WHERE date >= @fromDate 
										   AND date <= @toDate ";

			if(transactionItemType == TransactionItemType.Expense)
				ctx.Command.CommandText += " AND IsExpenseItem = 1";
			else if(transactionItemType == TransactionItemType.Income)
				ctx.Command.CommandText += " AND IsIncomeItem = 1";
			//else if(transactionItemType == TransactionItemType.Both) <-- No filter predicate
		}

		protected virtual void BuildGetAllQuery(TransactionItemType? transactionItemType, ExinAdoNetContextBase ctx)
		{
			ctx.Command.CommandText = "SELECT * FROM " + TableName;

			if(transactionItemType.HasValue)
				if(transactionItemType == TransactionItemType.Expense)
					ctx.Command.CommandText += " WHERE IsExpenseItem = 1";
				else if(transactionItemType == TransactionItemType.Income)
					ctx.Command.CommandText += " WHERE IsIncomeItem = 1";
		}

		#region Fetch (from DataTable, DataRow)

		private List<TransactionItem> FetchItems(DataTable dataTable)
		{
			var transactionItems = dataTable.AsEnumerable().Select(FetchItem).ToList();
			return transactionItems;
		}

		private TransactionItem FetchItem(DataRow dataRow)
		{
			var transactionItem = new TransactionItem();

			transactionItem.ID = Convert.ToInt32(dataRow[transactionItem.Property(y => y.ID)]);
			transactionItem.Amount = Convert.ToInt32(dataRow[transactionItem.Property(y => y.Amount)]);
			transactionItem.Quantity = Convert.ToInt32(dataRow[transactionItem.Property(y => y.Quantity)]);
			transactionItem.Title = dataRow.Field<string>(transactionItem.Property(x => x.Title));
			transactionItem.Comment = dataRow.Field<string>(transactionItem.Property(x => x.Comment));
			transactionItem.Date = dataRow.Field<DateTime>(transactionItem.Property(x => x.Date));
			transactionItem.IsExpenseItem = dataRow.Field<bool>(transactionItem.Property(x => x.IsExpenseItem));
			transactionItem.IsIncomeItem = dataRow.Field<bool>(transactionItem.Property(x => x.IsIncomeItem));

			transactionItem.UnitID = Convert.ToInt32(dataRow[transactionItem.Property(y => y.UnitID)]);
			transactionItem.CategoryID = Convert.ToInt32(dataRow[transactionItem.Property(y => y.CategoryID)]);

			transactionItem.Unit = UnitManagerLocal.Get(transactionItem.UnitID);
			transactionItem.Category = CategoryManagerLocal.Get(transactionItem.CategoryID);

			return transactionItem;
		}

		#endregion

		#endregion

		#region CREATE, UPDATE, DELETE

		#region CREATE

		#region Insert

		public override void Insert(TransactionItem transactionItem, bool withId = false)
		{
			using(var ctx = ExinAdoNetContextFactory.Create(LocalConfig))
			using(ctx.WithIdentityInsert(TableName, activate: withId))
			{
				Insert(ctx, transactionItem, withId);
			}
		}

		public void Insert(ExinAdoNetContextBase ctx, TransactionItem transactionItem, bool withId = false)
		{
			ctx.Command.CommandText = BuildInsertQuery(withId);
			transactionItem.CopyStandardParams(ctx, LocalConfig.DbType);

			try
			{
				ExecInsertQuery(ctx);
			}
			catch(Exception e)
			{
				ExinLog.ger.LogException(Localized.Could_not_insert_the_transaction_item_, e, transactionItem);
				throw;
			}
		}

		protected virtual void ExecInsertQuery(ExinAdoNetContextBase ctx)
		{
			ctx.Command.ExecuteNonQuery();
		}

		protected virtual string BuildInsertQuery(bool withId)
		{
			var query = @"
					INSERT INTO " + TableName + @"
							   ([Amount]
							   ,[Quantity]
							   ,[UnitID]
							   ,[Title]
							   ,[Comment]
							   ,[Date]
							   ,[CategoryID]
							   ,[IsExpenseItem]
							   ,[IsIncomeItem] ";

			query += withId ? ",[ID]) " : ") ";
			query += @"
						 VALUES
							   (@Amount
							   ,@Quantity
							   ,@UnitID
							   ,@Title
							   ,@Comment
							   ,@Date
							   ,@CategoryID
							   ,@IsExpenseItem
							   ,@IsIncomeItem ";

			query += withId ? ",@ID) " : ") ";

			return query;
		}

		#endregion

		#region InsertMany

		public override void InsertMany(IList<TransactionItem> transactionItems, bool withId = false, bool forceOneByOne = false)
		{
			using(var ctx = ExinAdoNetContextFactory.Create(LocalConfig))
			{
				InsertMany(ctx, transactionItems, withId, forceOneByOne);
			}
		}

		protected virtual void InsertMany(ExinAdoNetContextBase ctx, IList<TransactionItem> transactionItems, bool withId = false, bool forceOneByOne = false)
		{
			using(ctx.WithIdentityInsert(TableName, activate: withId))
			{
				if(transactionItems == null || transactionItems.Count == 0)
					return;

				if(transactionItems.Count == 1)
				{
					Insert(ctx, transactionItems[0], withId);
					return;
				}

				if(forceOneByOne)
					InsertMany_OneByOne(transactionItems, withId, transactionItems.Count, ctx);
				else
					InsertMany_Bundled(transactionItems, withId, transactionItems.Count, ctx);
			}
		}

		protected virtual void InsertMany_OneByOne(IList<TransactionItem> transactionItems, bool withId, int sumOfRecords, ExinAdoNetContextBase ctx)
		{
			using(var transactionScope = new TransactionScope())
			{
				ctx.Command.CommandText = BuildInsertQuery(withId);

				try
				{
					foreach(var transactionItem in transactionItems)
					{
						ctx.Command.Parameters.Clear();
						transactionItem.CopyStandardParams(ctx, LocalConfig.DbType);
						ctx.Command.ExecuteNonQuery();
					}
					transactionScope.Complete();
				}
				catch(Exception e)
				{
					var msg = Localized.Could_not_insert_the_transaction_items_ + transactionItems.Count + Localized._pc_;
					ExinLog.ger.LogException(msg, e, transactionItems);
					throw;
				}
			}
		}

		protected virtual void InsertMany_Bundled(IList<TransactionItem> transactionItems, bool withId, int sumOfRecords, ExinAdoNetContextBase ctx)
		{
			var queryHeader = BuildInsertManyQueryHeader(withId);
			var queryFirstRecord = BuildInsertManyQueryBody(transactionItems, withId, 1, 1, removeLastSemicolon: false); // The number of the records starts at 1 (not at 0)

			const int maxParamsPerTurn = 2000; // The maximum allowed number of parameters is 2100
			var paramsPerRecord = queryFirstRecord.Count(ch => ch == '@'); // The num of '@' == num of parameters (in the 1. record)
			var recordsPerTurn = maxParamsPerTurn / paramsPerRecord; // Int -> the value is floord (eg: 2.2 -> 2)

			//var sumOfTurns = Math.Ceiling((float)sumOfRecords / recordsPerTurn);
			//var sumOfParams = paramsPerRecord * sumOfRecords;

			if(sumOfRecords <= recordsPerTurn) // We can do all at once
			{
				var queryRemindedRecords = BuildInsertManyQueryBody(transactionItems, withId, 2, sumOfRecords);
				ctx.Command.CommandText = queryHeader + queryFirstRecord + queryRemindedRecords;
				CopyInsertManyParams(transactionItems, ctx, 1, sumOfRecords, withId);

				try
				{
					// Here, we insert multiple records; but don't need a TransactionScope, because these all
					// are executed in 1 statement (no ";" in the query)
					// Tested with Ms SQL Server 2012
					ctx.Command.ExecuteNonQuery();
				}
				catch(Exception e)
				{
					var msg = Localized.Could_not_insert_the_transaction_items_ + transactionItems.Count + Localized._pc_;
					ExinLog.ger.LogException(msg, e, transactionItems);
					throw;
				}
			}
			else // sumOfRecords > recordsPerTurn <-- We need more turns
			{
				try
				{
					using(var transactionScope = new TransactionScope())
					{
						var recordsDone = 0;

						while(recordsDone < sumOfRecords)
						{
							var actualFromRecord = recordsDone + 1;
							var actualToRecord = Math.Min(recordsDone + recordsPerTurn, sumOfRecords);

							var actualquerybody = BuildInsertManyQueryBody(transactionItems, withId, actualFromRecord, actualToRecord);
							ctx.Command.CommandText = queryHeader + actualquerybody;

							ctx.Command.Parameters.Clear();
							CopyInsertManyParams(transactionItems, ctx, actualFromRecord, actualToRecord, withId);

							ctx.Command.ExecuteNonQuery();

							recordsDone += recordsPerTurn;
						}

						transactionScope.Complete();
					}
				}
				catch(Exception e)
				{
					var msg = Localized.Could_not_insert_the_transaction_items_ + transactionItems.Count + Localized._pc_;
					ExinLog.ger.LogException(msg, e, transactionItems);
					throw;
				}
			}
		}

		protected virtual string BuildInsertManyQueryHeader(bool withId)
		{
			var query = new StringBuilder();

			query.Append(@"
					INSERT INTO " + TableName + @"
							   ([Amount]
							   ,[Quantity]
							   ,[UnitID]
							   ,[Title]
							   ,[Comment]
							   ,[Date]
							   ,[CategoryID]
							   ,[IsExpenseItem]
							   ,[IsIncomeItem] ");

			query.Append(withId ? ",[ID]) " : ") ");
			query.Append(" VALUES ");
			return query.ToString();
		}

		protected virtual string BuildInsertManyQueryBody(IList<TransactionItem> transactionItems, bool withId, int from, int to, bool removeLastSemicolon = true)
		{
			var query = new StringBuilder();

			for(int i = from; i <= to; i++)
			{
				var transactionItem = transactionItems[i - 1];

				query.Append("(");
				query.Append(transactionItem.Amount);

				query.Append(",").Append(transactionItem.Quantity);
				query.Append(",").Append(transactionItem.UnitID);
				query.Append(",").Append("@Title").Append(i);
				query.Append(",").Append("@Comment").Append(i);
				query.Append(",").AppendDateToQuery(transactionItem.Date);
				query.Append(",").Append(transactionItem.CategoryID);
				query.Append(",").AppendBoolToQuery(transactionItem.IsExpenseItem);
				query.Append(",").AppendBoolToQuery(transactionItem.IsIncomeItem);

				if(withId)
					query.Append(",").Append(transactionItem.ID);

				query.Append("),");
			}

			if(removeLastSemicolon)
				query.Remove(query.Length - 1, 1); // Remove the last ',' semicolon

			return query.ToString();
		}

		private void CopyInsertManyParams(IList<TransactionItem> transactionItems, ExinAdoNetContextBase ctx, int fromRecord, int toRecord, bool withId)
		{
			for(int i = fromRecord; i <= toRecord; i++)
			{
				var transactionItem = transactionItems[i - 1];
				var comment = string.IsNullOrWhiteSpace(transactionItem.Comment) ? Config.DbStringNull : transactionItem.Comment;

				//ctx.Command.Parameters.AddWithValue("@Amount" + i, transactionItem.Amount, LocalConfig.DbType);
				//ctx.Command.Parameters.AddWithValue("@Quantity" + i, transactionItem.Quantity, LocalConfig.DbType);
				//ctx.Command.Parameters.AddWithValue("@UnitID" + i, transactionItem.UnitID, LocalConfig.DbType);
				ctx.Command.Parameters.AddWithValue("@Title" + i, transactionItem.Title, LocalConfig.DbType);
				ctx.Command.Parameters.AddWithValue("@Comment" + i, comment, LocalConfig.DbType);
				//ctx.Command.Parameters.AddWithValue("@Date" + i, transactionItem.Date, LocalConfig.DbType);
				//ctx.Command.Parameters.AddWithValue("@CategoryID" + i, transactionItem.CategoryID, LocalConfig.DbType);
				//ctx.Command.Parameters.AddWithValue("@IsExpenseItem" + i, transactionItem.IsExpenseItem, LocalConfig.DbType);
				//ctx.Command.Parameters.AddWithValue("@IsIncomeItem" + i, transactionItem.IsIncomeItem, LocalConfig.DbType);

				//if(withId)
				//	ctx.Command.Parameters.AddWithValue("@ID" + i, transactionItem.ID, LocalConfig.DbType);
			}
		}

		#endregion

		#endregion

		#region UPDATE

		public override int UpdateFullRecord(TransactionItem transactionItem)
		{
			using(var ctx = ExinAdoNetContextFactory.Create(LocalConfig))
			{
				ctx.Command.CommandText = BuildUpdateFullRecordQuery();

				transactionItem.CopyStandardParams(ctx, LocalConfig.DbType);

				try
				{
					int queryResult = ExecUpdateFullRecordQuery(ctx);
					return queryResult;
				}
				catch(Exception e)
				{
					var msg = Localized.Could_not_update_the_transaction_record__TransactionItem__ID_ + transactionItem.ID + ")" + Environment.NewLine;
					msg += Localized.So_these_modifications_did_not_apply_in_the_database; //data: transactionItem
					ExinLog.ger.LogException(msg, e, transactionItem);
					throw;
				}
			}
		}

		protected virtual string BuildUpdateFullRecordQuery()
		{
			return @"
					UPDATE " + TableName + @"
						SET	 [Amount] = @Amount
							,[Quantity] = @Quantity
							,[UnitID] = @UnitID
							,[Title] = @Title
							,[Comment] = @Comment
							,[Date] = @Date
							,[CategoryID] = @CategoryID
							,[IsExpenseItem] = @IsExpenseItem
							,[IsIncomeItem] = @IsIncomeItem
						WHERE [ID] = @ID";
		}

		protected virtual int ExecUpdateFullRecordQuery(ExinAdoNetContextBase ctx)
		{
			return ctx.Command.ExecuteNonQuery();
		}

		#endregion

		#region DELETE

		#region Delete (1 record)

		public override int Delete(int id)
		{
			using(var ctx = ExinAdoNetContextFactory.Create(LocalConfig))
			{
				ctx.Command.CommandText = BuildDeleteQuery();

				ctx.Command.Parameters.AddWithValue("@ID", id, LocalConfig.DbType);

				try
				{
					int queryResult = ExecDeleteQuery(ctx);
					return queryResult;
				}
				catch(Exception e)
				{
					var msg = Localized.Could_not_remove_the_transaction_record__TransactionItem__ID_ + id + ")";
					ExinLog.ger.LogException(msg, e);
					throw;
				}
			}
		}

		protected virtual int ExecDeleteQuery(ExinAdoNetContextBase ctx)
		{
			return ctx.Command.ExecuteNonQuery();
		}

		protected virtual string BuildDeleteQuery()
		{
			return @"
					DELETE FROM " + TableName + @"
						WHERE ID = @ID";
		}

		#endregion

		#region ClearDay

		public override int ClearDay(DateTime date, TransactionItemType transactionItemType)
		{
			using(var ctx = ExinAdoNetContextFactory.Create(LocalConfig))
			{
				return ClearDay(ctx, date, transactionItemType);
			}
		}

		protected virtual int ClearDay(ExinAdoNetContextBase ctx, DateTime date, TransactionItemType transactionItemType)
		{
			var isExpense = transactionItemType == TransactionItemType.Expense;
			if(isExpense)
				date = date.Date;

			var isIncome = transactionItemType == TransactionItemType.Income;
			if(isIncome)
				date = new DateTime(date.Year, date.Month, 1);

			ctx.Command.CommandText = BuildClearDayQuery();

			ctx.Command.Parameters.AddWithValue("@Date", date, LocalConfig.DbType);
			ctx.Command.Parameters.AddWithValue("@IsExpenseItem", isExpense, LocalConfig.DbType);
			ctx.Command.Parameters.AddWithValue("@IsIncomeItem", isIncome, LocalConfig.DbType);

			try
			{
				int queryResult = ExecClearDayQuery(ctx);
				return queryResult;
			}
			catch(Exception e)
			{
				var transactionItemTypeStr = isExpense ? Localized.expenses : isIncome ? Localized.incomes : Localized.transaction_items;
				var msg = string.Format(Localized.Could_not_remove_the_daily_0_at_1__FORMAT__, transactionItemTypeStr, date.ToLocalizedShortDateString());
				ExinLog.ger.LogException(msg, e);
				throw;
			}
		}

		protected virtual int ExecClearDayQuery(ExinAdoNetContextBase ctx)
		{
			return ctx.Command.ExecuteNonQuery();
		}

		protected virtual string BuildClearDayQuery()
		{
			return @"
					DELETE FROM " + TableName + @"
						WHERE Date = @Date
							AND IsExpenseItem = @IsExpenseItem
							AND IsIncomeItem = @IsIncomeItem";
		}

		#endregion

		#endregion

		/// <param name="transactionItems">If it's null, throws an exception. If it's empty, only clears the day. </param>
		public override void ReplaceDailyItems(IList<TransactionItem> transactionItems, TransactionItemType transactionItemType, DateTime date)
		{
			using(var transactionScope = new TransactionScope())
			using(var ctx = ExinAdoNetContextFactory.Create(LocalConfig))
			{
				ReplaceDailyItems(ctx, transactionItems, transactionItemType, date);

				transactionScope.Complete();
			}
		}

		protected virtual void ReplaceDailyItems(ExinAdoNetContextBase ctx, IList<TransactionItem> transactionItems, TransactionItemType transactionItemType, DateTime date)
		{
			if(transactionItems == null)
			{
				string msg = Localized.ReplaceDailyItems_method_needs_a_not_null_list__;
				var e = new Exception(msg);
				ExinLog.ger.LogException(msg, e);
				throw e;
			}

			if(transactionItems.Any(transactionItem => transactionItem.Date != date))
			{
				string msg = Localized.ReplaceDailyItems_method_replaces_only_1_day__the_item_s_dates_must_be_the_same__day_;
				var e = new Exception(msg);
				ExinLog.ger.LogException(msg, e);
				throw e;
			}

			ClearDay(ctx, date, transactionItemType);

			ctx.Command.CommandText = ""; // for safety sake
			ctx.Command.Parameters.Clear();

			InsertMany(ctx, transactionItems);
		}

		#endregion
	}

	public class TransactionItemManagerAdoNetMsSql : TransactionItemManagerAdoNetBase
	{
		// No need to changed anything

		public TransactionItemManagerAdoNetMsSql(IRepoConfiguration repoConfiguration,
			ICategoryManager categoryManager, IUnitManager unitManager) : base(repoConfiguration, categoryManager, unitManager)
		{
		}
	}

	public class TransactionItemManagerAdoNetSQLite : TransactionItemManagerAdoNetBase
	{
		public TransactionItemManagerAdoNetSQLite(IRepoConfiguration repoConfiguration,
			ICategoryManager categoryManager, IUnitManager unitManager) : base(repoConfiguration, categoryManager, unitManager)
		{
		}

		protected override void ExecInsertQuery(ExinAdoNetContextBase ctx)
		{
			if(_calledFromReplaceDaily)
				ctx.Command.ExecuteNonQuery();
			else
				ctx.ExecInTransactionWithCommit();

			//ctx.ExecInTransactionWithCommit();
		}

		protected override int ExecUpdateFullRecordQuery(ExinAdoNetContextBase ctx)
		{
			return ctx.ExecInTransactionWithCommit();
		}

		protected override int ExecDeleteQuery(ExinAdoNetContextBase ctx)
		{
			return ctx.ExecInTransactionWithCommit();
		}

		#region ReplaceDailyItems and others depends on it

		// This is so not thread safe; but SQLite from the first don't allow multithreaded access
		private bool _calledFromReplaceDaily = false;

		protected override void ReplaceDailyItems(ExinAdoNetContextBase ctx, IList<TransactionItem> transactionItems, TransactionItemType transactionItemType, DateTime date)
		{
			// In SQLite, there are no nested transactions...

			_calledFromReplaceDaily = true;

			using(ctx.WithTransaction())
			{
				base.ReplaceDailyItems(ctx, transactionItems, transactionItemType, date);
				ctx.Command.Transaction.Commit();
			}

			_calledFromReplaceDaily = false;
		}

		protected override void InsertMany_OneByOne(IList<TransactionItem> transactionItems, bool withId, int sumOfRecords, ExinAdoNetContextBase ctx)
		{
			if(_calledFromReplaceDaily)
			{
				base.InsertMany_OneByOne(transactionItems, withId, sumOfRecords, ctx);
			}
			else
			{
				// If the TransactionScope object and the operations with it don't make any sense,
				// we only have to wrap the original method in an SQLite "transaction scope" :)

				using(ctx.WithTransaction())
				{
					base.InsertMany_OneByOne(transactionItems, withId, sumOfRecords, ctx);

					ctx.Command.Transaction.Commit();
				}
			}
		}

		protected override void InsertMany_Bundled(IList<TransactionItem> transactionItems, bool withId, int sumOfRecords, ExinAdoNetContextBase ctx)
		{
			if(_calledFromReplaceDaily)
			{
				base.InsertMany_Bundled(transactionItems, withId, sumOfRecords, ctx);
			}
			else
			{
				// If the TransactionScope object and the operations with it don't make any sense,
				// we only have to wrap the original method in an SQLite "transaction scope" :)

				using(ctx.WithTransaction())
				{
					base.InsertMany_Bundled(transactionItems, withId, sumOfRecords, ctx);

					ctx.Command.Transaction.Commit();
				}
			}
		}

		protected override int ExecClearDayQuery(ExinAdoNetContextBase ctx)
		{
			if(_calledFromReplaceDaily)
				return ctx.Command.ExecuteNonQuery();
			else
				return ctx.ExecInTransactionWithCommit();
		}

		#endregion

	}
}
