using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Common;
using Common.Db.Entities;
using Exin.Common.Logging;
using Exin.Common.Logging.Core;
using Common.UiModels.WPF;
using Common.Utils.Helpers;
using Localization;
using WPF.ViewModels;
using WPF.ViewModels.SummaryModels;

namespace WPF.Controls
{
	public partial class NewTransactionForm : UserControl
	{
		#region MainWindowViewModel

		public static readonly DependencyProperty MainWindowViewModelProperty = DependencyProperty.Register(
			"MainWindowViewModel", typeof(MainWindowViewModel), typeof(NewTransactionForm), new PropertyMetadata(default(MainWindowViewModel)));

		public MainWindowViewModel MainWindowViewModel
		{
			get
			{
				var viewModel = (MainWindowViewModel)GetValue(MainWindowViewModelProperty);
				if(viewModel == null)
				{
					var msg = "MainWindowViewModel should be set.";
					throw Log.Fatal(this, m => m(msg), LogTarget.All, new InvalidOperationException(msg));
				}
				return viewModel;
			}
			set { SetValue(MainWindowViewModelProperty, value); }
		}

		#endregion

		#region ViewModel

		public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
			"ViewModel", typeof(MutableSummaryViewModelBase), typeof(NewTransactionForm), new PropertyMetadata(null));

		public MutableSummaryViewModelBase ViewModel
		{
			get
			{
				var viewModel = (MutableSummaryViewModelBase)GetValue(ViewModelProperty);
				if(viewModel == null)
				{
					var msg = "ViewModel should be set.";
					throw Log.Fatal(this, m => m(msg), LogTarget.All, new InvalidOperationException(msg));
				}
				return viewModel;
			}
			set { SetValue(ViewModelProperty, value); }
		}

		#endregion

		#region Date

		public static readonly DependencyProperty DateProperty = DependencyProperty.Register(
			"Date", typeof(DateTime), typeof(NewTransactionForm), new PropertyMetadata(default(DateTime)));

		public DateTime Date
		{
			get
			{
				var date = (DateTime)GetValue(DateProperty);
				if(date == default(DateTime))
				{
					var msg = "Date should be set.";
					throw Log.Fatal(this, m => m(msg), LogTarget.All, new InvalidOperationException(msg));
				}
				return date;
			}
			set { SetValue(DateProperty, value); }
		}

		#endregion

		#region Type

		public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(
			"Type", typeof(TransactionItemType), typeof(NewTransactionForm), new PropertyMetadata(default(TransactionItemType), TypeSet));

		public TransactionItemType Type
		{
			get
			{
				var type = (TransactionItemType)GetValue(TypeProperty);
				if(type == 0)
				{
					var msg = "Type should be set.";
					throw Log.Fatal(this, m => m(msg), LogTarget.All, new InvalidOperationException(msg));
				}
				return type;
			}
			set { SetValue(TypeProperty, value); }
		}

		#endregion

		public NewTransactionForm()
		{
			InitializeComponent();
			//LayoutRoot.DataContext = this;
		}
		private static void TypeSet(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
		{
			// -- Header
			var header = "";
			var self = (NewTransactionForm)dependencyObject;
			switch(self.Type)
			{
				case TransactionItemType.Expense:
					header = Localized.New_daily_expense;
					break;
				case TransactionItemType.Income:
					header = Localized.New_monthly_income;
					break;
				case TransactionItemType.Both:
				default:
					throw new ArgumentOutOfRangeException();
			}
			self.GroupBox.Header = header;

			// -- IncomeItem init
			if(self.Type == TransactionItemType.Income)
			{
				// Removing ExpenseItem only form elements
				// This ruins the layout, the last row will slide under the 2.
				//self.Form.RowDefinitions.RemoveRange(2, 4);

				// Removing ExpenseItem only bindings
				BindingOperations.ClearBinding(self.CategoryCB, ComboBox.SelectedItemProperty);
				BindingOperations.ClearBinding(self.UnitCB, ComboBox.SelectedItemProperty);
				BindingOperations.ClearBinding(self.QuantityTB, TextBox.TextProperty);
			}
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

		private void New_OnClick(object sender, RoutedEventArgs e)
		{
			ViewModel.New();
		}

		private void Save_OnClick(object sender, RoutedEventArgs e)
		{
			switch(Type)
			{
				case TransactionItemType.Expense:
					MainWindowViewModel.SaveDailyExpenses();
					break;
				case TransactionItemType.Income:
					MainWindowViewModel.SaveMonthlyIncomes();
					break;
				case TransactionItemType.Both:
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void Add_OnClick(object sender, RoutedEventArgs e)
		{
			ViewModel.Add();
		}

		private void Remove_OnClick(object sender, RoutedEventArgs e)
		{
			ViewModel.Remove();
		}
	}
}
