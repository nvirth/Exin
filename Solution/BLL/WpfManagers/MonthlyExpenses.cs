using System;
using System.Threading.Tasks;
using Common;
using Common.Utils;
using DAL.DataBase.Managers;
using Localization;

namespace BLL.WpfManagers
{
	public class MonthlyExpenses : SummaryEngineBase
	{
		#region Ctors

		public MonthlyExpenses(bool doWork = true) : this(DateTime.Now, doWork) { }
		public MonthlyExpenses(DateTime dateTime, bool doWork = true) : this(new DatePaths(dateTime), doWork) { }
		public MonthlyExpenses(DatePaths datePaths, bool doWork = true) : base(datePaths, doWork) { }

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
			Task.Run(() => TransactionItemManager.Instance.GetMonthlyExpenses(DatePaths.Date).ForEach(Add));
		}

		#endregion
	}
}
