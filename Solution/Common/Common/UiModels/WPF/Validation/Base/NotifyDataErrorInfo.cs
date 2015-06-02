using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Common.UiModels.WPF.Base;
using Common.Utils.Helpers;

namespace Common.UiModels.WPF.Validation.Base
{
	[Serializable]
	public class NotifyDataErrorInfo : NotifyPropertyChanged, INotifyDataErrorInfo
	{
		private readonly Dictionary<string, ICollection<string>>
			_validationErrors = new Dictionary<string, ICollection<string>>();

		/// <summary>
		/// INotifyDataErrorInfo member, called by the framework. Use the type safe alternatives instead.
		/// DO NOT CALL THIS! Here we mock the framework, if it demands the entity level errors, we give
		/// it null!
		/// </summary>
		public IEnumerable GetErrors(string propertyName)
		{
			if(string.IsNullOrEmpty(propertyName) || !_validationErrors.ContainsKey(propertyName))
				return null;

			return GetPropertyErrors(propertyName);
		}

		public Dictionary<string, ICollection<string>> GetAllErrors()
		{
			return _validationErrors;
		}

		public ICollection<string> GetPropertyErrors(string propertyName)
		{
			if(!_validationErrors.ContainsKey(propertyName))
				return new[] { "" };

			return _validationErrors[propertyName];
		}

		public bool HasErrors => _validationErrors.Count > 0;

	    [field: NonSerialized]
		public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
		private void RaiseErrorsChanged(string propertyName)
		{
			var handler = ErrorsChanged;
			if(handler != null)
				handler(this, new DataErrorsChangedEventArgs(propertyName));
		}

		#region Validation

		private bool _isValidationOn = true;
		public bool IsValidationOn
		{
			get { return _isValidationOn; }
			set { _isValidationOn = value; }
		}

		protected void ValidateEntity()
		{
			if(!_isValidationOn)
				return;

			this.GetType().GetProperties()
				.Where(pi => MetadataTypeAttribute.MetadataClassType.GetProperty(pi.Name) != null)
				.ForEach(propertyInfo => ValidateProperty(propertyInfo.Name));

			OnPropertyChanged(this.Property(n => n.HasErrors));
		}

		/// <param name="valueBeforeSetting">
		///	This should be set in ValidationStep RawProposed. If set, this value will be used as the 
		/// property's value; if not set, then the property's real (already set) value.
		/// </param>
		protected void ValidateProperty(string propertyName, object valueBeforeSetting = null)
		{
			if(!_isValidationOn)
				return;

			ValidateModelProperty_Reflection(propertyName, valueBeforeSetting);
			OnPropertyChanged(this.Property(n => n.HasErrors));
		}

		#region MetadataTypeAttribute

		[field: NonSerialized]
		private bool _is_metadataTypeAttribute_set = false;

		[field: NonSerialized]
		private MetadataTypeAttribute _metadataTypeAttribute;
		public MetadataTypeAttribute MetadataTypeAttribute
		{
			get
			{
				if(!_is_metadataTypeAttribute_set)
				{
					_metadataTypeAttribute = this.GetType()
						.GetCustomAttributes(typeof(MetadataTypeAttribute), true)
						.OfType<MetadataTypeAttribute>()
						.FirstOrDefault();

					_is_metadataTypeAttribute_set = true;
				}

				return _metadataTypeAttribute;
			}
		}

		#endregion

		private void ValidateModelProperty_Reflection(string propertyName, object valueBeforeSetting = null)
		{
			if(_validationErrors.ContainsKey(propertyName))
				_validationErrors.Remove(propertyName);

			var instancePropertyInfo = this.GetType().GetProperty(propertyName);

			var metaPropertyInfo = MetadataTypeAttribute == null
				? instancePropertyInfo
				: MetadataTypeAttribute.MetadataClassType.GetProperty(propertyName)
					?? instancePropertyInfo;

			var value = valueBeforeSetting ?? instancePropertyInfo.GetValue(this);

			var validationErrors = metaPropertyInfo.GetCustomAttributes(true)
				.OfType<ValidationAttribute>()
				.Where(validationAttribute => !validationAttribute.IsValid(value))
				.Select(
					validationAttribute =>
					{
						var displayName = propertyName;
						var displayAttribute = metaPropertyInfo.GetCustomAttribute(typeof(DisplayAttribute)) as DisplayAttribute;
						if(displayAttribute != null)
							displayName = displayAttribute.Name;

						return validationAttribute.FormatErrorMessage(displayName);
					})
				.ToList();

			if(validationErrors.Count != 0)
				_validationErrors.Add(propertyName, validationErrors);

			RaiseErrorsChanged(propertyName);
		}

		#endregion
	}
}
