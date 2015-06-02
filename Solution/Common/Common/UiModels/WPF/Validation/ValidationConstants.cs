namespace Common.UiModels.WPF.Validation
{
	/// <summary>
	/// Before localization, the messages contained fix string messages. Now, they contain resource keys. 
	/// (Like mirror keys, just to make them type safe)
	/// </summary>
	public class ValidationConstants
	{
		public const string RequiredErrorMessageFormat = "RequiredErrorMessageFormat";
		public const string StringLengthMinMaxErrorMessageFormat = "StringLengthMinMaxErrorMessageFormat";
		public const string StringLengthMaxErrorMessageFormat = "StringLengthMaxErrorMessageFormat";
		public const string RangeMinMaxErrorMessageFormat = "RangeMinMaxErrorMessageFormat";
		public const string RangeMinErrorMessageFormat = "RangeMinErrorMessageFormat";

		public const string Amount = "Amount";
		public const string Quantity = "Quantity";
		public const string Unit = "Unit";
		public const string Title = "Title";
		public const string Comment = "Comment";

		#region RegEx

		private const string HuChar = @"[a-zA-ZíéáőúűóüöÍÉÁŐÚŰÓÜÖ]";
		private const string HuCharOrSpace = @"[a-zA-ZíéáőúűóüöÍÉÁŐÚŰÓÜÖ ]";
		private const string HuCharOrSpaceOrComma = @"[a-zA-ZíéáőúűóüöÍÉÁŐÚŰÓÜÖ ,]";
		private const string HuCharOrDigit = @"([a-zA-ZíéáőúűóüöÍÉÁŐÚŰÓÜÖ]|\d)";
		private const string HuCharOrDigitOrSeparator = @"([a-zA-ZíéáőúűóüöÍÉÁŐÚŰÓÜÖ ,;.]|\d)";
		private const string EnChar = @"([a-zA-Z])";
		private const string EnCharOrSpace = @"([a-zA-Z] )";
		private const string EnCharOrDigit = @"([a-zA-Z]|\d)";

		public const string RegEx_notEmptyOrWhiteSpace = @".*[a-zA-Z]+.*";
		public const string RegEx_onlyNumbers = @"(-|\+)?[0-9]+";
		public const string RegEx_START_EnChar_EnCharOrDigit_0Ti_END = "^" + EnChar + EnCharOrDigit + "*" + "$";
		public const string RegEx_START_HuChar_HuCharOrDigitOrSeparator_0Ti_END = "^" + HuChar + HuCharOrDigitOrSeparator + "*" + "$";
		public const string RegEx_START_HuChar_HuCharOrSpace_0Ti_END = "^" + HuChar + HuCharOrSpace + "*" + "$";
		public const string RegEx_START_HuChar_HuCharOrSpaceOrComma_0Ti_END = "^" + HuChar + HuCharOrSpaceOrComma + "*" + "$";

		public const string RegEx_notEmptyOrWhiteSpace_errMsg = "RegEx_notEmptyOrWhiteSpace_errMsg";
		public const string RegEx_onlyNumbers_errMsg = "RegEx_onlyNumbers_errMsg";
		public const string RegExErrMsg_START_EnChar_EnCharOrDigit_0Ti_END = "RegExErrMsg_START_EnChar_EnCharOrDigit_0Ti_END";
		public const string RegExErrMsg_START_HuChar_HuCharOrDigitOrSeparator_0Ti_END = "RegExErrMsg_START_HuChar_HuCharOrDigitOrSeparator_0Ti_END";
		public const string RegExErrMsg_START_HuChar_HuCharOrSpace_0Ti_END = "RegExErrMsg_START_HuChar_HuCharOrSpace_0Ti_END";
		public const string RegExErrMsg_START_HuChar_HuCharOrSpaceOrComma_0Ti_END = "RegExErrMsg_START_HuChar_HuCharOrSpaceOrComma_0Ti_END";

		#endregion
	}
}
