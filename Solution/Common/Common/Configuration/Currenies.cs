using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Exin.Common.Logging;

namespace Common.Configuration
{
	public static class Currenies
	{
		public const string HUF = "HUF";
		public const string USD = "USD";

		public static bool IsValid(string currencyText)
		{
			switch(currencyText)
			{
				case HUF:
				case USD:
					return true;
				default:
					return false;
			}
		}

		public static IEnumerable<string> Available
		{
			get
			{
				yield return HUF;
				yield return USD;
			}
		}
	}
}
