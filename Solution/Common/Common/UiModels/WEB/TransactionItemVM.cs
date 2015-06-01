using System;
using System.ComponentModel.DataAnnotations;
using Common.DbEntities;
using Common.UiModels.WPF;
using Common.UiModels.WPF.Validation;
using UtilsLocal;
using UtilsLocal.Validation;

namespace Common.UiModels.WEB
{
	[MetadataType(typeof(Validations))]
	public partial class TransactionItemVM
	{
		public TransactionItemVM()
		{
			Quantity = 1;
		}

		public int Amount { get; set; }
		public int Quantity { get; set; }
		public string Title { get; set; }
		public string Comment { get; set; }
		public DateTime Date { get; set; }

		//public Category Category { get; set; }
		public int CategoryID { get; set; }
		//public Unit Unit { get; set; }
		public int UnitID { get; set; }

		//public int ID { get; set; }

		public TransactionItemType Type { get; set; }

		private class Validations
		{
			[Display(Name = "Összeg (Ft)")]
			[Required(ErrorMessage = ValidationConstants.RequiredErrorMessageFormat)]
			[RegularExpression(ValidationConstants.RegEx_onlyNumbers, ErrorMessage = ValidationConstants.RegEx_onlyNumbers_errMsg)]
			[Range(0, int.MaxValue, ErrorMessage = ValidationConstants.RangeMinErrorMessageFormat)]
			public int Amount { get; set; }

			[Display(Name = "Mennyiség")]
			[Required(ErrorMessage = ValidationConstants.RequiredErrorMessageFormat)]
			[RegularExpression(ValidationConstants.RegEx_onlyNumbers, ErrorMessage = ValidationConstants.RegEx_onlyNumbers_errMsg)]
			[Range(1, int.MaxValue, ErrorMessage = ValidationConstants.RangeMinErrorMessageFormat)]
			public int Quantity { get; set; }

			[Display(Name = "Megnevezés")]
			[Required(ErrorMessage = ValidationConstants.RequiredErrorMessageFormat)]
			[StringLength(100, ErrorMessage = ValidationConstants.StringLengthMaxErrorMessageFormat)]
			public string Title { get; set; }

			[Display(Name = "Megjegyzés")]
			[StringLength(200, ErrorMessage = ValidationConstants.StringLengthMaxErrorMessageFormat)]
			public string Comment { get; set; }

			[Display(Name = "Nap")]
			[Required(ErrorMessage = ValidationConstants.RequiredErrorMessageFormat)]
			[DateUpToToday]
			public DateTime Date { get; set; }


			[Display(Name = "Kategória")]
			[Required(ErrorMessage = ValidationConstants.RequiredErrorMessageFormat)]
			public int CategoryID { get; set; }

			[Display(Name = "Mértékegység")]
			[Required(ErrorMessage = ValidationConstants.RequiredErrorMessageFormat)]
			public int UnitID { get; set; }
		}
	}

	public partial class TransactionItemVM : IComparable<TransactionItemVM>
	{
		public int CompareTo(TransactionItemVM other)
		{
			int result = Amount.CompareTo(other.Amount);
			return result;
		}
	}
}