using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using BLL.WpfManagers;
using Common.Log;
using Common.UiModels.WPF;
using Common.UiModels.WPF.Base;
using Common.Utils.Helpers;
using Localization;
using WPF.Utils;

namespace WPF.ViewModels.SummaryModels
{
	public abstract class SummaryViewModelBase : ChainedCommonBase
	{
		#region SummaryEngine

		private SummaryEngineBase _summaryEngine;
		protected SummaryEngineBase SummaryEngine
		{
			get { return _summaryEngine; }
			set
			{
				Util.SaveSort(_summaryEngine, value);
				_summaryEngine = value;
				OnPropertyChanged();

				_summaryEngine.TransactionItems.CollectionChanged += CheckIfPreviousSeceltedEditedRemoved;
			}
		}

		#endregion

		#region ActualItem

		private TransactionItemBase _actualItem;
		protected TransactionItemBase ActualItem
		{
			get { return _actualItem; }
			set
			{
				if(_actualItem == value)
					return;

				var oldValue = _actualItem;
				_actualItem = value;

				RefreshSelectedEdited(oldValue, value);

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
			set { _listView = value; }
		}

		#endregion

		#region Selected-Edited

		public string AddButtonLabel => PrevSelectedEditedItem == null ? Localized.Add : Localized.Copy;

		#region PrevSelectedEditedItem

		/// The previous Selected-Edited item
		private TransactionItemBase _prevSelectedEditedItem;
		public TransactionItemBase PrevSelectedEditedItem
		{
			get { return _prevSelectedEditedItem; }
			set
			{
				if(_prevSelectedEditedItem == value)
					return;

				_prevSelectedEditedItem = value;
				OnPropertyChanged();
				OnPropertyChanged(this.Property(x => x.AddButtonLabel));
			}
		}

		#endregion

		#region PrevSelectedEditedOriginalFontWeight

		/// The original FontWeight of the previous Selected-Edited item
		private FontWeight _prevSelectedEditedOriginalFontWeight;
		public FontWeight PrevSelectedEditedOriginalFontWeight
		{
			get { return _prevSelectedEditedOriginalFontWeight; }
			set
			{
				if(_prevSelectedEditedOriginalFontWeight == value)
					return;

				_prevSelectedEditedOriginalFontWeight = value;
				OnPropertyChanged();
			}
		}

		#endregion

		private void RefreshSelectedEdited(TransactionItemBase prevItem, TransactionItemBase newItem)
		{
			if(ListView.SelectedItem == newItem) // Contains would be enough for us now. But this is faster :)
			{
				if(PrevSelectedEditedItem != null && PrevSelectedEditedItem != prevItem)
					ExinLog.ger.LogError("[WARN] PrevSelectedEditedItem != prevItem");

				RemovePreviousSelectedEditedHighlight();
				HighlightSelectedEdited();
			}
			else
			{
				RemovePreviousSelectedEditedHighlight();
			}
		}

		/// Adds highlight to the new selected-and-edited item in the ListView. Removes the highlight from the previous, if exists
		private void HighlightSelectedEdited()
		{
			if(PrevSelectedEditedItem != null)
				RemovePreviousSelectedEditedHighlight();

			// Currently, this is the "ActualSelectedEditedItem"; we just store here a reference for later
			PrevSelectedEditedItem = (TransactionItemBase)ListView.SelectedItem; // TODO what if multiple selection?

			var lvItem = (ListViewItem)ListView.ItemContainerGenerator.ContainerFromIndex(ListView.SelectedIndex);

			PrevSelectedEditedOriginalFontWeight = lvItem.FontWeight;
			lvItem.FontWeight = FontWeights.Bold;
		}

		private void RemovePreviousSelectedEditedHighlight()
		{
			if(PrevSelectedEditedItem == null)
				return;

			var prevLvItem = (ListViewItem)ListView.ItemContainerGenerator.ContainerFromItem(PrevSelectedEditedItem);
			if(prevLvItem != null)
				prevLvItem.FontWeight = PrevSelectedEditedOriginalFontWeight;
			else
				ExinLog.ger.LogError("[WARN] The stored reference for the previous Selected-Edited item is invalid. It will be cleared now anyway. ");

			PrevSelectedEditedItem = null;
		}

		private void CheckIfPreviousSeceltedEditedRemoved(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch(e.Action)
			{
				case NotifyCollectionChangedAction.Remove:
				case NotifyCollectionChangedAction.Replace:
				case NotifyCollectionChangedAction.Reset:
					if(e.OldItems.Contains(PrevSelectedEditedItem))
						RemovePreviousSelectedEditedHighlight();
					break;
			}
		}

		#endregion
	}
}
