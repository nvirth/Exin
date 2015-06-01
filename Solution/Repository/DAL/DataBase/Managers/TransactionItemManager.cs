using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Transactions;
using Common;
using Common.DbEntities;
using DAL.DataBase.AdoNet;
using DAL.DataBase.AdoNet.Managers.Base;
using DAL.DataBase.Managers.Factory;
using Config = Common.Config.Config;

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
		#region ITransactionItemManager singleton

		// --
		private static readonly ITransactionItemManager Manager = ManagerFactory.ITransactionItemManager;
		// --

		public static List<TransactionItem> GetDaily(DateTime date, TransactionItemType transactionItemType)
		{
			return Manager.GetDaily(date, transactionItemType);
		}
		public static List<TransactionItem> GetMontly(DateTime date, TransactionItemType transactionItemType)
		{
			return Manager.GetMontly(date, transactionItemType);
		}
		public static List<TransactionItem> GetYearly(DateTime date, TransactionItemType transactionItemType)
		{
			return Manager.GetYearly(date, transactionItemType);
		}
		public static List<TransactionItem> GetInterval(DateTime fromDate, DateTime toDate, TransactionItemType transactionItemType)
		{
			return Manager.GetInterval(fromDate, toDate, transactionItemType);
		}
		public static void Insert(TransactionItem transactionItem, bool withId = false)
		{
			Manager.Insert(transactionItem, withId);
		}
		public static void InsertMany(IList<TransactionItem> transactionItems, bool withId = false)
		{
			Manager.InsertMany(transactionItems, withId);
		}
		public static int UpdateFullRecord(TransactionItem transactionItem)
		{
			return Manager.UpdateFullRecord(transactionItem);
		}
		public static int Delete(int id)
		{
			return Manager.Delete(id);
		}
		public static int ClearDay(DateTime date, TransactionItemType transactionItemType)
		{
			return Manager.ClearDay(date, transactionItemType);
		}
		public static void ReplaceDailyItems(IList<TransactionItem> transactionItems, TransactionItemType transactionItemType, DateTime date)
		{
			Manager.ReplaceDailyItems(transactionItems, transactionItemType, date);
		}
		public static List<TransactionItem> GetAll(TransactionItemType? transactionItemType)
		{
			return Manager.GetAll(transactionItemType);
		}

		#endregion
	}
}
