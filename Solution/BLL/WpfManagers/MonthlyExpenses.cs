using System;
using System.Threading.Tasks;
using Common;
using Common.Utils;
using DAL;
using DAL.DataBase.Managers;
using DAL.FileRepo;
using Localization;

namespace BLL.WpfManagers
{
	public class MonthlyExpenses : SummaryEngineBase
	{
		#region Ctors

		// In the base class, there is a virtual method call in ctor. So in this class (and in other inherited classes too) 
		// it is recommended to use ctors only with empty body. 
		// Read this for the reason: 
		//
		// When an object written in C# is constructed, what happens is that the initializers run in order 
		// from the most derived class to the base class, and then constructors run in order from the 
		// base class to the most derived class (see Eric Lippert's blog for details as to why this is).
		//
		// Also in .NET objects do not change type as they are constructed, but start out as the most derived type,
		// with the method table being for the most derived type. This means that virtual method calls always 
		// run on the most derived type.
		//
		// When you combine these two facts you are left with the problem that if you make a virtual method call
		// in a constructor, and it is not the most derived type in its inheritance hierarchy, 
		// THAT IT WILL BE CALLED ON A CLASS WHOSE CONSTRUCTOR HAS NOT BEEN RUN, 
		// and therefore may not be in a suitable state to have that method called. 

		public MonthlyExpenses(bool doWork = true) : this(DateTime.Now, doWork) { }
		public MonthlyExpenses(DateTime dateTime, bool doWork = true) : this(new DatePaths(dateTime), doWork) { }
		public MonthlyExpenses(DatePaths datePaths, bool doWork = true) : base(datePaths, doWork) { }

		#endregion

		#region Methods

		public override void ReadData()
		{
			// In a parallel thread to not to freeze the GUI
			Task.Factory.StartNew(ReadDataAction);
		}

		/// <summary>
		/// It runs in another thread
		/// </summary>
		private void ReadDataAction()
		{
			base.ReadData();

			// Monthly expenses' summaries will be saved immediately after reading them. 
			if (!HasError)
				Save();
		}

		protected override void ReadDataMessage()
		{
			MessagePresenter.WriteLineSeparator();
			MessagePresenter.WriteLine(Localized.Monthly_summary_ + DatePaths.Date.ToString(Localized.DateFormat_year_month));
		}

		protected override void ReadDataFromDb()
		{
			var transactionItems = TransactionItemManager.GetMontly(DatePaths.Date, TransactionItemType.Expense);
			foreach(var transactionItem in transactionItems)
			{
				Add(transactionItem.ToExpenseItem());
			}
		}

		protected override void ReadDataFromFile()
		{
			FileRepoManager.Instance.GetMonthlyExpenses(DatePaths).ForEach(Add);
		}

		protected override void SaveToDb()
		{
			// Monthly expenses do not need to be saved in DB
		}

		protected override void SaveSummariesToDb()
		{
			// Monthly summaries will be calculated from dailys...
		}

		protected override void SaveToFile()
		{
			FileRepoManager.Instance.WriteOutMonthlySummaries(Summary, DatePaths, TransactionItems);
		}

		#endregion

	}
}
