using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Windows.Controls;
using Common.Configuration;
using Common.Logging;
using Common.Utils.Helpers;
using Exin.Common.Logging.CommonLogging;
using Exin.Common.Logging.CommonLogging.Loggers;
using Exin.Common.Logging.Log4Net;

namespace Exin.Common.Logging.Core
{
	public static class LogInit
	{
		public const string DateTimeFormat = "yyyy/MM/dd HH:mm:ss:fff";

		#region LogLevels

		// TODO from some config file, to be able to change the level in production
		// TODO different log level for UI and Log, and maybe for TransportData

		private static LogLevel? _uiLoggerLevel;
		public static LogLevel UiLoggerLevel
		{
			get
			{
				if(_uiLoggerLevel.HasValue)
					return _uiLoggerLevel.Value;

				LogLevel logLevel;
				try
				{
#if DEBUG
					logLevel = Config.MainSettings.Logging.UiLoggerLevel;
					//logLevel = LogLevel.Info;
#else
					logLevel = Config.MainSettings.Logging.UiLoggerLevel;
#endif
				}
				catch(Exception e)
				{
					Debug.WriteLine("Could not read this: Config.MainSettings.Logging.UiLoggerLevel. Exception.Message: " + e.Message);
					logLevel = LogLevel.Info;
				}
				return logLevel;
			}
		}

		private static LogLevel? _logLoggerLevel;
		public static LogLevel LogLoggerLevel
		{
			get
			{
				if(_logLoggerLevel.HasValue)
					return _logLoggerLevel.Value;

				LogLevel logLevel;
				try
				{
#if DEBUG
					logLevel = Config.MainSettings.Logging.LogLoggerLevel;
					//logLevel = LogLevel.Info;
#else
					logLevel = Config.MainSettings.Logging.LogLoggerLevel;
#endif
				}
				catch(Exception e)
				{
					Debug.WriteLine("Could not read this: Config.MainSettings.Logging.LogLoggerLevel. Exception.Message: " + e.Message);
					logLevel = LogLevel.Info;
				}
				return logLevel;
			}
		}

		public static LogLevel AggregateLoggerLevel => Helpers.Min(UiLoggerLevel, LogLoggerLevel);

		#endregion

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
					logLevel: LogLoggerLevel, 
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
				logLevel: UiLoggerLevel,
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
						logLevel: UiLoggerLevel,
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
			Log.Init(CreateAggregateLogger(loggerInstances));
		}

		private static ILoggerFactoryAdapter CreateAggregateLogger(LoggerInstancesArgs loggerInstances)
		{
			return new AggregateLoggerFactoryAdapter(
				// Dummies
				showDateTime: true,
				showLogName: false,
				showLevel: true,
				dateTimeFormat: DateTimeFormat,
				// 
				logLevel: AggregateLoggerLevel,
				instances: loggerInstances
			);
		}

		private static ILoggerFactoryAdapter _defaultLoggerFactoryAdapter;
		public static ILoggerFactoryAdapter DefaultLoggerFactoryAdapter
		{
			get
			{
				if(_defaultLoggerFactoryAdapter == null)
					_defaultLoggerFactoryAdapter = CreateAggregateLogger(new LoggerInstancesArgs() {
						UiLoggers = new List<IExinLog>(0),
						LogLoggers = new List<IExinLog>(1) {
							new Log4NetLogger(Log4NetLog.Create())
						},
					});

				return _defaultLoggerFactoryAdapter;
			}
		}
	}
}

