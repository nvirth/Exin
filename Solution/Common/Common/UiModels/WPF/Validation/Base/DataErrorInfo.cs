using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Common.Utils;

namespace Common.UiModels.WPF.Validation.Base
{
	[Serializable]
	public class DataErrorInfo : NotifyDataErrorInfo, IDataErrorInfo
	{
		#region IDataErrorInfo members

		public string this[string propertyName] => DoValidation(propertyName);

	    public string Error => null;

	    #endregion

		#region DoValidation

		public string DoValidation()
		{
			base.ValidateEntity();

			var error = GetAllErrorMessages();
			if (!string.IsNullOrWhiteSpace(error))
				MessagePresenter.WriteError(error);

			return error;
		}

		public string DoValidation(string propertyName, object valueBeforeSetting = null)
		{
			base.ValidateProperty(propertyName, valueBeforeSetting);

			var error = GetFirstErrorMessage(propertyName);
			if (!string.IsNullOrWhiteSpace(error))
				MessagePresenter.WriteError(error);

			return error;
		}

		private string GetFirstErrorMessage(string propertyName)
		{
			var error = base.GetPropertyErrors(propertyName).FirstOrDefault();
			return error;
		}

		private string GetAllErrorMessages(string propertyName = null)
		{
			var errors = string.IsNullOrWhiteSpace(propertyName)
				? base.GetAllErrors().Values.SelectMany(x => x)
				: base.GetPropertyErrors(propertyName);

			var errorStr = errors
				.Where(error => !string.IsNullOrWhiteSpace(error))
				.Aggregate(new StringBuilder(), (acc, error) => acc.Append(error).Append(Environment.NewLine));

			// Remove last Environment.NewLine
			if (errorStr.Length > 2)
				errorStr.Remove(errorStr.Length - 2, 2);

			return errorStr.ToString();
		}

		#endregion

		#region HandleRaw

		public UpdateSourceExceptionFilterCallback HandleRaw => HandleRawProposedValue;

	    private object HandleRawProposedValue(object bindExpressionObj, Exception exception)
		{
			var bindingExpression = bindExpressionObj as BindingExpression;
			if (bindingExpression == null)
				return null;

			// The ExceptionValidationRule is applied when an exception is thrown 
			// in the setter of the source property. The ObjectSaverValidationRule's ValidationStep
			// is set to RawProposedValue, so in all cases, it will run before the ExceptionVR,
			// and so, it will save the raw value for us to use here (this method is called by ExceptionVR)
			var objectSaver = bindingExpression.ParentBinding.ValidationRules
				.OfType<ObjectSaverValidationRule>()
				.Single();

			var valueBeforeSetting = objectSaver.Data;
			var propertyName = bindingExpression.ResolvedSourcePropertyName;
			DoValidation(propertyName, valueBeforeSetting);

			return null;
		}

		#endregion
	}
}
