using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;
using BLL;
using BLL.WpfManagers;
using Common;
using Common.Annotations;
using Common.Configuration;
using Common.Db.Entities;
using Common.Log;
using Common.Log.New;
using Common.UiModels.WPF;
using Common.Utils;
using Common.Utils.Helpers;
using DAL;
using DAL.DataBase;
using DAL.DataBase.Managers;
using DAL.RepoCommon;
using DAL.RepoCommon.Managers;
using Localization;
using WPF.ViewModels;
using WPF.Utils;
using WPF.ValueConverters;

namespace WPF
{
	public partial class MainWindow : Window, INotifyPropertyChanged
	{
		#region Properties, Fields

		#region MainWindowViewmodel

		private MainWindowViewModel _viewModel;
		public MainWindowViewModel ViewModel
		{
			get { return _viewModel; }
			set
			{
				_viewModel = value;
				OnPropertyChanged();
			}
		}

		#endregion

		#endregion

		#region Ctor, Init

		public MainWindow()
		{
			InitializeComponent();
			MessagePresenterManager.WireToRichTextBox(LogTB, Dispatcher);
			LogInit.InitWpfAppUiLoggers(LogTB);

			InitFirstRepoRootIfNeeded();
			Config.InitRepo();
			ShowConfig();

			StaticInitializer.InitAllStatic();
			InitOptimize();

			LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

			// For init, these have to be off. These will be turned on again at the end of ContentRendered
			MainTabControl.SelectionChanged -= MainTabControl_SelectionChanged;
			this.IsEnabled = false;

			// ContentRendered, and not Loaded; because if these would be in Loaded, all the data provided
			// in the way would be validated at once after the Loaded event finished (and not during the event).
			// If we call these from ContentRendered, the Framework already set its handlers on the PropertyChanged
			// events; and so can do (an do) all the validations requested in the event at once (and not all in one
			// at the end)
			this.ContentRendered +=
				async (sender, args) => {
					await Init();

					// For init, we turned these off in the ctor
					MainTabControl.SelectionChanged += MainTabControl_SelectionChanged;
					this.IsEnabled = true;
					ActualExpenseGP.TitleTb.Focus();
				};
		}

		private static void InitOptimize()
		{
			// Somehow, if we use EntityFramework (6), the first access to the Category
			// table is slow - if it's called from the ContentRendered event's handler.
			// (it's independent form the DB type, so MsSql and Sqlite also do the same)
			// And somehow, if we force the app to get the Categories from the DB before
			// entering the eventhandler, we will get them faster
			if(Config.Repo.Settings.ReadMode != ReadMode.FromDb || Config.Repo.Settings.DbAccessMode != DbAccessMode.EntityFramework)
				return;

			// In case we use SQLite and do not have the DB file yet, first we need to init that...
			// We will do it in the Init method
			if(SQLiteSpecific.IsDbOk())
			{
				var categoryNone = CategoryManager.Instance.GetCategoryNone;
			}
		}

		public DateTime StartDate
		{
			get
			{
#if DEBUG
				return new DateTime(2015, 1, 6); // According to FilerepoDeveloper.template
#else
				return DateTime.Today;
#endif
			}
		}


		private async Task Init()
		{
			await SQLiteSpecific.InitSqliteFileIfNeeded(); // Has to be the first!

			InitViewModel(StartDate);
			InitSummaryDatePicker(StartDate);
			ItemsControlSorter.Init(this);
			InitStatistics();
		}

		private void InitSummaryDatePicker(DateTime startDate)
		{
			// In XAML, the event is bound; but the TextChange event fires this twice... Fix with ..DateChanger
			SummaryDatePicker.SelectedDateChanged -= SummaryDatePicker_SelectedDateChanged;
			SummaryDatePicker.SelectedDate = startDate;
			ViewModel.DateChanger = new DatePickerFromCodeDateChanger(SummaryDatePicker, SummaryDatePicker_SelectedDateChanged);

			if(!Config.MainSettings.UserSettings.AllowsFutureDate)
				SummaryDatePicker.DisplayDateEnd = DateTime.Today;
		}

