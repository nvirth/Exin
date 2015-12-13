using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Windows.Controls;
using Common.Log.CommonLogging;
using Common.Log.CommonLogging.Loggers;
using Common.Log.Log4Net;
using Common.Logging;

namespace Common.Log.Core
{
	public static class LogInit
	{
		public const string DateTimeFormat = "yyyy/MM/dd HH:mm:ss:fff";

		// TODO from some config file, to be able to change the level in production
		public static LogLevel GetLogLevel()
		{
#if DEBUG
			return LogLevel.Debug;
#else
			return LogLevel.Info;
#endif
		}

		#region Wpf App

		public static void InitWpfAppLogLoggers()
		{
			InitCommon(GetLoggersForWpfApp());
		}

		public static void InitWpfAppUiLoggers(RichTextBox richTextBox)
		{
			var aggregateLoggerFactoryAdapter = Core.Log.LoggerFactoryAdapter as AggregateLoggerFactoryAdapter;
			var aggregateLogger = Core.Log.Core as AggregateLogger;
			if(aggregateLogger == null || aggregateLoggerFactoryAdapter == null)
				throw new ConfigurationErrorsException("aggregateLogger == null || aggregateLoggerFactoryAdapter == null");

			var uiLogger = GetRichTextBoxLogger(richTextBox);
			aggregateLoggerFactoryAdapter.LoggerInstances.UiLoggers.Add(uiLogger);
			aggregateLogger.UiLoggers.Add(uiLogger);
		}

		private static LoggerInstancesArgs GetLoggersForWpfApp()
		{
			var logLoggers = new List<IExinLog>(Debugger.IsAttached ? 2 : 1) {
				new Log4NetLogger(Log4NetLog.Create()),
			};
			if(Debugger.IsAttached)
			{
				logLoggers.Add(new DebugWriteLogger(
					logName: typeof(DebugWriteLogger).Name,
					logLevel: GetLogLevel(),
					showDateTime: true,
					showLogName: false,
					showLevel: true,
					dateTimeFormat: DateTimeFormat
					));
			}

			var result = new LoggerInstancesArgs() {
				UiLoggers = new List<IExinLog>(1), // Initialized later
				LogLoggers = logLoggers,
			};
			return result;
		}

		private static IExinLog GetRichTextBoxLogger(RichTextBox richTextBox)
		{
			var uiLogger = new WpfRichTextBoxLogger(
				logName: typeof(WpfRichTextBoxLogger).Name,
				logLevel: GetLogLevel(),
				showDateTime: false,
				showLogName: false,
				showLevel: false,
				dateTimeFormat: DateTimeFormat,
				richTextBox: richTextBox
				);
			return uiLogger;
		}

		#endregion

		#region TransportData

		public static void InitTransportData()
		{
			InitCommon(GetLoggersForTransportData());
		}

		private static LoggerInstancesArgs GetLoggersForTransportData()
		{
			var result = new LoggerInstancesArgs() {
				UiLoggers = new List<IExinLog>(1) {
					new ConsoleColorOutLogger(
						logName: typeof(ConsoleColorOutLogger).Name,
						logLevel: GetLogLevel(),
						showDateTime: false,
						showLogName: false,
						showLevel: false,
						dateTimeFormat: DateTimeFormat,
						useColor: true
						)
				},
				LogLoggers = new List<IExinLog>(1) {
					new Log4NetLogger(Log4NetLog.Create()),
				}
			};
			return result;
		}

		#endregion

		private static void InitCommon(LoggerInstancesArgs loggerInstances)
		{
			Core.Log.Init(
				new AggregateLoggerFactoryAdapter(
					// Dummies
					showDateTime: true,
					showLogName: false,
					showLevel: true,
					dateTimeFormat: DateTimeFormat,
					// 
					logLevel: GetLogLevel(),
					instances: loggerInstances
				)
			);
		}
	}
}

