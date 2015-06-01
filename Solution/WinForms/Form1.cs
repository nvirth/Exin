using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BLL;
using BLL.WpfManagers;
using Common;
using Common.DbEntities;
using Common.UiModels.WinForms;
using Common.UiModels.WPF;
using Common.Utils;
using Common.Utils.Helpers;
using DAL;
using DAL.DataBase.Managers;
using UtilsShared;
using WinForms.Annotations;

namespace WinForms
{
	public partial class Form1 : Form, INotifyPropertyChanged
	{
		#region Properties

		#region SummaryEngines

		private DailyExpenses _dailyExpenses;

		public DailyExpenses DailyExpenses
		{
			get { return _dailyExpenses; }
			set
			{
				_dailyExpenses = value;
				OnPropertyChanged();
			}
		}

		private MonthlyExpenses _monthlyExpenses;

		public MonthlyExpenses MonthlyExpenses
		{
			get { return _monthlyExpenses; }
			set
			{
				_monthlyExpenses = value;
				OnPropertyChanged();
			}
		}

		private MonthlyIncomes _monthlyIncomes;

		public MonthlyIncomes MonthlyIncomes
		{
			get { return _monthlyIncomes; }
			set
			{
				_monthlyIncomes = value;
				OnPropertyChanged();
			}
		}

		#endregion

		public BindingList<ExpenseItemDgvViewModel> DailyExpensesPhantom { get; set; }
		public BindingList<ExpenseItemDgvViewModel> MonthlyExpensesPhantom { get; set; }
		public BindingList<ExpenseItemDgvViewModel> MonthlyIncomesPhantom { get; set; }

		#endregion

		#region Init

		public Form1()
		{
			//GeneralFunctions.SetDefaultCultureToEnglish();	// For Exeption messages <-- won't work (english month names)

			InitializeComponent();
			InitMessagePresenter(); // Important to be here!
			Init();
		}

		private async void Init()
		{
			await GetDailyExpenses(DateTime.Today, useStarter: true);
			MonthlyExpenses = new MonthlyExpenses(DateTime.Today, doWork: false);
			MonthlyIncomes = new MonthlyIncomes(DateTime.Today, doWork: false);

			CategoryBindingSource.DataSource = CategoryManager.GetAllValid();
			UnitBindingSource.DataSource = UnitManager.GetAllValid();

			Validation.ErrorProvider = errorProvider1;	// Init the Validation class
			//SQLiteSpecific.InitSqliteFileIfNeeded(); //-> Initialized in Starter
		}

		private void InitMessagePresenter()
		{
			if(!this.IsHandleCreated) // without this, we will get an Exception!
				this.CreateHandle(); // (when tbLog.Invoke will be called)

			MessagePresenterManager.WireToWinFormsRichTextBox(tbLog);
		}

		#endregion

		#region Event Handlers

		private void ClearLogButton_OnClick(object sender, EventArgs e)
		{
			tbLog.Clear();
		}

		private void MainTabControl_SelectedIndexChanged(object sender, EventArgs e)
		{
			SwitchMainTab();
		}

		private void SwitchMainTab()
		{
			var date = DatePicker.Value.Date;

			switch((TabSummaryNumber)mainTabControl.SelectedIndex)
			{
				case TabSummaryNumber.DailyExpenses:
					GetDailyExpenses(date);
					break;

				case TabSummaryNumber.MonthlyExpenses:
					GetMonthlyExpenses(date);
					break;

				case TabSummaryNumber.MonthlyIncomes:
					GetMonthlyIncomes(date);
					break;

				default:
					throw new Exception();
			}
		}

		private void DatePicker_ValueChanged(object sender, EventArgs e)
		{
			SwitchMainTab();	// This will refresh the data on the actually opened tab
		}

		private void SaveDailyExpensesButton_Click(object sender, EventArgs e)
		{
			if (!ValidateDailyExpenses())
				return;

			Save(saveDailyExpenses: true, saveMonthlyIncomes: false);
		}

		private void SaveMonthlyIncomesButton_Click(object sender, EventArgs e)
		{
			if(!ValidateMonthlyIncomes())
				return;

			// We have to convert the data from ExpenseItemDgvViewModel to IncomeItem
			// before calling the Save method, because it works with that type

			var originalCollection = MonthlyIncomes.TransactionItems;

			var incomeItems = MonthlyIncomes.TransactionItems
				.Cast<ExpenseItemDgvViewModel>()
				.Select(ExpenseItemDgvViewModel.ToIncomeItem)
				.ToList();
			MonthlyIncomes.TransactionItems =
				new ObservableCollection<TransactionItemBase>(incomeItems);

			Save(saveDailyExpenses: false, saveMonthlyIncomes: true);

			// At the end, we restore the original state

			MonthlyIncomes.TransactionItems = originalCollection;
		}

		private void RemoveExpenseButton_Click(object sender, EventArgs e)
		{
			foreach(DataGridViewRow selectedRow in dailyExpensesDgv.SelectedRows)
			{
				//var eivm = (ExpenseItemDgvVieModel) selectedRow.DataBoundItem;
				DailyExpenses.TransactionItems.RemoveAt(selectedRow.Index);
			}
		}

