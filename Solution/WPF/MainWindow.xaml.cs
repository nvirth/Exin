using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace WPF
{
	public partial class MainWindow : Window, INotifyPropertyChanged
	{
		#region MainWindowViewmodel

		private MainWindowViewmodel _model;
		public MainWindowViewmodel Model
		{
			get { return _model; }
			set
			{
				_model = value;
				OnPropertyChanged();
			}
		}

		#endregion

		public MainWindow()
		{
			InitializeComponent();
			MessagePresenterManager.WireToRichTextBox(LogTB, Dispatcher);
			InitStatic();
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
				async (sender, args) =>
				{
					await Init();

					// For init, we turned these off in the ctor
					MainTabControl.SelectionChanged += MainTabControl_SelectionChanged;
					this.IsEnabled = true;
					NewExpenseTitleTB.Focus();
				};
		}

		// Makes static class' initialization (more) determinable
		private static void InitStatic()
		{
			StaticInitializer.InitAllStatic();
		}

		private static void InitOptimize()
		{
			// Somehow, if we use EntityFramework (6), the first access to the Category
			// table is slow - if it's called from the ContentRendered event's handler.
			// (it's independent form the DB type, so MsSql and Sqlite also do the same)
			// And somehow, if we force the app to get the Categories from the DB before
			// entering the eventhandler, we will get them faster
			if(Config.Repo.ReadMode != ReadMode.FromDb || Config.Repo.DbAccessMode != DbAccessMode.EntityFramework)
				return;

			// In case we use SQLite and do not have the DB file yet, first we need to init that...
			// We will do it in the Init method
			if(SQLiteSpecific.IsDbOk())
			{
				var categoryNone = CategoryManager.Instance.GetCategoryNone;
			}
		}

		private async Task Init()
		{
			var startDate = DateTime.Today;

			var dailyExpenses = await Starter.Start(startDate);
			Model = new MainWindowViewmodel
			{
				DailyExpenses = dailyExpenses,
				MonthlyExpenses = new MonthlyExpenses(startDate, /*doWork*/ false),
				MonthlyIncomes = new MonthlyIncomes(startDate, /*doWork*/ false),
			};
			// These MUST NOT be in the object initializer (so these would be validated at now)
			Model.ActualExpenseItem = new ExpenseItem();
			Model.ActualIncomeItem = new IncomeItem();
			Model.Statistics.SetDateToMonthly(startDate);

			// In XAML, the event is bound; but the TextChange event fires this twice... Fix with ..DateChanger
			SummaryDate.SelectedDateChanged -= SummaryDate_SelectedDateChanged;
			SummaryDate.SelectedDate = startDate;
			Model.DateChanger = new DatePickerFromCodeDateChanger(SummaryDate, SummaryDate_SelectedDateChanged);

			SummaryDate.DisplayDateEnd = DateTime.Today;

			ItemsControlSorter.Init(this);
			MenuManager.Init(this);
		}

		#region Event handler methods

		#region Others...

		private void MainWindow_OnClosing(object sender, CancelEventArgs e)
		{
			var messageBoxResult = SaveWithPromptYesNoCancel();
			if(messageBoxResult == MessageBoxResult.Cancel)
				e.Cancel = true; // Exit revoked, so do not exit
		}

		private void MainWindow_PreviewKeyUp(object sender, KeyEventArgs e)
		{
			var onlyControl = Keyboard.Modifiers == ModifierKeys.Control;
			var onlyAlt = Keyboard.Modifiers == ModifierKeys.Alt;
			var onlyAltControl = Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Alt);

			if(onlyControl || onlyAlt || onlyAltControl)
			{
				var date = SummaryDate.SelectedDate ?? DateTime.Today;
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
				Model.DateChanger.ChangeSelectedDate(date);
				e.Handled = handled;
			}
		}

		private void SummaryDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
		{
			// -- Save

			SaveWithPromptYesNo();

			// -- Change
			var selectedDate = (e.AddedItems[0] as DateTime?) ?? DateTime.Today;
			var originalDate = (e.RemovedItems[0] as DateTime?) ?? DateTime.Today;

			var isNewMonth = selectedDate.Month != originalDate.Month || selectedDate.Year != originalDate.Year;
			if(isNewMonth)
			{
				Model.MonthlyExpenses = new MonthlyExpenses(selectedDate, /*doWork*/ false);
				Model.MonthlyIncomes = new MonthlyIncomes(selectedDate, /*doWork*/ false);
			}
			Model.DailyExpenses = new DailyExpenses(selectedDate, /*doWork*/ false); // There is anyway a new day
			Model.Statistics.SetDateToMonthly(selectedDate);

			SwitchMainTab();
		}

		private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			SwitchMainTab();
		}

		private void CategoryCB_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			Func<object, string> stringSelector = comboBoxItem => ((Category)comboBoxItem).DisplayName;
			CategoryCB.ComboBoxSearchKey(sender, e, stringSelector);
		}

		private void UnitCB_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			Func<object, string> stringSelector = comboBoxItem => ((Unit)comboBoxItem).DisplayName;
			UnitCB.ComboBoxSearchKey(sender, e, stringSelector);
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
			var propertyName = Model.ActualExpenseItem.Property(ei => ei.Quantity);
			ItemsControlSorter.SortControl(e, sortByProperty: propertyName);
			e.Handled = true;
		}

		#endregion


		#region DailyExpenses

		private void DailyExpensesLW_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			var originalSource = e.OriginalSource as DependencyObject;
			if(originalSource != null)
				if(originalSource.FindAncestor<ListViewItem>() == null)
					return; // DoubleClick is in ListView, but not in a ListViewItem

			if(DailyExpensesLW.SelectedIndex != -1)
			{
				var selectedExpenseItem = DailyExpensesLW.SelectedItem as ExpenseItem;
				Model.ActualExpenseItem = selectedExpenseItem;
			}
		}

		private void AddExpenseButton_OnClick(object sender, RoutedEventArgs e)
		{
			var expenseItem = Model.ActualExpenseItem.DeepClone();
			expenseItem.Date = SummaryDate.SelectedDate.Value;
			Model.DailyExpenses.Add(expenseItem);
			NewExpenseTitleTB.Focus();
		}

		private void RemoveExpenseButton_OnClick(object sender, RoutedEventArgs e)
		{
			RemoveExpenseButtonClick();
		}

		private void RemoveExpenseButtonClick()
		{
			foreach(var selectedItem in DailyExpensesLW.SelectedItems.Cast<ExpenseItem>().ToList()) // There must be a .ToList call, because the source is synchronised immediately
				Model.DailyExpenses.Remove(selectedItem);

			//NewExpenseButtonClick();
			//NewExpenseTitleTB.Focus();
		}

		private void DailyExpensesLW_KeyUp(object sender, KeyEventArgs e)
		{
			if(e.Key == Key.Delete)
			{
				var selectedIndex = DailyExpensesLW.SelectedIndex;
				RemoveExpenseButtonClick();

				DailyExpensesLW.SelectedIndex = selectedIndex <= (DailyExpensesLW.Items.Count - 1)
					? selectedIndex
					: (DailyExpensesLW.Items.Count - 1); // It works for empty list (-1)

				DailyExpensesLW.Focus();

				e.Handled = true;
			}
		}

		private void NewExpenseButton_OnClick(object sender, RoutedEventArgs e)
		{
			NewExpenseButtonClick();
		}

		private void NewExpenseButtonClick()
		{
			Model.ActualExpenseItem = new ExpenseItem();
			DailyExpensesLW.SelectedIndex = -1;
			NewExpenseTitleTB.Focus();
		}

		private void SaveDailyExpensesButton_OnClick(object sender, RoutedEventArgs e)
		{
			SaveDailyExpenses();
		}

		#endregion

		#region MonthlyExpenses

		private void RedoMonthlyExpensesButton_OnClick(object sender, RoutedEventArgs e)
		{
			var selectedDate = SummaryDate.SelectedDate ?? DateTime.Now;
			Model.MonthlyExpenses = new MonthlyExpenses(selectedDate, /* doWork */ true);
		}

		private void MonthlyExpensesLW_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if(MonthlyExpensesLW.SelectedIndex != -1)
			{
				var selectedExpenseItem = (ExpenseItem)MonthlyExpensesLW.SelectedItem;
				SummaryDate.SelectedDate = selectedExpenseItem.Date;
				MainTabControl.SelectedIndex = (int)TabSummaryNumber.DailyExpenses;

				var equalExpenseItem = Model.DailyExpenses.GetTheEqual(selectedExpenseItem);
				if(equalExpenseItem == null)
				{
					MessagePresenter.Instance.WriteError(Localized.The_chosen_expense_item_already_exists__);
					return;
				}

				Model.ActualExpenseItem = equalExpenseItem;
				DailyExpensesLW.SelectedItem = equalExpenseItem;

				//http://stackoverflow.com/questions/13955340/keyboard-focus-does-not-work-on-text-box-in-wpf
				//The code you posted should set the focus correctly, so something must be occurring afterwards
				//to move Keyboard Focus out of your TextBox. By setting focus at a later dispatcher priority, 
				//you'll be ensuring that setting keyboard focus to your SearchCriteriaTextBox gets done last.
				Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() => NewExpenseTitleTB.Focus()));
				//Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() => DailyExpensesLW.Focus()));
			}
		}

		#endregion

		#region MonthlyIncomes

		private void AddIncomeButton_OnClick(object sender, RoutedEventArgs e)
		{
			var incomeItem = Model.ActualIncomeItem.DeepClone();
			incomeItem.Date = SummaryDate.SelectedDate.Value;
			Model.MonthlyIncomes.Add(incomeItem);
			NewIncomeTitleTB.Focus();
		}

		private void RemoveIncomeButton_OnClick(object sender, RoutedEventArgs e)
		{
			RemoveIncomeButtonClick();
		}

		private void RemoveIncomeButtonClick()
		{
			foreach(var selectedItem in MonthlyIncomesLW.SelectedItems.Cast<IncomeItem>().ToList()) // There must be a .ToList call, because the source is synchronised immediately
				Model.MonthlyIncomes.Remove(selectedItem);

			//NewIncomeButtonClick();
			//NewIncomeTitleTB.Focus();
		}

		private void NewIncomeButton_OnClick(object sender, RoutedEventArgs e)
		{
			NewIncomeButtonClick();
		}

		private void NewIncomeButtonClick()
		{
			Model.ActualIncomeItem = new IncomeItem();
			MonthlyIncomesLW.SelectedIndex = -1;
			NewIncomeTitleTB.Focus();
		}

		private void MonthlyIncomesLW_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			var originalSource = e.OriginalSource as DependencyObject;
			if(originalSource != null)
				if(originalSource.FindAncestor<ListViewItem>() == null)
					return; // DoubleClick is in ListView, but not in a ListViewItem

			if(MonthlyIncomesLW.SelectedIndex != -1)
			{
				var selectedIncomeItem = MonthlyIncomesLW.SelectedItem as IncomeItem;
				Model.ActualIncomeItem = selectedIncomeItem;
			}
		}

		private void MonthlyIncomesLW_KeyUp(object sender, KeyEventArgs e)
		{
			if(e.Key == Key.Delete)
			{
				var selectedIndex = MonthlyIncomesLW.SelectedIndex;
				RemoveIncomeButtonClick();

				MonthlyIncomesLW.SelectedIndex = selectedIndex <= (MonthlyIncomesLW.Items.Count - 1)
					? selectedIndex
					: (MonthlyIncomesLW.Items.Count - 1); // It works for empty list (-1)

				MonthlyIncomesLW.Focus();

				e.Handled = true;
			}
		}

		private void SaveMonthlyIncomesButton_OnClick(object sender, RoutedEventArgs e)
		{
			SaveMonthlyIncomes();
		}

		#endregion

		#region Statistics

		private void RedoStatisticsButton_OnClick(object sender, RoutedEventArgs e)
		{
			Model.Statistics.Refresh();
		}

		#endregion

		#region Menu (delegating)

		private void MenuItem_Copy_OnClick(object sender, RoutedEventArgs e)
		{
			MenuManager.Copy(sender, e);
		}

		private void MenuItem_Options_OnClick(object sender, RoutedEventArgs e)
		{
			MenuManager.Options(sender, e);
		}

		private void MenuItem_Shortcuts_OnClick(object sender, RoutedEventArgs e)
		{
			MenuManager.Shortcuts(sender, e);
		}

		private void MenuItem_Search_OnClick(object sender, RoutedEventArgs e)
		{
			MenuManager.Search(sender, e);
		}

		private void MenuItem_Exit_OnClick(object sender, RoutedEventArgs e)
		{
			MenuManager.Exit(sender, e);
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

			if(listView == DailyExpensesLW)
				return Model.DailyExpenses;
			else if(listView == MonthlyExpensesLW)
				return Model.MonthlyExpenses;
			else if(listView == MonthlyIncomesLW)
				return Model.MonthlyIncomes;
			else
			{
				if(returnNull)
					return null;

				throw new Exception(string.Format("The ListView '{0}' does not have any SummaryEngineBase pair.", listView.Name));
			}
		}

		/// <summary>
		/// Note that this would not work as a Dictionary, 
		/// unless you always refresh the object references
		/// </summary>
		public ListView SummaryEngineBase2ListView(string summaryEngineBaseName, bool returnNull = false)
		{
			if(summaryEngineBaseName == null)
				throw new ArgumentNullException("summaryEngineBaseName");

			if(summaryEngineBaseName == Model.Property(vm => vm.DailyExpenses))
				return DailyExpensesLW;
			else if(summaryEngineBaseName == Model.Property(vm => vm.MonthlyExpenses))
				return MonthlyExpensesLW;
			else if(summaryEngineBaseName == Model.Property(vm => vm.MonthlyIncomes))
				return MonthlyIncomesLW;
			else
			{
				if(returnNull)
					return null;

				throw new Exception(string.Format("The SummaryEngineBase's name '{0}' does not have any ListView pair.", summaryEngineBaseName));
			}
		}

		private MessageBoxResult PromptErrorWindow(string errorMsg)
		{
			return MessageBox.Show(errorMsg, Localized.Error_, MessageBoxButton.OK, MessageBoxImage.Error);
		}

		private void SwitchMainTab()
		{
			var tabSummaryNumber = (TabSummaryNumber)MainTabControl.SelectedIndex;
			switch(tabSummaryNumber)
			{
				case TabSummaryNumber.DailyExpenses:
					if(!Model.DailyExpenses.IsReady)
						Model.DailyExpenses.LoadData();
					ApplySortDescriptions(DailyExpensesLW, Model.DailyExpenses);
					break;
				case TabSummaryNumber.MonthlyExpenses:
					if(!Model.MonthlyExpenses.IsReady)
						Model.MonthlyExpenses.LoadData();
					ApplySortDescriptions(MonthlyExpensesLW, Model.MonthlyExpenses);
					break;
				case TabSummaryNumber.MonthlyIncomes:
					if(!Model.MonthlyIncomes.IsReady)
						Model.MonthlyIncomes.LoadData();
					ApplySortDescriptions(MonthlyIncomesLW, Model.MonthlyIncomes);
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

		#region Save

		private MessageBoxResult PromptSaveWindow(MessageBoxButton buttons, bool saveDailyExpenses = true, bool saveMonthlyIncomes = true)
		{
			if(Model.DailyExpenses.IsModified || Model.MonthlyIncomes.IsModified)
			{
				var needSaveDailyExpenses = saveDailyExpenses && Model.DailyExpenses.IsModified;
				var needSaveMonthlyIncomes = saveMonthlyIncomes && Model.MonthlyIncomes.IsModified;

				var msg = Localized.Save_changes_ + " (";
				msg += needSaveDailyExpenses ? Localized.daily_expenses__LowerCase : "";
				msg += needSaveDailyExpenses && needSaveMonthlyIncomes ? ", " : "";
				msg += needSaveMonthlyIncomes ? Localized.monthly_incomes__LowerCase : "";
				msg += ')';

				return MessageBox.Show(msg, Localized.Save, buttons, MessageBoxImage.Question);
			}

			return MessageBoxResult.None;
		}

		private bool Save(bool saveDailyExpenses = true, bool saveMonthlyIncomes = true)
		{
			var errorMsg = "";

			if(saveDailyExpenses && Model.DailyExpenses.IsModified)
			{
				MessagePresenter.Instance.WriteLineSeparator();
				try
				{
					Model.DailyExpenses.SaveData();
					MessagePresenter.Instance.WriteLine(Localized.Daily_expenses_saved_successfully__);
				}
				catch(Exception ex)
				{
					var msg = Localized.Could_not_save_the_daily_expenses_ + ex.Message + "\r\n";
					MessagePresenter.Instance.WriteError(msg);
					MessagePresenter.Instance.WriteException(ex);
					errorMsg += msg;
				}
			}
			if(saveMonthlyIncomes && Model.MonthlyIncomes.IsModified)
			{
				MessagePresenter.Instance.WriteLineSeparator();
				try
				{
					Model.MonthlyIncomes.SaveData();
					MessagePresenter.Instance.WriteLine(Localized.Monthly_incomes_saved_successfully__);
				}
				catch(Exception ex)
				{
					var msg = Localized.Could_not_save_the_monthly_incomes_ + ex.Message + "\r\n";
					MessagePresenter.Instance.WriteError(msg);
					MessagePresenter.Instance.WriteException(ex);
					errorMsg += msg;
				}
			}

			if(!String.IsNullOrEmpty(errorMsg))
			{
				PromptErrorWindow(errorMsg);
				return false;
			}
			return true;
		}

		private MessageBoxResult SaveWithPromptYesNo(bool saveDailyExpenses = true, bool saveMonthlyIncomes = true)
		{
			var messageBoxResult = PromptSaveWindow(MessageBoxButton.YesNo, saveDailyExpenses, saveMonthlyIncomes);
			switch(messageBoxResult)
			{
				case MessageBoxResult.Yes:
					Save(saveDailyExpenses, saveMonthlyIncomes);
					break;

				case MessageBoxResult.No:
				case MessageBoxResult.None:
					break;
			}
			return messageBoxResult;
		}

		private MessageBoxResult SaveWithPromptOkCancel(bool saveDailyExpenses = true, bool saveMonthlyIncomes = true)
		{
			var messageBoxResult = PromptSaveWindow(MessageBoxButton.OKCancel, saveDailyExpenses, saveMonthlyIncomes);
			switch(messageBoxResult)
			{
				case MessageBoxResult.OK:
					Save(saveDailyExpenses, saveMonthlyIncomes);
					break;

				case MessageBoxResult.Cancel:
				case MessageBoxResult.None:
					break;
			}
			return messageBoxResult;
		}

		private MessageBoxResult SaveWithPromptYesNoCancel(bool saveDailyExpenses = true, bool saveMonthlyIncomes = true)
		{
			var messageBoxResult = PromptSaveWindow(MessageBoxButton.YesNoCancel, saveDailyExpenses, saveMonthlyIncomes);
			switch(messageBoxResult)
			{
				case MessageBoxResult.Yes:
					Save(saveDailyExpenses, saveMonthlyIncomes);
					break;

				case MessageBoxResult.No:
				case MessageBoxResult.None:
					break;

				case MessageBoxResult.Cancel:
					break;
			}
			return messageBoxResult;
		}

		private MessageBoxResult SaveDailyExpenses()
		{
			return SaveWithPromptOkCancel(saveDailyExpenses: true, saveMonthlyIncomes: false);
		}

		private MessageBoxResult SaveMonthlyIncomes()
		{
			return SaveWithPromptOkCancel(saveDailyExpenses: false, saveMonthlyIncomes: true);
		}

		private MessageBoxResult SaveAccordingToOpenedTab()
		{
			var messageBoxResult = MessageBoxResult.None;
			switch((TabSummaryNumber)MainTabControl.SelectedIndex)
			{
				case TabSummaryNumber.DailyExpenses:
					messageBoxResult = SaveDailyExpenses();
					break;
				case TabSummaryNumber.MonthlyExpenses:
					break;
				case TabSummaryNumber.MonthlyIncomes:
					messageBoxResult = SaveMonthlyIncomes();
					break;
				default:
					throw new Exception();
			}
			return messageBoxResult;
		}

		#endregion

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
	}
}
