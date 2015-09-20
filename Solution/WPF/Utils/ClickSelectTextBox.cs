using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WPF.Utils
{
	public class ClickSelectTextBox : TextBox
	{
		#region Properties

		public static readonly DependencyProperty IsNumericProperty = DependencyProperty.Register(
			"IsNumeric", typeof (bool), typeof (ClickSelectTextBox), new PropertyMetadata(default(bool)));

		public bool IsNumeric
		{
			get { return (bool) GetValue(IsNumericProperty); }
			set { SetValue(IsNumericProperty, value); }
		}

		#endregion

		public ClickSelectTextBox()
		{
			AddHandler(PreviewMouseLeftButtonDownEvent,
			  new MouseButtonEventHandler(SelectivelyIgnoreMouseButton), true);
			AddHandler(GotKeyboardFocusEvent,
			  new RoutedEventHandler(SelectAllText), true);
			AddHandler(MouseDoubleClickEvent,
			  new RoutedEventHandler(SelectAllText), true);

			PreviewTextInput += OnPreviewTextInput;
			DataObject.AddPastingHandler(this, OnPaste);
		}

		private static void OnPaste(object sender, DataObjectPastingEventArgs e)
		{
			if(e.DataObject.GetDataPresent(DataFormats.Text))
			{
				var text = (String)e.DataObject.GetData(typeof(String));
				if(!IsValid(text))
					e.CancelCommand();
			}
			else
			{
				e.CancelCommand();
			}
		}

		private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			if(!IsNumeric)
				return;

			if(!IsValid(e.Text))
				e.Handled = true;
		}

		private static bool IsValid(string text)
		{
			var regex = new Regex(@"^[-+]?[\d]*$"); //TODO float
			var isValid = regex.IsMatch(text);
			return isValid;
		}

		private static void SelectivelyIgnoreMouseButton(object sender, MouseButtonEventArgs e)
		{
			// Find the TextBox
			DependencyObject parent = e.OriginalSource as UIElement;
			while(parent != null && !(parent is TextBox))
				parent = VisualTreeHelper.GetParent(parent);

			if(parent != null)
			{
				var textBox = (TextBox)parent;
				if(!textBox.IsKeyboardFocusWithin)
				{
					// If the text box is not yet focussed, give it the focus and
					// stop further processing of this click event.
					textBox.Focus();
					e.Handled = true;
				}
			}
		}

		private static void SelectAllText(object sender, RoutedEventArgs e)
		{
			var textBox = e.OriginalSource as TextBox;
		    textBox?.SelectAll();
		}
	}
}