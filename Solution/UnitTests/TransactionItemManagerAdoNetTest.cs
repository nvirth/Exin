using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using BLL;
using Common;
using Common.DbEntities;
using Common.UiModels;
using Common.UiModels.WPF;
using Common.Utils;
using Common.Utils.Helpers;
using DAL;
using DAL.DataBase.AdoNet.Managers;
using DAL.DataBase.Managers;
using DAL.FileRepo;
using Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UtilsShared;
using Helpers = Common.Utils.Helpers;

namespace UnitTests
{
	[TestClass]
	public class TransactionItemManagerAdoNetTest : ITransactionItemManager
	{
		// todo test for incomes
		// todo DailyExpenses test without using this manager
		// todo Instead of TransactionItemManagerAdoNetMsSql check how will it be ok with it's Factory (for MsSql and SQLite as well)

		#region Members

		#region Dates

		/// <summary>
		/// 1 ExpenseItem in this day
		/// </summary>
		private static readonly DateTime Date_2009_10_01 = new DateTime(2009, 10, 01);

		/// <summary>
		/// 6 ExpenseItem in this day
		/// </summary>
		private static readonly DateTime Date_2009_10_05 = new DateTime(2009, 10, 05);

		/// <summary>
		/// 1 ExpenseItem in this day
		/// </summary>
		private static readonly DateTime Date_2009_10_06 = new DateTime(2009, 10, 06);

		/// <summary>
		/// 1 ExpenseItem in this day
		/// </summary>
		private static readonly DateTime Date_2009_11_01 = new DateTime(2009, 11, 01);

		/// <summary>
		/// 3 ExpenseItem in this day
		/// </summary>
		private static readonly DateTime Date_2009_11_04 = new DateTime(2009, 11, 04);

		/// <summary>
		/// 1 ExpenseItem in this day
		/// </summary>
		private static readonly DateTime Date_2010_01_05 = new DateTime(2010, 01, 05);

		/// <summary>
		/// 2 ExpenseItem in this day
		/// </summary>
		private static readonly DateTime Date_2010_01_29 = new DateTime(2010, 01, 29);

		#endregion

		/// <summary>
		/// The manager to test
		/// </summary>
		private static readonly TransactionItemManagerAdoNetBase Manager = new TransactionItemManagerAdoNetMsSql();

		/// <summary>
		/// Caches the full test repository. Get filled only once per ClassInitialize, 
		/// so it's items should not be changed. It is sorted by List.Sort() method. 
		/// </summary>
		private static readonly List<TransactionItem> AllTransactionItems = new List<TransactionItem>(70);

		/// <summary>
		/// Caches the full test repository. Get filled only once per ClassInitialize, 
		/// so it's items should not be changed. It is sorted by List.Sort() method. 
		/// It's computed from field <see cref="AllTransactionItems"/>. 
		/// It also loses the <see cref="TransactionItem"/>'s ID field. 
		/// </summary>
		private static readonly List<ExpenseItem> AllExpenseItems = new List<ExpenseItem>(70);

		#endregion

		#region Helpers

		private void CheckEquals(List<TransactionItem> expenses, List<TransactionItem> referenceExpenses, string msg, bool sortReferences = true, bool sortExpenses = true)
		{
			Assert.IsTrue(expenses.Count == referenceExpenses.Count, "The quantities differ");

			bool equals = ContainsSameAs(referenceExpenses, expenses, sortReferences, sortExpenses);
			Assert.IsTrue(@equals, msg);
		}

		private bool ContainsSameAsAllExpenseItems(List<ExpenseItem> referenceExpenses, bool needSort = false)
		{
			return ContainsSameAs(referenceExpenses, AllExpenseItems, needSort, /*sortRight*/ false, ExpenseItem.DefaultExpenseItemComparer);
		}

		private bool ContainsSameAs(List<ExpenseItem> left, List<ExpenseItem> right, bool sortLeft = false, bool sortRight = false)
		{
			return ContainsSameAs(left, right, sortLeft, sortRight, ExpenseItem.DefaultExpenseItemComparer);
		}

