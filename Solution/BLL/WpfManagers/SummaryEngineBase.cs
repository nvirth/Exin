using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Common;
using Common.Log;
using Common.UiModels.WPF;
using Common.UiModels.WPF.Base;
using Common.Utils;
using Common.Utils.Helpers;
using Localization;
using Config = Common.Configuration.Config;

namespace BLL.WpfManagers
{
	public abstract class SummaryEngineBase : ChainedCommonBase
	{
		#region Properties, Fields

		/// <summary>
		/// This is here for make sorting permanent
		/// </summary>
		public SortDescription[] SortDescriptions { get; set; }

		public ObservableCollection<TransactionItemBase> TransactionItems { get; set; }

		public Summary Summary { get; set; }
		protected DatePaths DatePaths;

		private bool _isModified;
		public bool IsModified
		{
			get { return _isModified; }
			set
			{
				_isModified = value;
				OnPropertyChanged();
			}
		}

		private bool _isReady;
		/// <summary>
		/// Once it's set, should never be unset
		/// </summary>
		public bool IsReady
		{
			get { return _isReady; }
			set
			{
				_isReady = value;
				if(_isReady)
					TransactionItems.CollectionChanged +=
						(sender, e) => IsModified = true;

				OnPropertyChanged();
			}
		}

		public bool HasError { get; set; }

		#endregion

		#region Ctors

		protected SummaryEngineBase(bool doWork = true) : this(DateTime.Now, doWork) { }
		protected SummaryEngineBase(DateTime dateTime, bool doWork = true) : this(new DatePaths(dateTime), doWork) { }
		protected SummaryEngineBase(DatePaths datePaths, bool doWork = true)
		{
			TransactionItems = new ObservableCollection<TransactionItemBase>();
			Summary = new Summary();
			DatePaths = datePaths;

			// !!! Virtual member call in ctor !!!
			if(doWork)
				ReadData();
		}

		#endregion

		#region Methods

		#region Add, Remove, GetEqual

		public virtual void Add(TransactionItemBase item)
		{
			item.AmountChanged +=
				(tib, plusAmount) =>
				{
					Summary.Update(item, plusAmount);
					TransactionItems.Remove(tib);
					TransactionItems.InsertIntoSorted(tib);
				};

			item.PropertyChanged +=
				(sender, e) =>
				{
					if(!IsModified)
						IsModified = true;
				};

			item.CategoryChanged +=
				(tib, oldCategory) =>
				{
					var expenseItem = tib as ExpenseItem;
					if(expenseItem == null)
						return;

					var mockEiForRemove = new ExpenseItem() { Amount = -expenseItem.Amount, Category = oldCategory };
					Summary.Update(mockEiForRemove);

					var mockEiForAdd = new ExpenseItem() { Amount = +expenseItem.Amount, Category = expenseItem.Category };
					Summary.Update(mockEiForAdd);

					TransactionItems.Remove(expenseItem);
					TransactionItems.InsertIntoSorted(expenseItem);
				};

			TransactionItems.InsertIntoSorted(item);
			Summary.Update(item);
		}

		public void Remove(TransactionItemBase item)
		{
			if(TransactionItems.Contains(item))
			{
				TransactionItems.Remove(item);
				Summary.Update(item, item.Amount * -1);
			}
		}

		#endregion

		public virtual void ReadData()
		{
			try
			{
				ReadDataMessage();

				switch(Config.Repo.ReadMode)
				{
					case ReadMode.FromFile:
						ReadDataFromFile();
						break;
					case ReadMode.FromDb:
						ReadDataFromDb();
						break;
					default:
						throw new NotImplementedException(Localized.The_reading_of_data_is_not_implemented_for_this_ + Config.Repo.ReadMode);
				}
			}
			catch(Exception e)
			{
                ExinLog.ger.LogException(Localized.SummaryEngineBase_ReadData_failed_, e);
				HasError = true;
			}
			IsReady = true;
		}

		public void Save()
		{
			if(!Validate())
			{
				MessagePresenter.Instance.WriteLine("");
				throw new Exception(Localized.The_saving_of_the_data_was_unsuccessful__there_were_invalid_values_among_them__Fix_them__then_save_again);
			}

			SaveToFile();

			if(Config.Repo.SaveMode == SaveMode.FileAndDb)
			{
				SaveToDb();
				SaveSummariesToDb();
			}

			IsModified = false;
		}

		private bool Validate()
		{
			var isValid = true;

			// ExpenseItem's Category is not validated... But you can't ruin it via the GUI
			foreach(var tib in TransactionItems)
			{
				var errorMessage = tib.DoValidation();
				isValid = isValid & string.IsNullOrWhiteSpace(errorMessage);
			}

			return isValid;
		}

		#endregion

		#region Abstract

		protected abstract void ReadDataMessage();
		protected abstract void ReadDataFromFile();
		protected abstract void ReadDataFromDb();

		protected abstract void SaveToFile();
		protected abstract void SaveToDb();
		protected abstract void SaveSummariesToDb();

		#endregion
	}
}
