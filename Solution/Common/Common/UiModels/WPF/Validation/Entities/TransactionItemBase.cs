using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Common.Db.Entities;
using Common.UiModels.WPF.Validation;
using Common.Utils;
using Localization;

namespace Common.UiModels.WPF
{
	[MetadataType(typeof(TransactionItemBaseValidation))]
	public partial class TransactionItemBase
	{
		class TransactionItemBaseValidation
		{
			[LocalizedDisplayName(ValidationConstants.Amount)]
			[RegularExpression(ValidationConstants.RegEx_onlyNumbers, ErrorMessageResourceName = ValidationConstants.RegEx_onlyNumbers_errMsg, ErrorMessageResourceType = typeof(Localized))]
			[Required(ErrorMessageResourceName = ValidationConstants.RequiredErrorMessageFormat, ErrorMessageResourceType = typeof(Localized))]
			[Range(0, int.MaxValue, ErrorMessageResourceName = ValidationConstants.RangeMinErrorMessageFormat, ErrorMessageResourceType = typeof(Localized))]
			public int Amount { get; set; }

			[LocalizedDisplayName(ValidationConstants.Quantity)]
			[RegularExpression(ValidationConstants.RegEx_onlyNumbers, ErrorMessageResourceName = ValidationConstants.RegEx_onlyNumbers_errMsg, ErrorMessageResourceType = typeof(Localized))]
			[Required(ErrorMessageResourceName = ValidationConstants.RequiredErrorMessageFormat, ErrorMessageResourceType = typeof(Localized))]
			[Range(0, int.MaxValue, ErrorMessageResourceName = ValidationConstants.RangeMinErrorMessageFormat, ErrorMessageResourceType = typeof(Localized))]
			public int Quantity { get; set; }

			[LocalizedDisplayName(ValidationConstants.Unit)]
			[Required(ErrorMessageResourceName = ValidationConstants.RequiredErrorMessageFormat, ErrorMessageResourceType = typeof(Localized))]
			public Unit Unit { get; set; }

			[LocalizedDisplayName(ValidationConstants.Title)]
			[Required(ErrorMessageResourceName = ValidationConstants.RequiredErrorMessageFormat, ErrorMessageResourceType = typeof(Localized))]
			[StringLength(100, ErrorMessageResourceName = ValidationConstants.StringLengthMaxErrorMessageFormat, ErrorMessageResourceType = typeof(Localized))]
			[RegularExpression(ValidationConstants.RegEx_minOneLetter, ErrorMessageResourceName = ValidationConstants.RegEx_minOneLetter_errMsg, ErrorMessageResourceType = typeof(Localized))]
			public string Title { get; set; }

			[LocalizedDisplayName(ValidationConstants.Comment)]
			[StringLength(200, ErrorMessageResourceName = ValidationConstants.StringLengthMaxErrorMessageFormat, ErrorMessageResourceType = typeof(Localized))]
			public string Comment { get; set; }
		}
	}
}
