using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Common.DbEntities;
using Common.Log;
using Common.Utils.Helpers;
using Localization;

namespace Common.UiModels.WPF.DefaultValues
{
	/// <summary>
	/// This class uses DependecyInjection to avoid circular dependecies between 
	/// Exin.Common and Exin.DAL projects. <para />
	/// NOTE: Initialization needed from DAL.
	/// </summary>
	public class DefaultValueProvider
	{
		#region Singleton

		public static DefaultValueProvider Instance;

		protected DefaultValueProvider()
		{
		}

		static DefaultValueProvider()
		{
			Instance = new DefaultValueProvider();
		}

		#endregion
		#region DefaultUnit

		private Func<Unit> _getDefaultUnit;
		public Unit DefaultUnit
		{
			get
			{
				var res = _getDefaultUnit?.Invoke();
				if(res != null)
					return res;
				else
				{
					var msg = "{0}.{1}: Called before initialized. ".Formatted(this.GetType().Name, this.Property(_this => _this.DefaultUnit));
					throw ExinLog.ger.LogException(msg, new Exception(msg));
				}
			}
		}

		public void InitDefaultUnit(Func<Unit> getterFunc)
		{
			_getDefaultUnit = getterFunc;
		}

		#endregion
		#region DefaultCategory

		private Func<Category> _getDefaultCategory;
		public Category DefaultCategory
		{
			get
			{
				var res = _getDefaultCategory?.Invoke();
				if(res != null)
					return res;
				else
				{
					var msg = "{0}.{1}: Called before initialized. ".Formatted(this.GetType().Name, this.Property(_this => _this.DefaultCategory));
					throw ExinLog.ger.LogException(msg, new Exception(msg));
				}
			}
		}

		public void InitDefaultCategory(Func<Category> getterFunc)
		{
			_getDefaultCategory = getterFunc;
		}
		#endregion
	}
}
