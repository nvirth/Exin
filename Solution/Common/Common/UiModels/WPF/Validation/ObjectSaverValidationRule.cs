using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

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
