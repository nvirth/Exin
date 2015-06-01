using AutoMapper;
using Common.DbEntities;
using Common.UiModels.WPF;
using Common.Utils;
using Common.Utils.Helpers;

namespace Common.UiModels.WinForms
{
	public class ExpenseItemDgvViewModel : ExpenseItem
	{

		#region Overrided properties

		public override Category Category
		{
			get { return base.Category; }
			set
			{
				base.Category = value;

				OnPropertyChanged(this.Property(t => t.CategoryView));
			}
		}

		public override int Amount
		{
			get { return base.Amount; }
			set
			{
				base.Amount = value;

				OnPropertyChanged(this.Property(t => t.AmountView));
			}
		}

		public override int Quantity
		{
			get { return base.Quantity; }
			set
			{
				base.Quantity = value;
				
				OnPropertyChanged(this.Property(t => t.QuantityView));
			}
		}

		public override Unit Unit
		{
			get { return base.Unit; }
			set
			{
				base.Unit = value;
				
				OnPropertyChanged(this.Property(t => t.QuantityView));
			}
		}

		#endregion

		#region New (ViewModel) Properties

		public string AmountView
		{
			get { return Amount.ToString("c0", Cultures.hu_HU); }
			//set { Amount = int.Parse(Regex.Replace(value, @"\D", "")); }
		}

		public string QuantityView
		{
			get { return string.Format("{0} {1}", Quantity, Unit.DisplayName); }
			//set
			//{
			//	var split = value.Split(new[] { ' ' }, 2);
			//	Quantity = int.Parse(split[0]);
			//	Unit = UnitManager.GetByDisplayName(split[1]);
			//}
		}

		public string CategoryView
		{
			get { return Category.DisplayName; }
			//set { Category = CategoryManager.GetByDisplayName(value); }
		}

		#endregion


		public static ExpenseItemDgvViewModel CopyCtor(TransactionItemBase transactionItemBase)
		{
			AutoMapperInitializer<IncomeItem, ExpenseItemDgvViewModel>.InitializeIfNeeded();
			AutoMapperInitializer<ExpenseItem, ExpenseItemDgvViewModel>.InitializeIfNeeded();
			
			var vm = Mapper.Map<ExpenseItemDgvViewModel>(transactionItemBase);
			
			// Copy the event handler subscriptions as well
			transactionItemBase.CopyAmountChangedHandlerTo(vm);		// this is important!
			transactionItemBase.CopyPropertyChangedHandlerTo(vm);	// this is not (the framework did not attach any handlers yet)

			return vm;
		}

		public static IncomeItem ToIncomeItem(ExpenseItemDgvViewModel viewModel)
		{
			AutoMapperInitializer<ExpenseItemDgvViewModel, IncomeItem>.InitializeIfNeeded();

			var vm = Mapper.Map<IncomeItem>(viewModel);
			
			// Copy the event handler subscriptions as well
			viewModel.CopyAmountChangedHandlerTo(vm);		// this is important!
			viewModel.CopyPropertyChangedHandlerTo(vm);	// this is not (the framework did not attach any handlers yet)

			return vm;
		}

	}
}