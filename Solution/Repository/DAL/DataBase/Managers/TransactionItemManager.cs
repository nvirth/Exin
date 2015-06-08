using System;
using System.Collections.Generic;
using Common;
using Common.Db.Entities;
using DAL.DataBase.Managers.Factory;

namespace DAL.DataBase.Managers
{
	public interface ITransactionItemManager
	{
		List<TransactionItem> GetDaily(DateTime date, TransactionItemType transactionItemType);
		List<TransactionItem> GetMontly(DateTime date, TransactionItemType transactionItemType);
		List<TransactionItem> GetYearly(DateTime date, TransactionItemType transactionItemType);
		List<TransactionItem> GetInterval(DateTime fromDate, DateTime toDate, TransactionItemType transactionItemType);
		List<TransactionItem> GetAll(TransactionItemType? transactionItemType);

		void Insert(TransactionItem transactionItem, bool withId = false);
		void InsertMany(IList<TransactionItem> transactionItems, bool withId = false, bool forceOneByOne = false);

		int UpdateFullRecord(TransactionItem transactionItem);

		int Delete(int id);
		int ClearDay(DateTime date, TransactionItemType transactionItemType);

		void ReplaceDailyItems(IList<TransactionItem> transactionItems, TransactionItemType transactionItemType, DateTime date);
	}

	public abstract class TransactionItemManagerCommonBase : ITransactionItemManager
	{
		#region READ

		public List<TransactionItem> GetDaily(DateTime date, TransactionItemType transactionItemType)
		{
			if(transactionItemType == TransactionItemType.Income)
				date = new DateTime(date.Year, date.Month, 1);

			var fromDate = date.Date;
			var toDate = date.Date;

			var transactionItems = GetInterval(fromDate, toDate, transactionItemType);
			return transactionItems;
		}

		public List<TransactionItem> GetMontly(DateTime date, TransactionItemType transactionItemType)
		{
			var fromDate = new DateTime(date.Year, date.Month, 1);
			var toDate = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));

			var transactionItems = GetInterval(fromDate, toDate, transactionItemType);
			return transactionItems;
		}

		public List<TransactionItem> GetYearly(DateTime date, TransactionItemType transactionItemType)
		{
			var fromDate = new DateTime(date.Year, 1, 1);
			var toDate = new DateTime(date.Year, 12, 31);

			var transactionItems = GetInterval(fromDate, toDate, transactionItemType);
			return transactionItems;
		}

		#endregion

		#region Abstract

		public abstract List<TransactionItem> GetInterval(DateTime fromDate, DateTime toDate, TransactionItemType transactionItemType);
		public abstract List<TransactionItem> GetAll(TransactionItemType? transactionItemType);
		public abstract void Insert(TransactionItem transactionItem, bool withId = false);
		public abstract void InsertMany(IList<TransactionItem> transactionItems, bool withId = false, bool forceOneByOne = false);
		public abstract int UpdateFullRecord(TransactionItem transactionItem);
		public abstract int Delete(int id);
		public abstract int ClearDay(DateTime date, TransactionItemType transactionItemType);
		public abstract void ReplaceDailyItems(IList<TransactionItem> transactionItems, TransactionItemType transactionItemType, DateTime date);

		#endregion

	}
	
	public static class TransactionItemManager
	{
		public static readonly ITransactionItemManager Instance = ManagerFactory.ITransactionItemManager;
	}
}