		private void InitViewModel(DateTime startDate)
		{
			var viewModel = new MainWindowViewModel();

			viewModel.DailyExpensesViewModel.Manager = new DailyExpensesManager(startDate, /*doWork*/ true);
			viewModel.DailyExpensesViewModel.ListView = DailyExpensesLV;
			viewModel.DailyExpensesViewModel.TitleTextBox = ActualExpenseGP.TitleTb;

			viewModel.MonthlyExpensesViewModel.Manager = new MonthlyExpensesManager(startDate, /*doWork*/ false);
			viewModel.MonthlyExpensesViewModel.ListView = MonthlyExpensesLV;

			viewModel.MonthlyIncomesViewModel.Manager = new MonthlyIncomesManager(startDate, /*doWork*/ false);
			viewModel.MonthlyIncomesViewModel.ListView = MonthlyIncomesLV;
			viewModel.MonthlyIncomesViewModel.TitleTextBox = ActualIncomeGP.TitleTb;

			viewModel.Statistics.SetDateToMonthly(startDate);

			viewModel.ClipboardManager.MainWindow = this;

			ViewModel = viewModel;

			// These have to be assigned delayed, to avoid their validation
			viewModel.DailyExpensesViewModel.ActualExpenseItem = new ExpenseItem();
			viewModel.MonthlyIncomesViewModel.ActualIncomeItem = new IncomeItem();
		}

		private void InitStatistics()
		{
			//-- Set the Chart's Y Axis's Maximum property
			int yAxisMax;
			var converter = new ChartYAxisMaxConverter();

			var yAxisMaxConfig = Config.Repo.Settings.UserSettings.StatYAxisMax;
			if(yAxisMaxConfig.HasValue)
				yAxisMax = yAxisMaxConfig.Value;
			else
				switch(Config.Repo.Settings.Currency)
				{
					case Currenies.USD:
						yAxisMax = 2;
						break;
					case Currenies.HUF:
						yAxisMax = 200;
						break;
					default:
						yAxisMax = 0;
						ExinLog.ger.LogError("Unexpected currency by Statistics/YAxisMax default value");
						break;
				}

			YAxis.Maximum = converter.Convert(yAxisMax);
		}

		private static void InitFirstRepoRootIfNeeded()
		{
			var firstRepoPaths = new RepoPaths(Config.FirstRepoRootPath);
			if(firstRepoPaths.CheckRepo())
				return;

			MessagePresenter.Instance.WriteError(string.Format(Localized.Could_not_find_the_Exin_s_work_directory_here__0__FORMAT__, Config.FirstRepoRootPath));
			MessagePresenter.Instance.WriteLine(Localized.The_app_will_now_create_the_necessary_directories);
			MessagePresenter.Instance.WriteLine();

			firstRepoPaths.InitRepo();

			MessagePresenter.Instance.WriteLine();
			MessagePresenter.Instance.WriteLine(Localized.All_created_successfully_);
			MessagePresenter.Instance.WriteLineSeparator();
		}

		private static void ShowConfig()
		{
			MessagePresenter.Instance.WriteLine(Localized.The_Exin_expenses_incomes_summarizer_application_welcomes_you_);
			MessagePresenter.Instance.WriteLine(Localized.The_application_s_configuration_);
			MessagePresenter.Instance.WriteLine(" - " + Config.Repo.Settings.ReadMode.ToLocalizedDescriptionString());
			MessagePresenter.Instance.WriteLine(" - " + Config.Repo.Settings.SaveMode.ToLocalizedDescriptionString());
			MessagePresenter.Instance.WriteLine(" - " + Config.Repo.Settings.DbAccessMode.ToLocalizedDescriptionString());
			MessagePresenter.Instance.WriteLine(" - " + Config.Repo.Settings.DbType.ToLocalizedDescriptionString());
			MessagePresenter.Instance.WriteLineSeparator();
		}

		#endregion

		#region Event handler methods

		#region Others...

		private async void MainWindow_OnClosing(object sender, CancelEventArgs e)
		{
			var messageBoxResult = ViewModel.SaveWithPromptYesNoCancel();
			if(messageBoxResult == MessageBoxResult.Cancel)
			{
				e.Cancel = true; // Exit revoked, so do not exit
			}
			else
			{
				this.Hide();
				e.Cancel = true; // Just because we await the next call
				await TaskManager.WaitBackgroundTasks(); // Wait all running async tasks to finish before exiting
				this.Closing -= MainWindow_OnClosing; // Do not call this method again
				this.Close();
			}
		}