		private bool ContainsSameAs(List<TransactionItem> left, List<TransactionItem> right, bool sortLeft = false, bool sortRight = false)
		{
			return ContainsSameAs(left, right, sortLeft, sortRight, TransactionItem.WithoutIdComparer);
		}

		private bool ContainsSameAs<T>(List<T> left, List<T> right, bool sortLeft, bool sortRight, IEqualityComparer<T> equalityComparer = null)
			where T : class
		{
			if(sortLeft)
				left.Sort();
			if(sortRight)
				right.Sort();

			var equals = Helpers.Helpers.SequenceEqual(left, right, equalityComparer);
			equals = equals || Helpers.Helpers.ContainsSameData(left, right, equalityComparer);
			return equals;
		}

		#endregion

		[TestInitialize]
		public void InitTests()
		{
			Common.InitDataBase();
		}

		[ClassInitialize]
		public static void InitTestClass(TestContext testContext)
		{
			Common.InitClass();

			AllTransactionItems.AddRange(Manager.GetAll());
			AllTransactionItems.Sort();

			AllExpenseItems.AddRange(AllTransactionItems.Select(ti => ti.ToExpenseItem()));
		}

		[TestMethod]
		public void ClearDay()
		{
			var dailyExpenses = Manager.GetDaily(Date_2009_10_05, TransactionItemType.Expense);
			Assert.IsFalse(dailyExpenses.Count == 0, "The unit test is wrong, the day to check is empty from the first");

			var fullDbCount = AllTransactionItems.Count;
			var dailyCount = dailyExpenses.Count;

			Manager.ClearDay(Date_2009_10_05, TransactionItemType.Expense);

			dailyExpenses = Manager.GetDaily(Date_2009_10_05, TransactionItemType.Expense);
			Assert.IsTrue(dailyExpenses.Count == 0, "The day has records");

			var newAllTransactionItems = Manager.GetAll();
			Assert.IsTrue(fullDbCount - dailyCount == newAllTransactionItems.Count, "Not only the meant day's records were deleted");
		}

		[TestMethod]
		public void Delete()
		{
			var expenses = Manager.GetDaily(Date_2009_10_05, TransactionItemType.Expense);
			var originalCount = expenses.Count;

			Manager.Delete(expenses[0].ID);

			expenses = Manager.GetDaily(Date_2009_10_05, TransactionItemType.Expense);
			Assert.IsTrue(expenses.Count == originalCount - 1, "The quantity of records did not decreased (by 1)");

			var newAllTransactionItems = Manager.GetAll();
			Assert.IsTrue(newAllTransactionItems.Count == AllTransactionItems.Count - 1, "The method deleted other records as well");
		}

		[TestMethod]
		public void GetDaily()
		{
			List<TransactionItem> expenses = Manager.GetDaily(Date_2009_10_05, TransactionItemType.Expense);
			var referenceExpenses = new List<TransactionItem>();
			FileRepoManager.GetDailyExpenses(Date_2009_10_05, ei => referenceExpenses.Add(ei.ToTransactionItem()));

			const string msg = "The result of the GetDaily method was not equal to the result of the DailyExpenses class' ctor (works from file repo)";
			CheckEquals(expenses, referenceExpenses, msg);
		}

		[TestMethod]
		public void GetMontly()
		{
			var expenses = Manager.GetMontly(Date_2009_10_05, TransactionItemType.Expense);
			var referenceExpenses = new List<TransactionItem>();
			FileRepoManager.GetMonthlyExpenses(Date_2009_10_05, ei => referenceExpenses.Add(ei.ToTransactionItem()));

			const string msg = "The result of the GetMontly method was not equal to the result of the MonthlyExpenses class' ctor (works from file repo)";
			CheckEquals(expenses, referenceExpenses, msg);
		}