		private void RemoveIncomeButton_Click(object sender, EventArgs e)
		{
			foreach(DataGridViewRow selectedRow in monthlyIncomesDgv.SelectedRows)
			{
				MonthlyIncomes.TransactionItems.RemoveAt(selectedRow.Index);
			}
		}

		private void NewExpenseButton_Click(object sender, EventArgs e)
		{
			AddNewExpense();
		}

		private void AddNewExpense(bool validate = true)
		{
			if(validate && !ValidateDailyExpenses())
				return;

			var newItem = new ExpenseItemDgvViewModel()
			{
				Date = DatePicker.Value.Date
			};
			DailyExpenses.Add(newItem);

			DailyExpensesBindingSource.Position = DailyExpenses.TransactionItems.Count - 1;
		}

		private void NewIncomeButton_Click(object sender, EventArgs e)
		{
			AddNewIncome();
		}

		private void AddNewIncome(bool validate = true)
		{
			if(validate && !ValidateMonthlyIncomes())
				return;

			var newItem = new ExpenseItemDgvViewModel()
			{
				Date = DatePicker.Value.Date
			};
			MonthlyIncomes.Add(newItem);

			MonthlyIncomesBindingSource.Position = MonthlyIncomes.TransactionItems.Count - 1;
		}

		private bool ValidateDailyExpenses()
		{
			return !Validation.HasValidationErrors(tabDailyExpenses.Controls);
		}
	
		private bool ValidateMonthlyIncomes()
		{
			return !Validation.HasValidationErrors(tabMonthlyIncomes.Controls);
		}

		#endregion

		#region Helpers

		private async Task GetDailyExpenses(DateTime dateTime, bool useStarter = false)
		{
			DailyExpenses = useStarter
				? await Starter.Start(dateTime)
				: new DailyExpenses(dateTime);

			DailyExpensesPhantom = GetTransactionItems(dateTime, DailyExpenses, DailyExpensesBindingSource);

			labelDailySummary.DataBindings.Clear();
			labelDailySummary.DataBindings.Add("Text", DailyExpenses.Summary, "SumOut", true, DataSourceUpdateMode.OnPropertyChanged, null, "c0");

			if(DailyExpensesPhantom.Count == 0)
				AddNewExpense(validate: false);
		}

		private void GetMonthlyExpenses(DateTime dateTime)
		{
			MonthlyExpenses = new MonthlyExpenses(dateTime, doWork: false);

			MonthlyExpenses.Summary.PropertyChanged +=
				(sender, args) =>
				{
					if(args.PropertyName == MonthlyExpenses.Summary.Property(s => s.SumOut))
					{
						Action action = () => labelMonthlySummary.Text = MonthlyExpenses.Summary.SumOut.ToString("c0");
						action.InvokeThreadSafe(this);
					}
				};

			MonthlyExpensesPhantom = GetTransactionItems(dateTime, MonthlyExpenses, MonthlyExpensesBindingSource);
			MonthlyExpenses.ReadData();
		}

		private void GetMonthlyIncomes(DateTime dateTime)
		{
			MonthlyIncomes = new MonthlyIncomes(dateTime, doWork: false);

			MonthlyIncomes.Summary.PropertyChanged +=
			(sender, args) =>
			{
				if(args.PropertyName == MonthlyIncomes.Summary.Property(s => s.SumOut)
					|| args.PropertyName == MonthlyIncomes.Summary.Property(s => s.SumIn))
				{
					Action action =
						() =>
						{
							// In the case of IncomeItems, the old engine don't tolerate the new view models...
							var sum = MonthlyIncomes.Summary.SumOut + MonthlyIncomes.Summary.SumIn;
							labelMonthlyIncomeSum.Text = sum.ToString("c0");
						};
					action.InvokeThreadSafe(this);
				}
			};

			MonthlyIncomesPhantom = GetTransactionItems(dateTime, MonthlyIncomes, MonthlyIncomesBindingSource);
			MonthlyIncomes.ReadData();

			if(MonthlyIncomesPhantom.Count == 0)
				AddNewIncome(validate: false);
		}

		private BindingList<ExpenseItemDgvViewModel> GetTransactionItems(DateTime dateTime, SummaryEngineBase summaryEngine, BindingSource bindingSource)
		{
			var phantomCollection = new BindingList<ExpenseItemDgvViewModel>();

			for(int i = 0; i < summaryEngine.TransactionItems.Count; i++)
			{
				ConvertToViewModel(summaryEngine, i, phantomCollection);
			}

			summaryEngine.TransactionItems.CollectionChanged +=
				CollectionChangedHandler(summaryEngine, bindingSource, phantomCollection);

			bindingSource.DataSource = phantomCollection;
			return phantomCollection;
		}

		private NotifyCollectionChangedEventHandler CollectionChangedHandler(SummaryEngineBase summaryEngine, BindingSource bindingSource, BindingList<ExpenseItemDgvViewModel> phantomCollection)
		{
			return (sender, args) => CollectionChangedHandlerCore(args, summaryEngine, bindingSource, phantomCollection);
		}

