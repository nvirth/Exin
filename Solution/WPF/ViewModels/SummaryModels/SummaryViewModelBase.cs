using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using BLL.WpfManagers;
using Common.Log;
using Common.UiModels.WPF;
using Common.UiModels.WPF.Base;
using Common.Utils.Helpers;
using Localization;
using WPF.Utils;
using Common.Configuration;
using Common.Configuration.Settings;

namespace WPF.ViewModels.SummaryModels
{
	public abstract class SummaryViewModelBase : ChainedCommonBase
	{
		#region SummaryEngine

		private SummaryEngineBase _managerBase;
		public virtual SummaryEngineBase ManagerBase
		{
			get { return _managerBase; }
			protected set
			{
				Util.SaveSort(_managerBase, value);
				_managerBase = value;
				OnPropertyChanged();

				SetAddButtonLabel(isAdd: true);
                _managerBase.TransactionItems.CollectionChanged += EnsurePreviousSeceltedEditedGotRemoved;
			}
		}

		#endregion

		#region ActualItem

		private TransactionItemBase _actualItem;
		public virtual TransactionItemBase ActualItem
		{
			get { return _actualItem; }
			protected set
			{
				if(_actualItem == value)
					return;

				_actualItem = value;

				SetAddButtonLabel(isAdd: !ManagerBase.TransactionItems.Contains(value));

				// New item is not valid by default; Title is empty
				_actualItem.IsValidationOn = false;
				OnPropertyChanged();
				_actualItem.IsValidationOn = true;
			}
		}

		#endregion

		#region ListView

		private ListView _listView;
		public ListView ListView
		{
			get
			{
				if(_listView == null)
				{
					var msg = "{0}ListView should be initialized".Formatted(Helpers.GetTag(this));
					throw ExinLog.ger.LogException(msg, new InvalidOperationException(msg));
				}
				return _listView;
			}
			set
			{
				_listView = value;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Selected-Edited

		#region AddButtonLabel

		public string AddButtonLabel { get; private set; }

		/// <param name="isAdd">True: Add, False: Copy</param>
		private void SetAddButtonLabel(bool isAdd)
		{
			// { return PrevSelectedEditedItem == null ? Localized.Add : Localized.Copy; }
			AddButtonLabel = isAdd ? Localized.Add : Localized.Copy;
			OnPropertyChanged(this.Property(x => x.AddButtonLabel));
		}

		#endregion

		private void EnsurePreviousSeceltedEditedGotRemoved(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch(e.Action)
			{
				case NotifyCollectionChangedAction.Remove:
				case NotifyCollectionChangedAction.Replace:
				case NotifyCollectionChangedAction.Reset:
					if(e.OldItems.Contains(ActualItem))
						SetAddButtonLabel(isAdd: true);
					break;
			}
		}

		#endregion

		public void CopySelectionToClipboard(CopyFormat? copyFormat = null)
		{
			copyFormat = copyFormat ?? Config.MainSettings.UserSettings.CopyFormat;
			var selectedTransactions = ListView.SelectedItems.Cast<TransactionItemBase>();
			string resultString = "";

			switch(copyFormat)
			{
				case CopyFormat.Xml:
					resultString = selectedTransactions
						.Select(ei => ei.ToXml().ToString())
						.Join("\r\n");
					break;
				case CopyFormat.Json:
					resultString = selectedTransactions
						.Select(ei => ei.ToXml().ToJson())
						.Join(",\r\n");
					break;
				case CopyFormat.Csv:
					ExinLog.ger.LogError("This CopyFormat is not impleneted yet: " + copyFormat);
					break;
				default:
					throw new NotImplementedException("This CopyFormat is not impleneted yet: " + copyFormat);
			}

			if(!string.IsNullOrWhiteSpace(resultString))
				Clipboard.SetText(resultString, TextDataFormat.Text);

			Config.MainSettings.UserSettings.CopyFormat = copyFormat.Value;
		}
	}
}