		[TestMethod]
		public void GetYearly()
		{
			var expenses = Manager.GetYearly(Date_2009_10_05, TransactionItemType.Expense);
			var referenceExpenses = new List<TransactionItem>();
			FileRepoManager.GetMonthlyExpenses(Date_2009_10_05, ei => referenceExpenses.Add(ei.ToTransactionItem()));
			FileRepoManager.GetMonthlyExpenses(Date_2009_11_01, ei => referenceExpenses.Add(ei.ToTransactionItem()));

			const string msg = "The result set of the GetYearly method was not equal to the manually collected data set (done with 2 MonthlyExpenses class' ctor (works from file repo))";
			CheckEquals(expenses, referenceExpenses, msg);
		}

		[TestMethod]
		public void GetAll()
		{
			var referenceExpenses = new List<TransactionItem>();
			FileRepoManager.GetMonthlyExpenses(Date_2009_10_05, ei => referenceExpenses.Add(ei.ToTransactionItem()));
			FileRepoManager.GetMonthlyExpenses(Date_2009_11_01, ei => referenceExpenses.Add(ei.ToTransactionItem()));
			FileRepoManager.GetMonthlyExpenses(Date_2010_01_05, ei => referenceExpenses.Add(ei.ToTransactionItem()));

			const string msg = "The result set of the GetAll method was not equal to the manually collected data set (done with 3 FileRepoManager.GetMonthlyExpenses(...) call)";
			CheckEquals(AllTransactionItems, referenceExpenses, msg, sortReferences: true, sortExpenses: false);

			//Assert.IsTrue(AllTransactionItems.Count == referenceExpenses.Count, "The quantities differ");

			//var containsSame = ContainsSameAsAllExpenseItems(referenceExpenses, needSort: true);
			//const string msg = "The result set of the GetAll method was not equal to the manually collected data set (done with 3 FileRepoManager.GetMonthlyExpenses(...) call)";
			//Assert.IsTrue(containsSame, msg);
		}

		[TestMethod]
		public void GetInterval()
		{
			List<TransactionItem> expenses = Manager.GetInterval(Date_2009_10_01, Date_2009_10_06, TransactionItemType.Expense);
			var referenceExpenses = new List<TransactionItem>();
			FileRepoManager.GetMonthlyExpenses(Date_2009_10_01,
				ei =>
				{
					if(ei.Date <= Date_2009_10_06)
						referenceExpenses.Add(ei.ToTransactionItem());
				});

			const string msg = "The result of the GetInterval method was not equal to the (filtered) result of the MonthlyExpenses class' ctor (works from file repo)";
			CheckEquals(expenses, referenceExpenses, msg);

			//var expenses = Manager.GetInterval(Date_2009_10_01, Date_2009_10_06, TransactionItemType.Expense)
			//	.Select(ti => ti.ToExpenseItem());
			//var referenceExpenses = new MonthlyExpenses(Date_2009_10_01).TransactionItems
			//	.Where(tib => tib.Date <= Date_2009_10_06)
			//	.ToArray();

			//foreach(var expenseItem in expenses)
			//	if(!referenceExpenses.Contains(expenseItem))
			//		Assert.Fail("The result of the GetInterval method was not equal to the (filtered) result of the MonthlyExpenses class' ctor (works from file repo)");
		}

