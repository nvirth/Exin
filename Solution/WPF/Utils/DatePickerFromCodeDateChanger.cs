using System;
using System.Windows.Controls;
using Localization;

namespace WPF.Utils
{
	public class DatePickerFromCodeDateChanger
	{
		/// <summary>
		/// It's set to true before calling a method, which changes the DatePicker's SelectedDate property. 
		/// It's set to false on the end of the (changer) operation. 
		/// </summary>
		private bool _calledFromCode = false;

		private readonly EventHandler<SelectionChangedEventArgs> _originalEventHandler;
		private readonly DatePicker _datePicker;

		public DatePickerFromCodeDateChanger(DatePicker datePicker, EventHandler<SelectionChangedEventArgs> eventHandler)
		{
			if(datePicker == null || eventHandler == null)
				throw new Exception(Localized.The_ChangeSummaryDateFromCode_class_can_not_access_null_value_in_ctor__);

			_datePicker = datePicker;
			_originalEventHandler = eventHandler;

			_datePicker.SelectedDateChanged += ProxyEventHandler;
		}

		private void ProxyEventHandler(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
		{
			_originalEventHandler(sender, selectionChangedEventArgs);
			
			// In wpf, a DatePicker's SelectedDateChanged event fires twice, if date is changed by code. Wpf bug
			if(_calledFromCode)
				_datePicker.SelectedDateChanged -= ProxyEventHandler;
		}

		/// <summary>
		/// In wpf, a DatePicker's SelectedDateChanged event fires twice, if date is changed by code. Wpf bug. 
		/// So we have to call this method to correct the way. 
		/// Before setting the DatePicker's SelectedDate property to the new value, the _calledFromCode flag is
		/// set to true. So in the ProxyEventHandler method, after calling the _originalEventHandler method, the 
		/// proxy method detaches itself from the (DatePicker's) SelectedDateChanged event. 
		/// At last, in the end of this method, we wire the ProxyEventHandler back to the SelectedDateChanged event. 
		/// </summary>
		public void ChangeSelectedDate(DateTime date)
		{
			_calledFromCode = true;
			_datePicker.SelectedDate = date > DateTime.Today ? DateTime.Today : date;
			_calledFromCode = false;
			
			// if the event was not raised, we do not want to attach the handler multiple times
			// if the event was not raised, the handler was not detached - we do it manually here
			// if the event was raised, the handler is already detached - this case this operation
			//  will do nothing
			_datePicker.SelectedDateChanged -= ProxyEventHandler;

			// we attach the handler back
			_datePicker.SelectedDateChanged += ProxyEventHandler;
		}
	}
}