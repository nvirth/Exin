using System;
using System.Threading.Tasks;
using Common;
using Common.Utils;
using DAL.DataBase.Managers;
using DAL.RepoCommon.Managers;
using Localization;

namespace BLL.WpfManagers
{
	public class MonthlyExpensesManager : SummaryEngineBase
	{
		#region Ctors

		public MonthlyExpensesManager(bool doWork = true) : this(DateTime.Now, doWork) { }
		public MonthlyExpensesManager(DateTime dateTime, bool doWork = true) : this(new DatePaths(dateTime), doWork) { }
		public MonthlyExpensesManager(DatePaths datePaths, bool doWork = true) : base(datePaths, doWork) { }

		#endregion

		#region Methods

		public override void SaveData()
		{
			// Monthly expenses don't save anything
		}

		protected override void WriteData()
		{
			// Monthly expenses don't save anything
		}

		protected override void ReadDataMessage()
		{
			MessagePresenter.Instance.WriteLineSeparator();
			MessagePresenter.Instance.WriteLine(Localized.Monthly_summary_ + DatePaths.Date.ToString(Localized.DateFormat_year_month));
		}

		protected override void ReadData()
		{
			TaskManager.Run(() => TransactionItemManager.Instance.GetMonthlyExpenses(DatePaths.Date).ForEach(Add));
		}

		#endregion
	}
}
