using System;
using Common.UiModels;
using Common.UiModels.WPF;
using DAL.DataBase.Managers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
	[TestClass]
	public class ExpenseItemTest
	{
		[TestInitialize]
		public void InitTests()
		{
			Common.InitDataBase();
		}

		[ClassInitialize]
		public static void InitTestClass(TestContext testContext)
		{
			Common.InitClass();
		}

		[TestMethod]
		public void CtorTest()
		{
			var expenseItem = new ExpenseItem();
			Assert.IsTrue(expenseItem.Unit == UnitManager.GetUnitDb, "Ctor misses default value for Unit");
			Assert.IsTrue(expenseItem.Category == CategoryManager.GetCategoryOthers, "Ctor misses default value for Category");
		}
	}
}
