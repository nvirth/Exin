using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exin.Common.Logging;
using Exin.Common.Logging.Core;
using Localization;

namespace Common.Utils.Helpers
{
	public static class ConfigurationManagerExtensions
	{
		public static ConnectionStringSettings GetConnectionString(string connStrName)
		{
			var connStr = ConfigurationManager.ConnectionStrings[connStrName];
			if(string.IsNullOrWhiteSpace(connStr?.ConnectionString))
			{
				throw Log.Fatal(typeof(ConfigurationManagerExtensions), 
					m => m(Localized.ResourceManager, LocalizedKeys.Could_not_find_this_ConnectionString___0_, connStrName),
					LogTarget.All,
					new ConfigurationErrorsException(Localized.Could_not_find_this_ConnectionString___0_.Formatted(connStrName))
				);
			}
			return connStr;
		}
	}
}
