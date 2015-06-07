using System.Globalization;
using System.Threading;

namespace Common.Configuration
{
	public static class Cultures
	{
		public static readonly CultureInfo en_US = new CultureInfo("en-US");
		public static readonly CultureInfo hu_HU = new CultureInfo("hu-HU");

		public static CultureInfo DefaultCulture => en_US;

		private static CultureInfo _currentCulture;
		public static CultureInfo CurrentCulture
		{
			get { return _currentCulture; }
			private set
			{
				_currentCulture = value;
                Thread.CurrentThread.CurrentCulture = value;
				Thread.CurrentThread.CurrentUICulture = value;
				CultureInfo.DefaultThreadCurrentCulture = value;
				CultureInfo.DefaultThreadCurrentUICulture = value;
			}
		}

		static Cultures()
		{
			CurrentCulture = DefaultCulture;
		}

		public static void SetToEnglish()
		{
			CurrentCulture = en_US;
		}

		public static void SetToHungarian()
		{
			CurrentCulture = hu_HU;
		}
	}
}
