using System;
using System.Collections.Generic;
using Common;
using Common.Configuration;
using Common.Db.Entities;
using Common.UiModels.WPF;
using Common.Utils.Helpers;

namespace DAL.DataBase.Managers
{
	public class TransactionItemManagerDaoAggregate : AggregateManagerBase<ITransactionItemManagerDao>, ITransactionItemManagerDao
	{
		public TransactionItemManagerDaoAggregate(List<ITransactionItemManagerDao> managers, IRepoConfiguration repoConfiguration) : base(managers, repoConfiguration)
		{
		}

		public List<ExpenseItem> GetDailyExpenses(DateTime date)
		{
			switch(LocalConfig.ReadMode)
			{
				case ReadMode.FromFile:
					return FirstFileRepoManager.GetDailyExpenses(date);
				case ReadMode.FromDb:
					return FirstDbManager.GetDailyExpenses(date);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public List<IncomeItem> GetMonthlyIncomes(DateTime date)
		{
			switch(LocalConfig.ReadMode)
			{
				case ReadMode.FromFile:
					return FirstFileRepoManager.GetMonthlyIncomes(date);
				case ReadMode.FromDb:
					return FirstDbManager.GetMonthlyIncomes(date);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public List<ExpenseItem> GetMonthlyExpenses(DateTime date)
		{
			switch(LocalConfig.ReadMode)
			{
				case ReadMode.FromFile:
					return FirstFileRepoManager.GetMonthlyExpenses(date);
				case ReadMode.FromDb:
					return FirstDbManager.GetMonthlyExpenses(date);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public void ReplaceDailyExpenses(IList<ExpenseItem> expenseItems, DateTime date)
		{
			switch(LocalConfig.SaveMode)
			{
				case SaveMode.OnlyToFile:
					AllFileRepoManagers.ForEach(dao => ReplaceDailyExpenses(expenseItems, date));
					break;
				case SaveMode.FileAndDb:
					AllManagers.ForEach(dao => ReplaceDailyExpenses(expenseItems, date));
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public void ReplaceMonthlyIncomes(IList<IncomeItem> incomeItems, DateTime date)
		{
			switch(LocalConfig.SaveMode)
			{
				case SaveMode.OnlyToFile:
					AllFileRepoManagers.ForEach(dao => ReplaceMonthlyIncomes(incomeItems, date));
					break;
				case SaveMode.FileAndDb:
					AllManagers.ForEach(dao => ReplaceMonthlyIncomes(incomeItems, date));
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}