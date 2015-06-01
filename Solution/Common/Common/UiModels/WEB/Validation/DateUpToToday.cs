using System;
using System.ComponentModel.DataAnnotations;

namespace UtilsLocal.Validation
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
	public sealed class DateUpToToday : ValidationAttribute
	{
		public override bool IsValid(object value)
		{
			if(!(value is DateTime))
			{
				ErrorMessage = "{0}: Nem érvényes dátum. ";
				return false;
			}

			var dateTime = (DateTime) value;
			if (dateTime.Date > DateTime.Today)
			{
				ErrorMessage = "{0}: A dátum nem lehet jövőbeni. ";
				return false;
			}

			return true;
		}
	}
}