		private void MainWindow_PreviewKeyUp(object sender, KeyEventArgs e)
		{
			var onlyControl = Keyboard.Modifiers == ModifierKeys.Control;
			var onlyAlt = Keyboard.Modifiers == ModifierKeys.Alt;
			var onlyAltControl = Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Alt);

			if(onlyControl || onlyAlt || onlyAltControl)
			{
				var date = SummaryDatePicker.SelectedDate ?? DateTime.Today;
				var key = e.Key == Key.System ? e.SystemKey : e.Key;
				bool handled = true;

				if(onlyControl)
				{
					switch(key)
					{
						case Key.S:
							SaveAccordingToOpenedTab();
							break;
						default:
							handled = false;
							break;
					}
				}
				else if(onlyAlt)
				{
					switch(key)
					{
						case Key.Up:
							date = date.AddMonths(1);
							break;
						case Key.Down:
							date = date.AddMonths(-1);
							break;
						case Key.Right:
							date = date.AddDays(1);
							break;
						case Key.Left:
							date = date.AddDays(-1);
							break;
						case Key.End:
							date = DateTime.Today;
							break;
						default:
							handled = false;
							break;
					}
				}
				else if(onlyAltControl)
				{
					switch(key)
					{
						case Key.Up:
							date = date.AddYears(1);
							break;
						case Key.Down:
							date = date.AddYears(-1);
							break;
						default:
							handled = false;
							break;
					}
				}
				ViewModel.DateChanger.ChangeSelectedDate(date);
				e.Handled = handled;
			}
		}

		private void SummaryDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
		{
			// -- Save

			ViewModel.SaveWithPromptYesNo();

			// -- Change
			var selectedDate = (e.AddedItems[0] as DateTime?) ?? DateTime.Today;
			var originalDate = (e.RemovedItems[0] as DateTime?) ?? DateTime.Today;

			var isNewMonth = selectedDate.Month != originalDate.Month || selectedDate.Year != originalDate.Year;
			if(isNewMonth)
			{
				ViewModel.MonthlyExpensesViewModel.Manager = new MonthlyExpensesManager(selectedDate, /*doWork*/ false);
				ViewModel.MonthlyIncomesViewModel.Manager = new MonthlyIncomesManager(selectedDate, /*doWork*/ false);
			}
			ViewModel.DailyExpensesViewModel.Manager = new DailyExpensesManager(selectedDate, /*doWork*/ false); // There is anyway a new day
			ViewModel.Statistics.SetDateToMonthly(selectedDate);

			SwitchMainTab();
		}

