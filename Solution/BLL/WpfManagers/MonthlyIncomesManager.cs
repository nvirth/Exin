using System;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Common.UiModels.WPF;
using Common.Utils;
using DAL.DataBase.Managers;
using DAL.RepoCommon.Managers;
using Localization;

namespace BLL.WpfManagers
{
	public class MonthlyIncomesManager : SummaryEngineBase
	{
		public MonthlyIncomesManager(bool doWork = true) : this(DateTime.Now, doWork) { }
		public MonthlyIncomesManager(DateTime dateTime, bool doWork = true) : this(new DatePaths(dateTime), doWork) { }
		public MonthlyIncomesManager(DatePaths datePaths, bool doWork = true) : base(datePaths, doWork) { }

		protected override void ReadDataMessage()
		{
			var isThisMonth = DatePaths.Date.Year == DateTime.Today.Year && DatePaths.Date.Month == DateTime.Today.Month;
			var msgStart = isThisMonth ? Localized.Instant_ : DatePaths.Date.ToString(Localized.DateFormat_year_month) + Localized._monthly_;
			MessagePresenter.Instance.WriteLineSeparator();
			MessagePresenter.Instance.WriteLine(msgStart + Localized.incomes_loading__);
		}

		protected override void ReadData()
		{
			TransactionItemManager.Instance.GetMonthlyIncomes(DatePaths.Date).ForEach(Add);
		}

		protected override void WriteData()
		{
			ReplaceMonthlyIncomes();
			TaskManager.Run((Action)ReplaceSummary);
		}

		private void ReplaceMonthlyIncomes()
		{
			try
			{
				var incomeItems = TransactionItems.Cast<IncomeItem>().ToList();
				TransactionItemManager.Instance.ReplaceMonthlyIncomes(incomeItems, DatePaths.Date);

				MessagePresenter.Instance.WriteLine(Localized.Monthly_incomes_successfully_saved_);
			}
			catch(Exception e)
			{
				ExinLog.ger.LogException(Localized.The_saving_of_the_monthly_incomes_was_unsuccessful_, e);
				throw;
			}
		}

		private void ReplaceSummary()
		{
			try
			{
				SummaryItemManager.Instance.ReplaceSummary(Summary, DatePaths.Date, TransactionItemType.Income);

				MessagePresenter.Instance.WriteLine(Localized.Income_statistics_successfully_saved_);
			}
			catch(Exception e)
			{
				throw ExinLog.ger.LogException(Localized.The_saving_of_the_income_statistics_was_unsuccessful_, e);
			}
		}

		public override void Add(TransactionItemBase item)
		{
			base.Add(item.WithMonthDate());
		}
	}
}
