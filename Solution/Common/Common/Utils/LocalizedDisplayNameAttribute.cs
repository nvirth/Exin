using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using Exin.Common.Logging;
using Exin.Common.Logging.Core;
using Common.Utils.Helpers;
using Localization;

namespace Common.Utils
{
	/// System.ComponentModel.DisplayNameAttribute: Not localized <para />
	/// System.ComponentModel.DataAnnotations.DisplayAttribute: Localized, but it <para />
	///   caches the values fetched from the ResourceManager. As long as runtime <para />
	///   language changing is not possible in the app, it should no cause problems; but <para />
	///   it seems it does. From a while, it only shows the English values.<para />
	/// <para />
	///   First thought was that it probably it prepares <para />
	///   it's caches before the app's language settings are loaded from config. <para />
	///   But English strings will be shown also when the OS language is Hunagarian <para />
	/// <para />
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
	public class LocalizedDisplayNameAttribute : DisplayNameAttribute
	{
		/// <param name="resourceType">Default value is typeof(Localized)</param>
		public LocalizedDisplayNameAttribute(string resourceName, Type resourceType = null)
		{
			_resourceType = resourceType ?? typeof(Localized);
			_resourceName = resourceName;
		}

		public static readonly ResourceManager DefaultResourceManager = Localized.ResourceManager;

		private readonly string _resourceName;
		private readonly Type _resourceType;

		private static Dictionary<Type, ResourceManager> _resourceManagerCache = new Dictionary<Type, ResourceManager>(1);
		private ResourceManager GetRresourceManager(Type type)
		{
			ResourceManager resourceManager;
			try
			{
				resourceManager = _resourceType
					.GetProperties(BindingFlags.Public | BindingFlags.Static)
					.Where(pi => pi.PropertyType == typeof(ResourceManager))
					.Select(pi => (ResourceManager)pi.GetValue(null))
					.First();
			}
			catch(Exception e)
			{
				// TODO localization
				var msg = "Could not find the ResourceManager (for type: {0}). Using the default one: {1}".Formatted(_resourceType, DefaultResourceManager);
				ExinLog.ger.LogException(msg, e);

				resourceManager = DefaultResourceManager;
			}
			return resourceManager;
		}

		private ResourceManager _resourceManager;
		public ResourceManager ResourceManager
		{
			get
			{
				if(_resourceManager == null)
				{
					if(!_resourceManagerCache.ContainsKey(_resourceType))
						_resourceManagerCache[_resourceType] = GetRresourceManager(_resourceType);

					_resourceManager = _resourceManagerCache[_resourceType];
				}
				return _resourceManager;
				
			}
		}

		public override string DisplayName => ResourceManager.GetString(_resourceName);
	}
}