		private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			SwitchMainTab();
		}

		private void ClearLogButton_OnClick(object sender, RoutedEventArgs e)
		{
			LogTB.Document.Blocks.Clear();
		}

		private void ListViewsHeaders_OnClick_Sort(object sender, RoutedEventArgs e)
		{
			ItemsControlSorter.SortControl(e);
		}

		private void ListViewsQuantityHeader_OnClick_Sort(object sender, RoutedEventArgs e)
		{
			var propertyName = ViewModel.DailyExpensesViewModel.ActualExpenseItem.Property(ei => ei.Quantity);
			ItemsControlSorter.SortControl(e, sortByProperty: propertyName);
			e.Handled = true;
		}

		#endregion


		#region DailyExpenses

		private void DailyExpensesLV_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if(!CheckIfLisviewitemGotClicked(e))
				return; // DoubleClick is in ListView, but not in a ListViewItem

			if(DailyExpensesLV.SelectedIndex != -1)
			{
				var selectedExpenseItem = DailyExpensesLV.SelectedItem as ExpenseItem;
				ViewModel.DailyExpensesViewModel.ActualExpenseItem = selectedExpenseItem;
			}
		}

		private void DailyExpensesLV_KeyUp(object sender, KeyEventArgs e)
		{
			if(e.Key == Key.Delete)
			{
				var selectedIndex = DailyExpensesLV.SelectedIndex;
				ViewModel.DailyExpensesViewModel.Remove();

				DailyExpensesLV.SelectedIndex = selectedIndex <= (DailyExpensesLV.Items.Count - 1)
					? selectedIndex
					: (DailyExpensesLV.Items.Count - 1); // It works for empty list (-1)

				DailyExpensesLV.Focus();

				e.Handled = true;
			}
			else
			{
				HandleClipboardKeyCombinations(e);
			}
		}

		private void DailyExpensesLV_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			OnListViewSelectionChanged(DailyExpensesLV, ViewModel.DailyExpensesViewModel.Manager);
		}

		#endregion

		#region MonthlyExpenses

		private void RedoMonthlyExpensesButton_OnClick(object sender, RoutedEventArgs e)
		{
			var selectedDate = SummaryDatePicker.SelectedDate ?? DateTime.Now;
			ViewModel.MonthlyExpensesViewModel.Manager= new MonthlyExpensesManager(selectedDate, /* doWork */ true);
		}

		private void MonthlyExpensesLV_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if(!CheckIfLisviewitemGotClicked(e))
				return; // DoubleClick is in ListView, but not in a ListViewItem

			if(MonthlyExpensesLV.SelectedIndex != -1)
			{
				var selectedExpenseItem = (ExpenseItem)MonthlyExpensesLV.SelectedItem;
				SummaryDatePicker.SelectedDate = selectedExpenseItem.Date;
				MainTabControl.SelectedIndex = (int)TabSummaryNumber.DailyExpenses;

				var equalExpenseItem = ViewModel.DailyExpensesViewModel.Manager.GetTheEqual(selectedExpenseItem);
				if(equalExpenseItem == null)
				{
					MessagePresenter.Instance.WriteError(Localized.The_chosen_expense_item_does_not_exists_anymore__);
					return;
				}

				ViewModel.DailyExpensesViewModel.ActualExpenseItem = equalExpenseItem;
				DailyExpensesLV.SelectedItem = equalExpenseItem;

				//http://stackoverflow.com/questions/13955340/keyboard-focus-does-not-work-on-text-box-in-wpf
				//The code you posted should set the focus correctly, so something must be occurring afterwards
				//to move Keyboard Focus out of your TextBox. By setting focus at a later dispatcher priority, 
				//you'll be ensuring that setting keyboard focus to your SearchCriteriaTextBox gets done last.
				Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() => ActualExpenseGP.TitleTb.Focus()));
				//Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() => DailyExpensesLV.Focus()));
			}
		}

		private void MonthlyExpensesLV_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			OnListViewSelectionChanged(MonthlyExpensesLV, ViewModel.MonthlyExpensesViewModel.Manager);
		}

		#endregion

		#region MonthlyIncomes

		private void MonthlyIncomesLV_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if(!CheckIfLisviewitemGotClicked(e))
				return; // DoubleClick is in ListView, but not in a ListViewItem

			if(MonthlyIncomesLV.SelectedIndex != -1)
			{
				var selectedIncomeItem = MonthlyIncomesLV.SelectedItem as IncomeItem;
				ViewModel.MonthlyIncomesViewModel.ActualIncomeItem = selectedIncomeItem;
			}
		}

		private void MonthlyIncomesLV_KeyUp(object sender, KeyEventArgs e)
		{
			if(e.Key == Key.Delete)
			{
				var selectedIndex = MonthlyIncomesLV.SelectedIndex;
				ViewModel.MonthlyIncomesViewModel.Remove();

				MonthlyIncomesLV.SelectedIndex = selectedIndex <= (MonthlyIncomesLV.Items.Count - 1)
					? selectedIndex
					: (MonthlyIncomesLV.Items.Count - 1); // It works for empty list (-1)

				MonthlyIncomesLV.Focus();

				e.Handled = true;
			}
			else
			{
				HandleClipboardKeyCombinations(e);
			}
		}

		private void MonthlyIncomesLV_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			OnListViewSelectionChanged(MonthlyIncomesLV, ViewModel.MonthlyIncomesViewModel.Manager);
		}

		#endregion

		#region Statistics

		private void RedoStatisticsButton_OnClick(object sender, RoutedEventArgs e)
		{
			ViewModel.Statistics.Refresh();
		}

		private void YAxisMaxTB_OnLostFocus(object sender, RoutedEventArgs e)
		{
			var textBox = (TextBox)sender;
			var yAxisMax = int.Parse(textBox.Text);
			Config.Repo.Settings.UserSettings.StatYAxisMax = yAxisMax;
		}

		#endregion

		#region Common

		private void OnListViewSelectionChanged(ListView listView, SummaryEngineBase summaryEngine)
		{
			if(listView.SelectedItems.Count > 1)
			{
				var sumSelection = Summary.Summarize(listView.SelectedItems.Cast<TransactionItemBase>());
				summaryEngine.SummaryForSelection = sumSelection;
			}
			else
			{
				summaryEngine.SummaryForSelection = null;
			}
		}

		#endregion

		#endregion

		#region Helper methods

		/// <summary>
		/// Note that this would not work as a Dictionary, 
		/// unless you always refresh the object references
		/// </summary>
		public SummaryEngineBase ListView2SummaryEngineBase(ListView listView, bool returnNull = false)
		{
			if(listView == null)
				throw new ArgumentNullException("listView", "MainWindow.ListView2SummaryEngineBase: Argument 'listView' cannot be null. ");

			if(listView == DailyExpensesLV)
				return ViewModel.DailyExpensesViewModel.Manager;
			else if(listView == MonthlyExpensesLV)
				return ViewModel.MonthlyExpensesViewModel.Manager;
			else if(listView == MonthlyIncomesLV)
				return ViewModel.MonthlyIncomesViewModel.Manager;
			else
			{
				if(returnNull)
					return null;

				throw new Exception(string.Format("The ListView '{0}' does not have any SummaryEngineBase pair.", listView.Name));
			}
		}

		private void SwitchMainTab()
		{
			var tabSummaryNumber = (TabSummaryNumber)MainTabControl.SelectedIndex;
			switch(tabSummaryNumber)
			{
				case TabSummaryNumber.DailyExpenses:
					if(!ViewModel.DailyExpensesViewModel.Manager.IsReady)
						ViewModel.DailyExpensesViewModel.Manager.LoadData();
					ApplySortDescriptions(DailyExpensesLV, ViewModel.DailyExpensesViewModel.Manager);
					break;
				case TabSummaryNumber.MonthlyExpenses:
					if(!ViewModel.MonthlyExpensesViewModel.Manager.IsReady)
						ViewModel.MonthlyExpensesViewModel.Manager.LoadData();
					ApplySortDescriptions(MonthlyExpensesLV, ViewModel.MonthlyExpensesViewModel.Manager);
					break;
				case TabSummaryNumber.MonthlyIncomes:
					if(!ViewModel.MonthlyIncomesViewModel.Manager.IsReady)
						ViewModel.MonthlyIncomesViewModel.Manager.LoadData();
					ApplySortDescriptions(MonthlyIncomesLV, ViewModel.MonthlyIncomesViewModel.Manager);
					break;
				case TabSummaryNumber.Statistics:
					break;
				default:
					throw new NotImplementedException("Tab: " + tabSummaryNumber);
			}
		}

		private void ApplySortDescriptions(ListView listView, SummaryEngineBase summaryEngineBase)
		{
			if(summaryEngineBase.SortDescriptions == null)
				return;

			var dataView = CollectionViewSource.GetDefaultView(listView.ItemsSource);

			dataView.SortDescriptions.Clear();
			summaryEngineBase.SortDescriptions.ForEach(dataView.SortDescriptions.Add);

			dataView.Refresh();
		}

		private bool CheckIfLisviewitemGotClicked(RoutedEventArgs e)
		{
			var originalSource = e.OriginalSource as DependencyObject;
			if(originalSource != null)
				if(originalSource.FindAncestor<ListViewItem>() == null)
					return false; // DoubleClick is in ListView, but not in a ListViewItem

			return true;
		}

		// TODO ?
		private MessageBoxResult SaveAccordingToOpenedTab()
		{
			var messageBoxResult = MessageBoxResult.None;
			switch((TabSummaryNumber)MainTabControl.SelectedIndex)
			{
				case TabSummaryNumber.DailyExpenses:
					messageBoxResult = ViewModel.SaveDailyExpenses();
					break;
				case TabSummaryNumber.MonthlyExpenses:
					break;
				case TabSummaryNumber.MonthlyIncomes:
					messageBoxResult = ViewModel.SaveMonthlyIncomes();
					break;
				default:
					throw new Exception();
			}
			return messageBoxResult;
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

		private void MonthlyExpensesLV_OnKeyUp(object sender, KeyEventArgs e)
		{
			HandleClipboardKeyCombinations(e);
		}

		private void HandleClipboardKeyCombinations(KeyEventArgs e)
		{
			if(e.IsKeyCombinationPressed(Key.C, ModifierKeys.Control, onlyModifierKeys: true))
			{
				ViewModel.ClipboardManager.Copy();
			}
		}
	}
}
