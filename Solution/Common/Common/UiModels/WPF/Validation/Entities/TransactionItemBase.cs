using System.ComponentModel.DataAnnotations;
using Common.Db.Entities;
using Common.UiModels.WPF.Validation;
using Localization;

namespace Common.UiModels.WPF
{
	[MetadataType(typeof(TransactionItemBaseValidation))]
	public partial class TransactionItemBase
	{
		class TransactionItemBaseValidation
		{
			[Display(Name = ValidationConstants.Amount, ResourceType = typeof(Localized))]
			[RegularExpression(ValidationConstants.RegEx_onlyNumbers, ErrorMessageResourceName = ValidationConstants.RegEx_onlyNumbers_errMsg, ErrorMessageResourceType = typeof(Localized))]
			[Required(ErrorMessageResourceName = ValidationConstants.RequiredErrorMessageFormat, ErrorMessageResourceType = typeof(Localized))]
			[Range(0, int.MaxValue, ErrorMessageResourceName = ValidationConstants.RangeMinErrorMessageFormat, ErrorMessageResourceType = typeof(Localized))]
			public int Amount { get; set; }

			[Display(Name = ValidationConstants.Quantity, ResourceType = typeof(Localized))]
			[RegularExpression(ValidationConstants.RegEx_onlyNumbers, ErrorMessageResourceName = ValidationConstants.RegEx_onlyNumbers_errMsg, ErrorMessageResourceType = typeof(Localized))]
			[Required(ErrorMessageResourceName = ValidationConstants.RequiredErrorMessageFormat, ErrorMessageResourceType = typeof(Localized))]
			[Range(0, int.MaxValue, ErrorMessageResourceName = ValidationConstants.RangeMinErrorMessageFormat, ErrorMessageResourceType = typeof(Localized))]
			public int Quantity { get; set; }

			[Display(Name = ValidationConstants.Unit, ResourceType = typeof(Localized))]
			[Required(ErrorMessageResourceName = ValidationConstants.RequiredErrorMessageFormat, ErrorMessageResourceType = typeof(Localized))]
			public Unit Unit { get; set; }

			[Display(Name = ValidationConstants.Title, ResourceType = typeof(Localized))]
			[Required(ErrorMessageResourceName = ValidationConstants.RequiredErrorMessageFormat, ErrorMessageResourceType = typeof(Localized))]
			[StringLength(100, ErrorMessageResourceName = ValidationConstants.StringLengthMaxErrorMessageFormat, ErrorMessageResourceType = typeof(Localized))]
			[RegularExpression(ValidationConstants.RegEx_notEmptyOrWhiteSpace, ErrorMessageResourceName = ValidationConstants.RegEx_notEmptyOrWhiteSpace_errMsg, ErrorMessageResourceType = typeof(Localized))]
			public string Title { get; set; }

			[Display(Name = ValidationConstants.Comment, ResourceType = typeof(Localized))]
			[StringLength(200, ErrorMessageResourceName = ValidationConstants.StringLengthMaxErrorMessageFormat, ErrorMessageResourceType = typeof(Localized))]
			public string Comment { get; set; }
		}
	}
}
