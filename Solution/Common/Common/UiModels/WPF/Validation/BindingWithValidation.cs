using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using Common.UiModels.WPF.Validation.Base;
using Common.Utils;

namespace Common.UiModels.WPF.Validation
{
	/// <summary>
	/// Initializes a BindingExpresson object for validation. So we don't have to write
	/// so much boilerplate code in the xaml files :)
	/// </summary>
	public class BindingWithValidation : BindingDecoratorBase
	{
		public override object ProvideValue(IServiceProvider provider)
		{
			Mode = BindingMode.TwoWay;
			UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
			ValidatesOnDataErrors = true;
			ValidatesOnExceptions = true;
			ValidationRules.Add(new ObjectSaverValidationRule());

			SetUpdateSourceExceptionFilter(provider);

			return base.ProvideValue(provider);
		}

		private void SetUpdateSourceExceptionFilter(IServiceProvider provider)
		{
			var valueProvider = provider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
			if(valueProvider != null)
			{
				var bindingTarget = valueProvider.TargetObject as FrameworkElement;
				if (bindingTarget != null)
					// On the time of the calling of this, the DataContext is null yet
					// And we also have to stay in sync
					bindingTarget.DataContextChanged += OnDataContextChanged;
			}
		}

		private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs args)
		{
			var dataErrorInfo = args.NewValue as DataErrorInfo;
			if (dataErrorInfo != null)
				// This is a simple property, not an event -> no need to unsubscribe the old one
				binding.UpdateSourceExceptionFilter = dataErrorInfo.HandleRaw;
		}
	}

	/// <summary>
	/// It's an alias to BindingWithValidation class
	/// </summary>
	public class Binding : BindingWithValidation { }
}
