using System.Globalization;
using System.Threading;
using Common.Utils.Helpers;
using Exin.Common.Logging;
using Exin.Common.Logging.Core;
using Localization;

namespace Common.Configuration
{
	public static class Cultures
	{
		public class C
		{
			public const string EN = "EN";
			public const string HU = "HU";
			public const string en_US = "en-US";
			public const string hu_HU = "hu-HU";
		}

		public static readonly CultureInfo en_US = new CultureInfo(C.en_US);
		public static readonly CultureInfo hu_HU = new CultureInfo(C.hu_HU);

		public static CultureInfo DefaultCulture => en_US;
		public static CultureInfo LogCulture => en_US;

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

		public static void ApplyUserSettings()
		{
			CurrentCulture = Config.MainSettings.UserSettings.Language;
		}

		public static CultureInfo Parse(string text)
		{
			switch(text.ToUpperInvariant())
			{
				case C.EN:
				case C.en_US:
					return en_US;
				case C.HU:
				case C.hu_HU:
					return hu_HU;
				case null:
				default:
					throw Log.Fatal(typeof(Cultures),
						m => m(Localized.ResourceManager, LocalizedKeys.Unrecognised_culture_string___0_, text),
						LogTarget.All,
						new CultureNotFoundException(Localized.Unrecognised_culture_string___0_.Formatted(text))
					);
			}
		}

		public static string Serialize(CultureInfo cultureInfo)
		{
			if(Equals(cultureInfo, en_US))
				return C.EN;
			if(Equals(cultureInfo, hu_HU))
				return C.HU;

			var aa = Localized.Unknown_culture___0_;

			throw Log.Fatal(typeof(Cultures),
				m => m(Localized.ResourceManager, LocalizedKeys.Unrecognised_culture_string___0_, cultureInfo),
				LogTarget.All,
				new CultureNotFoundException(Localized.Unrecognised_culture_string___0_.Formatted(cultureInfo))
			);
		}
	}
}
