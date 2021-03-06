﻿using System;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BLL.WpfManagers;
using Exin.Common.Logging;
using Exin.Common.Logging.Core;
using Common.UiModels.WPF;
using Common.UiModels.WPF.Base;
using Common.Utils.Helpers;
using Localization;
using WPF.Utils;

namespace WPF.ViewModels.SummaryModels
{
	public abstract class MutableSummaryViewModelBase : SummaryViewModelBase
	{
		#region TitleTextBox

		private TextBox _titleTextBox;
		public TextBox TitleTextBox
		{
			get
			{
				if(_titleTextBox == null)
				{
					const string msg = "TitleTextBox should be initialized. ";
					throw Log.Fatal(this, m => m(msg), LogTarget.All, new InvalidOperationException(msg));
				}
				return _titleTextBox;
			}
			set
			{
				_titleTextBox = value;
				OnPropertyChanged();
			}
		}

		#endregion

		protected abstract TransactionItemBase NewTransactionItem();

		public void New()
		{
			ActualItem = NewTransactionItem();
			ListView.SelectedIndex = -1;
			TitleTextBox.Focus();
		}

		public void Remove()
		{
			foreach(var selectedItem in ListView.SelectedItems.Cast<TransactionItemBase>().ToList()) // There must be a .ToList call, because the source is synchronised immediately
				ManagerBase.Remove(selectedItem);

			//NewExpenseButtonClick();
			//NewExpenseTitleTB.Focus();
		}

		public void Add()
		{
			var errorMessage = ActualItem.DoValidation();
			if(!string.IsNullOrWhiteSpace(errorMessage))
				return;

			var item = ActualItem.DeepClone();
			item.Date = ManagerBase.Date;
			ManagerBase.Add(item);
			TitleTextBox.Focus();
		}
	}
}
