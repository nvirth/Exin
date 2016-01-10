using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Common.Utils.Helpers;
using Exin.Common.Logging.Core;
using Localization;
using Microsoft.Win32;

namespace WPF.Web
{
	enum BrowserEmulationVersion
	{
		None = 0,
		Version7 = 7000,
		Version8 = 8000,
		Version8Standards = 8888,
		Version9 = 9000,
		Version9Standards = 9999,
		Version10 = 10000,
		Version10Standards = 10001,
		Version11 = 11000,
		Version11Edge = 11001
	}

	public static class WebBrowserHelpers
	{
		private const string IE_ROOT = @"Software\Microsoft\Internet Explorer";
		private const string IE_EMULATION = IE_ROOT + @"\Main\FeatureControl\FEATURE_BROWSER_EMULATION";
		private const int MIN_IE_VERSION_NECESSARY = 9;

		public static bool JsChartsEnabled => IeMajorVersion >= MIN_IE_VERSION_NECESSARY;

		private static int _ieMajorVersion = -1;
		private static int IeMajorVersion
		{
			get
			{
				if(_ieMajorVersion == -1)
				{
					try
					{
						using(var key = Registry.LocalMachine.OpenSubKey(IE_ROOT))
						{
							var value = key.GetValue("svcVersion", null) ?? key.GetValue("Version", null);

							var versionStr = value.ToString();
							var separatorIdx = versionStr.IndexOf('.');
							if(separatorIdx != -1)
								int.TryParse(versionStr.Substring(0, separatorIdx), out _ieMajorVersion);
						}
					}
					catch(Exception e)
					{
						Log.Warn(typeof(WebBrowserHelpers), m => m("Error while reading IE Major Version from registry. "), LogTarget.Log, e);
					}

					if(_ieMajorVersion == -1)
					{
						_ieMajorVersion = -2; // Property is initialized (unseccessfully though). Use the cached value from now on.
					}
					else if(_ieMajorVersion < MIN_IE_VERSION_NECESSARY)
					{
						// TODO logging to GUI is not stable here (currently not possible, 2016.01.09)
						Log.Warn(typeof(WebBrowserHelpers), m => m(Localized.ResourceManager, LocalizedKeys.Exin_uses_Internet_Explorer_to_draw_some_charts__FORMAT__, _ieMajorVersion, MIN_IE_VERSION_NECESSARY));
					}
				}
				return _ieMajorVersion;
			}
		}

		private static BrowserEmulationVersion IeEmulatedVersion
		{
			get
			{
				if(IeMajorVersion < 0 || IeMajorVersion >= 11)
					return BrowserEmulationVersion.Version11;

				switch(IeMajorVersion)
				{
					//case 11: return BrowserEmulationVersion.Version11;
					case 10: return BrowserEmulationVersion.Version10;
					case 9: return BrowserEmulationVersion.Version9;
					case 8: return BrowserEmulationVersion.Version8;
					case 7:
					default: return BrowserEmulationVersion.Version7;
				}
			}
		}

		public static void Init()
		{
			var appName = "";
			try
			{
				// -- Calc entry exe name
				var entryAssmeblyLocation = Assembly.GetEntryAssembly().Location;
				var extension = Path.GetExtension(entryAssmeblyLocation); // Always exe, since entry point?
				var fileName = Path.GetFileNameWithoutExtension(entryAssmeblyLocation);

				if(Debugger.IsAttached) // If we currently debug the app
					fileName += ".vshost";

				appName = fileName + extension;

				// -- Write into Registry
				using(var fbeKey = Registry.CurrentUser.OpenSubKey(IE_EMULATION, writable: true))
					fbeKey.SetValue(appName, IeEmulatedVersion, RegistryValueKind.DWord);

				Log.Info(typeof(WebBrowserHelpers), 
					m => m("Registry key set up successfully for Internet Explorer emulation. "), 
					LogTarget.Log,
					new ForDataOnlyException(new { RegistryKey = IE_EMULATION, appName, IE_VERSION = IeEmulatedVersion })
				);
			}
			catch(Exception e)
			{
				Log.Error(typeof(WebBrowserHelpers),
					m => m(Localized.ResourceManager, LocalizedKeys.Could_not_setup_Internet_Explorer_emulation_version__),
					LogTarget.All,
					e.WithData(new { RegistryKey = IE_EMULATION, appName, IE_VERSION = IeEmulatedVersion })
				);
			}
		}
	}
}