		[TestMethod]
		public void Insert()
		{
			var transactionItem = new TransactionItem()
			{
				//ID = 0,
				Amount = 999,
				Quantity = 2,
				Title = "Mock title",
				Comment = "Mock comment",
				Date = new DateTime(2011, 05, 10),
				IsExpenseItem = true,
				IsIncomeItem = false,
				Unit = UnitManager.GetUnitDb,
				UnitID = UnitManager.GetUnitDb.ID,
				Category = CategoryManager.GetCategoryOthers,
				CategoryID = CategoryManager.GetCategoryOthers.ID,
			};

			Manager.Insert(transactionItem);
			var dailyExpenses = Manager.GetDaily(transactionItem.Date, TransactionItemType.Expense);
			var contains = dailyExpenses.Contains(transactionItem, TransactionItem.WithoutIdComparer);
			Assert.IsTrue(contains, "The Insert method did not inserted a valid Expense record");

			transactionItem.IsIncomeItem = true;
			transactionItem.IsExpenseItem = false;
			transactionItem.Date = new DateTime(2011, 05, 01); // todo this should be done in function Manager.Insert
			Manager.Insert(transactionItem);
			var dailyIncomes = Manager.GetDaily(transactionItem.Date, TransactionItemType.Income);
			contains = dailyIncomes.Contains(transactionItem, TransactionItem.WithoutIdComparer);
			Assert.IsTrue(contains, "The Insert method did not inserted a valid Income record");

			try
			{
				transactionItem.IsIncomeItem = true;
				transactionItem.IsExpenseItem = true;
				Manager.Insert(transactionItem);
				Assert.Fail("The Insert method inserted an invalid record (with IsIncome==1, IsExpense==1)");
			}
			catch(AssertFailedException) { throw; }
			catch(Exception e) { Console.WriteLine(e); }
		}

		[TestMethod]
		public void InsertMany()
		{
			var expenseItems = Manager.GetDaily(Date_2009_10_05, TransactionItemType.Expense);
			var dailyCount = expenseItems.Count;

			expenseItems[0].Comment = null;
			InsertMany_Core(expenseItems, ref dailyCount, 1, "Problem with inserting 1 record with a NULL property");

			InsertMany_Core(expenseItems, ref dailyCount, 0, "Problem with inserting 0 records");
			InsertMany_Core(expenseItems, ref dailyCount, 1, "Problem with inserting 1 record");
			InsertMany_Core(expenseItems, ref dailyCount, 2, "Problem with inserting 2 records");

			while(expenseItems.Count <= 1001) // 2 turn
				expenseItems.AddRange(expenseItems);

			InsertMany_Core(expenseItems, ref dailyCount, 100, forceOneByOne: true, msg: "Problem with inserting one-by-one (100) records");
			InsertMany_Core(expenseItems, ref dailyCount, 1000, "Problem with inserting 1 turn bundled (1000) records");
			InsertMany_Core(expenseItems, ref dailyCount, expenseItems.Count, "Problem with inserting 1+ turn bundled (1000+) records");
		}

		private static void InsertMany_Core(List<TransactionItem> expenseItems, ref int dailyCount, int howMany, string msg, bool forceOneByOne = false)
		{
			Manager.InsertMany(expenseItems.GetRange(0, howMany), forceOneByOne: forceOneByOne);
			dailyCount += howMany;
			var expenseItemsToCheck = Manager.GetDaily(Date_2009_10_05, TransactionItemType.Expense);
			Assert.IsTrue(expenseItemsToCheck.Count == dailyCount, msg);
		}

		[TestMethod]
		public void UpdateFullRecord()
		{
			var sourceDayItems = Manager.GetDaily(Date_2010_01_05, TransactionItemType.Expense);
			var sourceDailyCount = sourceDayItems.Count;
			var transactionItem = sourceDayItems[0];

			var destinationDailyCount = Manager.GetDaily(Date_2010_01_29, TransactionItemType.Expense).Count;

			var updatedTransactionItem = new TransactionItem()
			{
				ID = transactionItem.ID, // The UpdateFullRecord method works via ID

				Amount = 999,
				Quantity = 2,
				Title = "Mock title",
				Comment = "Mock comment",
				Date = Date_2010_01_29,
				IsExpenseItem = true,
				IsIncomeItem = false,
				Unit = UnitManager.GetUnitDb,
				UnitID = UnitManager.GetUnitDb.ID,
				Category = CategoryManager.GetCategoryOthers,
				CategoryID = CategoryManager.GetCategoryOthers.ID,
			};
			Manager.UpdateFullRecord(updatedTransactionItem);

			sourceDayItems = Manager.GetDaily(Date_2010_01_05, TransactionItemType.Expense);
			Assert.IsTrue(sourceDailyCount == sourceDayItems.Count + 1, "The record's Date property was not updated properly");
			Assert.IsFalse(sourceDayItems.Contains(transactionItem), "The record's Date property was not updated properly");
			Assert.IsFalse(sourceDayItems.Contains(updatedTransactionItem), "The record's Date property was not updated properly");

			var destinationDayItems = Manager.GetDaily(Date_2010_01_29, TransactionItemType.Expense);
			Assert.IsTrue(destinationDailyCount == destinationDayItems.Count - 1, "The record's Date property was not updated properly");
			//Assert.IsFalse(destinationDayItems.Contains(transactionItem)); // At least the Date property differs
			Assert.IsTrue(destinationDayItems.Contains(updatedTransactionItem), "The record was not updated properly");
		}

