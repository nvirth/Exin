using System;
using System.Globalization;
using System.Windows.Controls;

namespace Common.UiModels.WPF.Validation
{
	public class ObjectSaverValidationRule : ValidationRule
	{
		public ObjectSaverValidationRule()
			: base(ValidationStep.RawProposedValue, /*validatesOnTargetUpdated*/ false)
		{
		}

		public Object Data { get; set; }

		public override ValidationResult Validate(object value, CultureInfo cultureInfo)
		{
			Data = value;
			return new ValidationResult(true, null);
		}
	}
}
