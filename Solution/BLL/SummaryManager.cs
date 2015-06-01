﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.DbEntities;
using Common.UiModels.WPF;
using DAL.DataBase.Managers;

namespace BLL.WpfManagers
{
	public static class SummaryManager
	{
		private static readonly SummaryManagerCore Manager = new SummaryManagerCore();

		#region Delegated member

		public static Summary GetInterval(DateTime startDate, DateTime endDate)
		{
			return Manager.GetInterval(startDate, endDate);
		}

		public static void RefreshCache(DateTime startDate, DateTime endDate)
		{
			Manager.RefreshCache(startDate, endDate);
		}

		#endregion

	}

	class SummaryManagerCore
	{
		private Dictionary<Tuple<DateTime, DateTime>, Summary> _cache;

		public SummaryManagerCore()
		{
			_cache = new Dictionary<Tuple<DateTime, DateTime>, Summary>();
		}

		public Summary GetInterval(DateTime startDate, DateTime endDate)
		{
			var key = MakeKey(startDate, endDate);

			if(!_cache.ContainsKey(key))
				RefreshCache(key);

			return _cache[key];
		}

		private static Tuple<DateTime, DateTime> MakeKey(DateTime startDate, DateTime endDate)
		{
			startDate = startDate.Date;
			endDate = endDate.Date;
			var key = new Tuple<DateTime, DateTime>(startDate, endDate);
			return key;
		}

		public void RefreshCache(DateTime startDate, DateTime endDate)
		{
			RefreshCache(MakeKey(startDate, endDate));
		}

		private void RefreshCache(Tuple<DateTime, DateTime> key)
		{
			var startDate = key.Item1;
			var endDate = key.Item2;

			var summary = new Summary();
			List<SummaryItem> summaryItems = SummaryItemManager.GetInterval(startDate, endDate);

			foreach (var summaryItem in summaryItems)
			{
				if (summaryItem.Category == CategoryManager.GetCategoryFullIncomeSummary)
					summary.SumIn += summaryItem.Amount;
				else if (summaryItem.Category == CategoryManager.GetCategoryFullExpenseSummary)
					summary.SumOut += summaryItem.Amount;
				else
					summary.UpdateDictionary(summaryItem.Amount, summaryItem.Category);
			}

			_cache[key] = summary;
		}
	}
}