		//TODO test when there is only 1 item in a day
		[TestMethod]
		public void ReplaceDailyItems()
		{
			var transactionItems = Manager.GetDaily(Date_2009_10_05, TransactionItemType.Expense);
			foreach(var transactionItem in transactionItems)
				transactionItem.Title += " - test change";

			transactionItems.RemoveRange(0, 2);
			var plusItems = Manager.GetDaily(Date_2009_11_04, TransactionItemType.Expense);
			foreach(var transactionItem in plusItems)
				transactionItem.Date = transactionItems[0].Date;
			transactionItems.AddRange(plusItems);

			Manager.ReplaceDailyItems(transactionItems, TransactionItemType.Expense, transactionItems[0].Date);
			var newTransactionItems = Manager.GetDaily(Date_2009_10_05, TransactionItemType.Expense);
			var equal = newTransactionItems.SequenceEqual(transactionItems, TransactionItem.WithoutIdComparer);
			Assert.IsTrue(equal, "Replace did not work, new and old lists were not equal");

			try
			{
				transactionItems[0].Date = transactionItems[0].Date.AddDays(1);
				Manager.ReplaceDailyItems(transactionItems, TransactionItemType.Expense, transactionItems[0].Date);
				Assert.Fail("Replace worked with different dates");
			}
			catch(AssertFailedException) { throw; }
			catch(Exception e) { Console.WriteLine(e); }
		}

		#region ITransactionItemManager

		// If the interface grows, there will be an error in this class, because the new method of the 
		// interface is not implemented yet. We can force this way to do testing :)

		public List<TransactionItem> GetDaily(DateTime date, TransactionItemType transactionItemType)
		{
			throw new NotImplementedException();
		}

		public List<TransactionItem> GetMontly(DateTime date, TransactionItemType transactionItemType)
		{
			throw new NotImplementedException();
		}

		public List<TransactionItem> GetYearly(DateTime date, TransactionItemType transactionItemType)
		{
			throw new NotImplementedException();
		}

		public List<TransactionItem> GetInterval(DateTime fromDate, DateTime toDate, TransactionItemType transactionItemType)
		{
			throw new NotImplementedException();
		}

		public List<TransactionItem> GetAll(TransactionItemType? transactionItemType)
		{
			throw new NotImplementedException();
		}

		public void Insert(TransactionItem transactionItem, bool withId = false)
		{
			throw new NotImplementedException();
		}

		public void InsertMany(IList<TransactionItem> transactionItems, bool withId = false, bool forceOneByOne = false)
		{
			throw new NotImplementedException();
		}

		public int UpdateFullRecord(TransactionItem transactionItem)
		{
			throw new NotImplementedException();
		}

		public int Delete(int id)
		{
			throw new NotImplementedException();
		}

		public int ClearDay(DateTime date, TransactionItemType transactionItemType)
		{
			throw new NotImplementedException();
		}

		public void ReplaceDailyItems(IList<TransactionItem> transactionItems, TransactionItemType transactionItemType, DateTime date)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
