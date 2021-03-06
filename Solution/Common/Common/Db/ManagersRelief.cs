﻿using System;
using System.Runtime.CompilerServices;
using Exin.Common.Logging;
using Common.Utils.Helpers;
using Common.Db.Entities;
using Exin.Common.Logging.Core;
using Localization;

namespace Common.Db
{
	/// <summary>
	/// This class uses DependecyInjection to avoid circular dependecies between 
	/// Exin.Common and Exin.DAL projects. <para />
	/// NOTE: Initialization needed from DAL.
	/// </summary>
	public static class ManagersRelief
	{
		public static class UnitManager
		{
			private static readonly string _className = typeof(UnitManager).Name;

			#region GetDefaultUnit

			private static Func<Unit> _getDefaultUnit;

			public static Unit GetDefaultUnit
			{
				get
				{
					var res = _getDefaultUnit?.Invoke();
					if (res != null)
						return res;
					else
					{
						throw CalledBeforeInitException(_className);
					}
				}
			}

			public static void InitDefaultUnit(Func<Unit> getterFunc)
			{
				_getDefaultUnit = getterFunc;
			}

			#endregion
			#region GetByName

			private static Func<string, bool, Unit> _getByName;
			public static Unit GetByName(string name, bool nullIfNotFound = false)
			{
				if(_getByName == null)
				{
					throw CalledBeforeInitException(_className);
				}

				return _getByName(name, nullIfNotFound);
			}

			public static void InitGetByName(Func<string, bool, Unit> getByNameFunc)
			{
				_getByName = getByNameFunc;
			}

			#endregion
		}

		public static class CategoryManager
		{
			private static readonly string _className = typeof(CategoryManager).Name;

			#region GetDefaultCategory

			private static Func<Category> _getDefaultCategory;

			public static Category GetDefaultCategory
			{
				get
				{
					var res = _getDefaultCategory?.Invoke();
					if (res != null)
						return res;
					else
					{
						throw CalledBeforeInitException(_className);
					}
				}
			}

			public static void InitDefaultCategory(Func<Category> getterFunc)
			{
				_getDefaultCategory = getterFunc;
			}

			#endregion
			#region GetByName

			private static Func<string,bool,Category> _getByName;
			public static Category GetByName(string name, bool nullIfNotFound = false)
			{
				if (_getByName == null)
				{
					throw CalledBeforeInitException(_className);
				}

				return _getByName(name, nullIfNotFound);
			}

			public static void InitGetByName(Func<string, bool, Category> getByNameFunc)
			{
				_getByName = getByNameFunc;
			}

			#endregion
		}

		#region Helpers

		private static Exception CalledBeforeInitException(string className, [CallerMemberName] string memberName = null)
		{
			return Log.Fatal(className,
				m => m(Localized.ResourceManager, LocalizedKeys.Called_before_initialized__),
				LogTarget.All,
				new InvalidOperationException(Localized.Called_before_initialized__),
				memberName
			);
		}

		#endregion

	}
}
