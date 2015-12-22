using System;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Navigation;
using Common.Configuration;
using Exin.Common.Logging;
using Exin.Common.Logging.Core;
using Common.Utils.Helpers;
using Localization;
using WPF.Utils;

namespace WPF
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public App()
		{
			// Global handler for uncaught exceptions.
			AppDomain.CurrentDomain.UnhandledException += UnhadledExceptionHandler;
			AppDomain.CurrentDomain.FirstChanceException += FirstChanceExceptionHandler;

			// Setting the language
			Cultures.ApplyUserSettings();

			// Initializing the log system
			LogInit.InitWpfAppLogLoggers();
		}
		
		protected override void OnStartup(StartupEventArgs e)
		{
			// Do not run multiple instances from the app
			ApplicationRunningHelper.SwitchToRunningInstanceIfExists();
		}

		#region ..ExceptionHandlers

		private static void FirstChanceExceptionHandler(object sender, FirstChanceExceptionEventArgs e)
		{
			//if (Debugger.IsAttached)
			//Debugger.Break(); // Stop here while debugging
		}

		private static int _unhadledExceptionCounter = 0;
		private const int UnhadledExceptionCounterMax = 10;

		private static void UnhadledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
		{
			//if (Debugger.IsAttached)
			//Debugger.Break(); // Stop here while debugging

			_unhadledExceptionCounter++;
			if (_unhadledExceptionCounter >= UnhadledExceptionCounterMax)
			{
				Log.Error(typeof(App), m => m(Localized.ResourceManager, LocalizedKeys.Probably_infinite_loop_detected_in_UnhadledExceptionHandler__Exiting_now__));
				return;
			}

			var r = Localized.ResourceManager;
			var c = Cultures.LogCulture;
			var errMsg = r
				.GetString(LocalizedKeys.Unhandled_Exception_occured__0___, c)
				.Formatted(e.IsTerminating ? r.GetString(LocalizedKeys._terminating_, c) : r.GetString(LocalizedKeys._NOT_terminating_, c));


			var promptMsg = Localized.An_unexpected_error_occured__The_app_will_stop_now__;

			var exception = e.ExceptionObject as Exception;
			if (exception == null)
			{
				Log.Fatal(typeof(App), m => m(errMsg), LogTarget.Log, new ForDataOnlyException(e.ExceptionObject));
				if (e.ExceptionObject != null)
					promptMsg += e.ExceptionObject.SerializeToLog();
			}
			else
			{
				Log.Fatal(typeof(App), m => m(errMsg), LogTarget.Log, exception);

				var plusMessage = exception.Message;

				if(exception.InnerException != null)
				{
					// Here we can gather those unhandled exception types, which wraps the information
					if(exception is XamlParseException)
						plusMessage = exception.InnerException.Message;
				}

				promptMsg += plusMessage;
			}

			if(e.IsTerminating)
				Util.PromptErrorWindow(promptMsg);
		}

		#endregion

	}
}
