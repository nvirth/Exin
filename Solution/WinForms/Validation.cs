using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;
using Common.Utils;

namespace WinForms
{
	public static class Validation
	{
		#region Validation1

		public static bool HasValidationErrors(Control.ControlCollection controls)
		{
			bool hasError = false;

			// Now we need to loop through the controls and deterime if any of them have errors
			foreach (Control control in controls)
			{
				// check the control and see what it returns
				bool validControl = IsValid(control);
				// If it's not valid then set the flag and keep going.  We want to get through all
				// the validators so they will display on the screen if errorProviders were used.
				if (!validControl)
					hasError = true;

				// If its a container control then it may have children that need to be checked
				if (control.HasChildren)
				{
					if (HasValidationErrors(control.Controls))
						hasError = true;
				}
			}
			return hasError;
		}

		// Here, let's determine if the control has a validating method attached to it
		// and if it does, let's execute it and return the result
		private static bool IsValid(object eventSource)
		{
			string name = "EventValidating";

			Type targetType = eventSource.GetType();

			do
			{
				FieldInfo[] fields = targetType.GetFields(
					BindingFlags.Static |
					BindingFlags.Instance |
					BindingFlags.NonPublic);

				foreach (FieldInfo field in fields)
				{
					if (field.Name == name)
					{
						EventHandlerList eventHandlers = ((EventHandlerList) (eventSource.GetType().GetProperty("Events",
							(BindingFlags.FlattenHierarchy |
							 (BindingFlags.NonPublic | BindingFlags.Instance))).GetValue(eventSource, null)));

						Delegate d = eventHandlers[field.GetValue(eventSource)];

						if ((d != null))
						{
							Delegate[] subscribers = d.GetInvocationList();

							// ok we found the validation event,  let's get the event method and call it
							foreach (Delegate d1 in subscribers)
							{
								// create the parameters
								object sender = eventSource;
								CancelEventArgs eventArgs = new CancelEventArgs();
								eventArgs.Cancel = false;
								object[] parameters = new object[2];
								parameters[0] = sender;
								parameters[1] = eventArgs;
								// call the method
								d1.DynamicInvoke(parameters);
								// if the validation failed we need to return that failure
								if (eventArgs.Cancel)
									return false;
								else
									return true;
							}
						}
					}
				}

				targetType = targetType.BaseType;

			} while (targetType != null);

			return true;
		}

		#endregion

		#region Validation2

		private delegate void ValidationFunction(ref string error, CancelEventArgs e);

		#region ErrorProvider

		private static ErrorProvider _errorProvider;

		public static ErrorProvider ErrorProvider
		{
			get
			{
				if (_errorProvider == null)
					throw new Exception("The Validation class's ErrorProvider property must be initialized before using.");

				return _errorProvider;
			}

			set { _errorProvider = value; }
		}

		#endregion

		public static void ValidateRequiredPositiveNumber(object sender, CancelEventArgs e, string fieldName, string textToValidate)
		{
			ValidationFunction f =
				(ref string error, CancelEventArgs args) =>
				{
					int intValue;

					if(string.IsNullOrEmpty(textToValidate))
					{
						error += string.Format("A(z) {0} nem hagyható üresen!", fieldName);
						e.Cancel = true;
					}
					else if(!int.TryParse(textToValidate, out intValue))
					{
						error += string.Format("A(z) {0} csak szám lehet!", fieldName);
						e.Cancel = true;
					}
					else if(intValue < 0)
					{
						error += string.Format("A(z) {0} nem lehet negatív!", fieldName);
						e.Cancel = true;
					}
				};

			ValidationCore((Control)sender, e, f);
		}

		public static void ValidateRequired(object sender, CancelEventArgs e, string fieldName, string textToValidate)
		{
			ValidationFunction f =
				(ref string error, CancelEventArgs args) =>
				{
					if(string.IsNullOrEmpty(textToValidate))
					{
						error += string.Format("A(z) {0} nem hagyható üresen!", fieldName);
						e.Cancel = true;
					}
				};

			ValidationCore((Control)sender, e, f);
		}

		private static void ValidationCore(Control control, CancelEventArgs e, ValidationFunction validateFunction)
		{
			string error = null;

			validateFunction(ref error, e);

			ErrorProvider.SetError(control, error);

			if(!string.IsNullOrWhiteSpace(error))
				MessagePresenter.WriteError(error);
		}

		#endregion
	}
}