		private void CollectionChangedHandlerCore(NotifyCollectionChangedEventArgs args, SummaryEngineBase summaryEngine, BindingSource bindingSource, BindingList<ExpenseItemDgvViewModel> phantomCollection)
		{
			switch(args.Action)
			{
				case NotifyCollectionChangedAction.Add:
					int position = args.NewStartingIndex;
					for(int i = 0; i < args.NewItems.Count; i++)
					{
						var newItemConverted = args.NewItems[i] as ExpenseItemDgvViewModel;

						if(newItemConverted == null) // In some async cases, we have to convert them posteriorly
						{
							var idx = summaryEngine.TransactionItems.IndexOf(args.NewItems[i] as TransactionItemBase);
							ConvertToViewModel(summaryEngine, idx);

							newItemConverted = summaryEngine.TransactionItems[idx] as ExpenseItemDgvViewModel;
							if(newItemConverted == null)
								throw new Exception();
						}

						Action action =
							() =>
							{
								phantomCollection.Insert(position, newItemConverted);
								bindingSource.Position = position; // Syncronise the binding source <-- Selected item in the DataGridView
							};
						action.InvokeThreadSafe(this);
						position++;
					}

					break;

				case NotifyCollectionChangedAction.Remove:
					for(int i = 0; i < args.OldItems.Count; i++)
						phantomCollection.RemoveAt(args.OldStartingIndex);
					break;

				case NotifyCollectionChangedAction.Replace:
					// Replace is an allowed case. When the ExpenseItem instances in a
					// SummaryEngine's "TransactionItems" list are placed asynchronously
					// (typically by MonthlyExpenses); we will run into this case section,
					// because the event handles are wired already
					//
					// But, we don't have to do anything here (except to don't-throw-exception)
					break;

				default:
					throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Converts TransactionItemBase inheritors to <see cref="ExpenseItemDgvViewModel"/>; the view-model,
		/// which the dataGridViews can work together. Replaces the old item in the ..sum.engine...TranscationItems
		/// collection to the new one; and if a phantomCollection is given too (which to are bound the dataGridViews'
		/// dataSource), then adds the item to it as well.
		/// </summary>
		private void ConvertToViewModel(SummaryEngineBase summaryEngine, int i, BindingList<ExpenseItemDgvViewModel> phantomCollection = null)
		{
			var vm = ExpenseItemDgvViewModel.CopyCtor(summaryEngine.TransactionItems[i]);

			summaryEngine.TransactionItems[i] = vm;

			if(phantomCollection != null)
			{
				Action b = () => phantomCollection.Add(vm);
				b.InvokeThreadSafe(this);
			}
		}

		private bool Save(bool saveDailyExpenses = true, bool saveMonthlyIncomes = true)
		{
			var errorMsg = "";

			if(saveDailyExpenses && DailyExpenses.IsModified)
			{
				MessagePresenter.WriteLineSeparator();
				try
				{
					DailyExpenses.Save();
					MessagePresenter.WriteLine("Napi kiadások mentése sikeres. ");
				}
				catch(Exception ex)
				{
					var msg = "Nem sikerült menteni a napi kiadásokat!\r\n" + ex.Message + "\r\n";
					MessagePresenter.WriteError(msg);
					MessagePresenter.WriteException(ex);
					errorMsg += msg;
				}
			}
			if(saveMonthlyIncomes && MonthlyIncomes.IsModified)
			{
				MessagePresenter.WriteLineSeparator();
				try
				{
					MonthlyIncomes.Save();
					MessagePresenter.WriteLine("Havi bevételek mentése sikeres. ");
				}
				catch(Exception ex)
				{
					var msg = "Nem sikerült menteni a havi bevételeket!\r\n" + ex.Message + "\r\n";
					MessagePresenter.WriteError(msg);
					MessagePresenter.WriteException(ex);
					errorMsg += msg;
				}
			}

			if(!String.IsNullOrEmpty(errorMsg))
				return false;

			return true;
		}

		#endregion

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			var handler = PropertyChanged;
			if(handler != null)
				handler(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion

		#region Validation

		private void tbExpenseTitle_Validating(object sender, CancelEventArgs e)
		{
			Validation.ValidateRequired(sender, e, "Megnevezés", tbExpenseTitle.Text);
		}

		private void tbExpenseAmount_Validating(object sender, CancelEventArgs e)
		{
			Validation.ValidateRequiredPositiveNumber(sender, e, "Összeg", tbExpenseAmount.Text);
		}

		private void tbExpenseQuantity_Validating(object sender, CancelEventArgs e)
		{
			Validation.ValidateRequiredPositiveNumber(sender, e, "Mennyiség", tbExpenseQuantity.Text);
		}

		private void tbIncomeTitle_Validating(object sender, CancelEventArgs e)
		{
			Validation.ValidateRequired(sender, e, "Megnevezés", tbIncomeTitle.Text);
		}

		private void tbIncomeAmount_Validating(object sender, CancelEventArgs e)
		{
			Validation.ValidateRequiredPositiveNumber(sender, e, "Összeg", tbIncomeAmount.Text);
		}

		#endregion
	}
}
