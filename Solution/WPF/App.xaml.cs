using System;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Windows;
using System.Windows.Navigation;
using Common.Configuration;
using Common.Log;
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

			// The controls will init with hungarian CultureInfo
			//Cultures.SetDefaultCultureToHungarian();
			Cultures.SetToEnglish();
		}
		
		protected override void OnLoadCompleted(NavigationEventArgs e)
		{
			base.OnLoadCompleted(e);

			// The exceptions' message will be english 
			Cultures.SetToEnglish();
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
				ExinLog.ger.LogError("Probably infinite loop detected in UnhadledExceptionHandler. Exiting now. ");
				return;
			}

			var errMsg = "Unhandled Exception occured{0}. ".Formatted(e.IsTerminating ? " (terminating)" : " (NOT terminating)");
			var promptMsg = Localized.An_unexpected_error_occured__The_app_will_stop_now__;

			var exception = e.ExceptionObject as Exception;
			if (exception == null)
			{
				ExinLog.ger.LogError(errMsg, e.ExceptionObject);
				if (e.ExceptionObject != null)
					promptMsg += e.ExceptionObject.SerializeToLog();
			}
			else
			{
				ExinLog.ger.LogException(errMsg, exception);
				promptMsg += exception.Message;
			}

			if(e.IsTerminating)
				Util.PromptErrorWindow(promptMsg);
		}

		#endregion

	}
}
