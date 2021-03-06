﻿using System;
using System.Diagnostics;
using System.Linq;
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
using WPF.Web;

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

			// Initializing the log system
			LogInit.InitWpfAppLogLoggers();

			// Setting the language
			Cultures.ApplyUserSettings();

			Log.Info(typeof(App), m => m("---------------------"), LogTarget.Log);
			Log.Info(typeof(App), m => m("    Exin started     "), LogTarget.Log);
			Log.Info(typeof(App), m => m("---------------------"), LogTarget.Log);
		}
		
		protected override void OnStartup(StartupEventArgs e)
		{
			// Do not run multiple instances from the app
			ApplicationRunningHelper.SwitchToRunningInstanceIfExists();

			// Initializing IE emulation. Has to be before the first WebBrowser object is instantiated; so before
			// calling InitializeComponent() in MainWindow. Note, that this way there can't be any GUI logs here.
			WebBrowserHelpers.Init();
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
					// TODO we should prompt all the innter exceptions' messages
					if(_wrapperExceptionTypes.Any(type => type.IsInstanceOfType(exception)))
						plusMessage = exception.InnerException.Message;
				}

				promptMsg += plusMessage;
			}

			if(e.IsTerminating)
				Util.PromptErrorWindow(promptMsg);
		}

		// Here we can gather those unhandled exception types, which wraps the information
		private static Type[] _wrapperExceptionTypes = new[] {
			typeof(TypeInitializationException),
			typeof(XamlParseException),
		};

		#endregion

	}
}
