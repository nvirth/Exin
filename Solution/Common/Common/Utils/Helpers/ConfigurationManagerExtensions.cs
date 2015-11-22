using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Log;

namespace Common.Utils.Helpers
{
	public static class ConfigurationManagerExtensions
	{
		public static ConnectionStringSettings GetConnectionString(string connStrName)
		{
			var connStr = ConfigurationManager.ConnectionStrings[connStrName];
			if(string.IsNullOrWhiteSpace(connStr?.ConnectionString))
			{
				var msg = "Could not find this ConnectionString: {0}".Formatted(connStrName); // TODO localization?
				throw ExinLog.ger.LogException(msg, new ConfigurationErrorsException(msg));
			}
			return connStr;
		}
	}
}
