using System;
using System.Configuration;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;
using Common.Logging;
using System.Threading.Tasks;
using Common.Utils.Helpers;
using Exin.Common.Logging.CommonLogging.Loggers;

namespace Exin.Common.Logging.Core
{
	public delegate string MessageFormatterHandler(string format, params object[] args);
	public delegate string MessageFormatterLocalizedHandler(ResourceManager resourceManager, string resourceKey, params object[] args);

	public static class Log
	{
		private static AggregateLogger _core;
		public static AggregateLogger Core
		{
			get
			{
				if(_core == null)
				{
					LogManager.Adapter = LoggerFactoryAdapter;
					_core = LogManager.GetLogger<object>() as AggregateLogger;
					if(_core == null)
						throw new ConfigurationErrorsException("LogManager.GetLogger<object>() did not return an AggregateLogger instance. ");
				}

				return _core;
			}
		}

		#region Init

		private static ILoggerFactoryAdapter _loggerFactoryAdapter;
		public static ILoggerFactoryAdapter LoggerFactoryAdapter
		{
			get
			{
				if(_loggerFactoryAdapter == null)
				{
					const string msg = "Log.Init should be called, there is no LoggerFactoryAdapter set";
					System.Diagnostics.Debug.WriteLine(msg);
					throw new ConfigurationErrorsException(msg);
				}

				return _loggerFactoryAdapter;
			}
		}

		public static readonly TaskCompletionSource<object> LogInitializedDfd = new TaskCompletionSource<object>();

		public static void Init(ILoggerFactoryAdapter adapter)
		{
			_core = null;
			_loggerFactoryAdapter = adapter;

			LogInitializedDfd.TrySetResult(new object());
		}

		#endregion

		#region Extensions

		private static readonly Func<MessageFormatterHandler, string> NoCallback = null;

		#region General log at level (for default methods)
		public static Exception LogAtLevel(Func<MessageFormatterHandler, string> printMessageCallback, LogLevel logLevel, Exception exception = null, LogTarget logTarget = LogTarget.All)
		{
			return !Core.IsLevelEnabled(logLevel) ? exception : DoLog("", printMessageCallback, exception, logTarget, logLevel);
		}
		public static Exception LogAtLevel(Func<MessageFormatterLocalizedHandler, string> printMessageCallback, LogLevel logLevel, Exception exception = null, LogTarget logTarget = LogTarget.All)
		{
			return !Core.IsLevelEnabled(logLevel) ? exception : DoLog("", printMessageCallback, exception, logTarget, logLevel);
		}
		public static Exception LogAtLevel(Func<MessageFormatterHandler, CultureInfo, string> printMessageCallback, LogLevel logLevel, Exception exception = null, LogTarget logTarget = LogTarget.All)
		{
			return !Core.IsLevelEnabled(logLevel) ? exception : DoLog("", printMessageCallback, exception, logTarget, logLevel);
		}
		#endregion

		#region General log at level (for our extensions)
		public static Exception LogAtLevel(object callerTypeInstance, Func<MessageFormatterHandler, string> printMessageCallback, LogLevel logLevel, LogTarget logTarget = LogTarget.All, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsLevelEnabled(logLevel) ? exception : DoLog(callerTypeInstance, printMessageCallback, exception, logTarget, logLevel, callerFnName);
		}
		public static Exception LogAtLevel(object callerTypeInstance, Func<MessageFormatterLocalizedHandler, string> printMessageCallback, LogLevel logLevel, LogTarget logTarget = LogTarget.All, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsLevelEnabled(logLevel) ? exception : DoLog(callerTypeInstance, printMessageCallback, exception, logTarget, logLevel, callerFnName);
		}
		public static Exception LogAtLevel(object callerTypeInstance, Func<MessageFormatterHandler, CultureInfo, string> printMessageCallback, LogLevel logLevel, LogTarget logTarget = LogTarget.All, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsLevelEnabled(logLevel) ? exception : DoLog(callerTypeInstance, printMessageCallback, exception, logTarget, logLevel, callerFnName);
		}
		public static Exception LogAtLevel(object callerTypeInstance, LogLevel logLevel, Exception exception, LogTarget logTarget = LogTarget.All, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsLevelEnabled(logLevel) ? exception : DoLog(callerTypeInstance, NoCallback, exception, logTarget, logLevel, callerFnName);
		}

		public static Exception LogAtLevel(Type callerType, Func<MessageFormatterHandler, string> printMessageCallback, LogLevel logLevel, LogTarget logTarget = LogTarget.All, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsLevelEnabled(logLevel) ? exception : DoLog(callerType, printMessageCallback, exception, logTarget, logLevel, callerFnName);
		}
		public static Exception LogAtLevel(Type callerType, Func<MessageFormatterLocalizedHandler, string> printMessageCallback, LogLevel logLevel, LogTarget logTarget = LogTarget.All, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsLevelEnabled(logLevel) ? exception : DoLog(callerType, printMessageCallback, exception, logTarget, logLevel, callerFnName);
		}
		public static Exception LogAtLevel(Type callerType, Func<MessageFormatterHandler, CultureInfo, string> printMessageCallback, LogLevel logLevel, LogTarget logTarget = LogTarget.All, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsLevelEnabled(logLevel) ? exception : DoLog(callerType, printMessageCallback, exception, logTarget, logLevel, callerFnName);
		}
		public static Exception LogAtLevel(Type callerType, LogLevel logLevel, Exception exception, LogTarget logTarget = LogTarget.All, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsLevelEnabled(logLevel) ? exception : DoLog(callerType, NoCallback, exception, logTarget, logLevel, callerFnName);
		}
		#endregion

		#region Concrete log at levels
		public static Exception Trace(object callerTypeInstance, Func<MessageFormatterHandler, string> printMessageCallback, LogTarget logTarget = LogTarget.All, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsTraceEnabled ? exception : DoLog(callerTypeInstance, printMessageCallback, exception, logTarget, LogLevel.Trace, callerFnName);
		}
		public static Exception Trace(object callerTypeInstance, Func<MessageFormatterLocalizedHandler, string> printMessageCallback, LogTarget logTarget = LogTarget.All, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsTraceEnabled ? exception : DoLog(callerTypeInstance, printMessageCallback, exception, logTarget, LogLevel.Trace, callerFnName);
		}
		public static Exception Trace(object callerTypeInstance, Func<MessageFormatterHandler, CultureInfo, string> printMessageCallback, LogTarget logTarget = LogTarget.All, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsTraceEnabled ? exception : DoLog(callerTypeInstance, printMessageCallback, exception, logTarget, LogLevel.Trace, callerFnName);
		}

		public static Exception Trace(Type callerType, Func<MessageFormatterHandler, string> printMessageCallback, LogTarget logTarget = LogTarget.All, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsTraceEnabled ? exception : DoLog(callerType, printMessageCallback, exception, logTarget, LogLevel.Trace, callerFnName);
		}
		public static Exception Trace(Type callerType, Func<MessageFormatterLocalizedHandler, string> printMessageCallback, LogTarget logTarget = LogTarget.All, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsTraceEnabled ? exception : DoLog(callerType, printMessageCallback, exception, logTarget, LogLevel.Trace, callerFnName);
		}
		public static Exception Trace(Type callerType, Func<MessageFormatterHandler, CultureInfo, string> printMessageCallback, LogTarget logTarget = LogTarget.All, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsTraceEnabled ? exception : DoLog(callerType, printMessageCallback, exception, logTarget, LogLevel.Trace, callerFnName);
		}

		public static Exception Trace(object callerTypeInstance, Exception exception, LogTarget logTarget = LogTarget.All, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsTraceEnabled ? exception : DoLog(callerTypeInstance, NoCallback, exception, logTarget, LogLevel.Trace, callerFnName);
		}

		public static Exception Trace(Type callerType, Exception exception, LogTarget logTarget = LogTarget.All, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsTraceEnabled ? exception : DoLog(callerType, NoCallback, exception, logTarget, LogLevel.Trace, callerFnName);
		}

		public static Exception Debug(object callerTypeInstance, Func<MessageFormatterHandler, string> printMessageCallback, LogTarget logTarget = LogTarget.All, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsDebugEnabled ? exception : DoLog(callerTypeInstance, printMessageCallback, exception, logTarget, LogLevel.Debug, callerFnName);
		}
		public static Exception Debug(object callerTypeInstance, Func<MessageFormatterLocalizedHandler, string> printMessageCallback, LogTarget logTarget = LogTarget.All, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsDebugEnabled ? exception : DoLog(callerTypeInstance, printMessageCallback, exception, logTarget, LogLevel.Debug, callerFnName);
		}
		public static Exception Debug(object callerTypeInstance, Func<MessageFormatterHandler, CultureInfo, string> printMessageCallback, LogTarget logTarget = LogTarget.All, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsDebugEnabled ? exception : DoLog(callerTypeInstance, printMessageCallback, exception, logTarget, LogLevel.Debug, callerFnName);
		}

		public static Exception Debug(Type callerType, Func<MessageFormatterHandler, string> printMessageCallback, LogTarget logTarget = LogTarget.All, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsDebugEnabled ? exception : DoLog(callerType, printMessageCallback, exception, logTarget, LogLevel.Debug, callerFnName);
		}
		public static Exception Debug(Type callerType, Func<MessageFormatterLocalizedHandler, string> printMessageCallback, LogTarget logTarget = LogTarget.All, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsDebugEnabled ? exception : DoLog(callerType, printMessageCallback, exception, logTarget, LogLevel.Debug, callerFnName);
		}
		public static Exception Debug(Type callerType, Func<MessageFormatterHandler, CultureInfo, string> printMessageCallback, LogTarget logTarget = LogTarget.All, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsDebugEnabled ? exception : DoLog(callerType, printMessageCallback, exception, logTarget, LogLevel.Debug, callerFnName);
		}

		public static Exception Debug(object callerTypeInstance, Exception exception, LogTarget logTarget = LogTarget.All, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsDebugEnabled ? exception : DoLog(callerTypeInstance, NoCallback, exception, logTarget, LogLevel.Debug, callerFnName);
		}

		public static Exception Debug(Type callerType, Exception exception, LogTarget logTarget = LogTarget.All, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsDebugEnabled ? exception : DoLog(callerType, NoCallback, exception, logTarget, LogLevel.Debug, callerFnName);
		}

		public static Exception Info(object callerTypeInstance, Func<MessageFormatterHandler, string> printMessageCallback, LogTarget logTarget = LogTarget.All, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsInfoEnabled ? exception : DoLog(callerTypeInstance, printMessageCallback, exception, logTarget, LogLevel.Info, callerFnName);
		}
		public static Exception Info(object callerTypeInstance, Func<MessageFormatterLocalizedHandler, string> printMessageCallback, LogTarget logTarget = LogTarget.All, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsInfoEnabled ? exception : DoLog(callerTypeInstance, printMessageCallback, exception, logTarget, LogLevel.Info, callerFnName);
		}
		public static Exception Info(object callerTypeInstance, Func<MessageFormatterHandler, CultureInfo, string> printMessageCallback, LogTarget logTarget = LogTarget.All, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsInfoEnabled ? exception : DoLog(callerTypeInstance, printMessageCallback, exception, logTarget, LogLevel.Info, callerFnName);
		}

		public static Exception Info(Type callerType, Func<MessageFormatterHandler, string> printMessageCallback, LogTarget logTarget = LogTarget.All, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsInfoEnabled ? exception : DoLog(callerType, printMessageCallback, exception, logTarget, LogLevel.Info, callerFnName);
		}
		public static Exception Info(Type callerType, Func<MessageFormatterLocalizedHandler, string> printMessageCallback, LogTarget logTarget = LogTarget.All, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsInfoEnabled ? exception : DoLog(callerType, printMessageCallback, exception, logTarget, LogLevel.Info, callerFnName);
		}
		public static Exception Info(Type callerType, Func<MessageFormatterHandler, CultureInfo, string> printMessageCallback, LogTarget logTarget = LogTarget.All, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsInfoEnabled ? exception : DoLog(callerType, printMessageCallback, exception, logTarget, LogLevel.Info, callerFnName);
		}

		public static Exception Info(object callerTypeInstance, Exception exception, LogTarget logTarget = LogTarget.All, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsInfoEnabled ? exception : DoLog(callerTypeInstance, NoCallback, exception, logTarget, LogLevel.Info, callerFnName);
		}

		public static Exception Info(Type callerType, LogTarget logTarget = LogTarget.All, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsInfoEnabled ? exception : DoLog(callerType, NoCallback, exception, logTarget, LogLevel.Info, callerFnName);
		}

		public static Exception Warn(object callerTypeInstance, Func<MessageFormatterHandler, string> printMessageCallback, LogTarget logTarget = LogTarget.All, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsWarnEnabled ? exception : DoLog(callerTypeInstance, printMessageCallback, exception, logTarget, LogLevel.Warn, callerFnName);
		}
		public static Exception Warn(object callerTypeInstance, Func<MessageFormatterLocalizedHandler, string> printMessageCallback, LogTarget logTarget = LogTarget.All, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsWarnEnabled ? exception : DoLog(callerTypeInstance, printMessageCallback, exception, logTarget, LogLevel.Warn, callerFnName);
		}
		public static Exception Warn(object callerTypeInstance, Func<MessageFormatterHandler, CultureInfo, string> printMessageCallback, LogTarget logTarget = LogTarget.All, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsWarnEnabled ? exception : DoLog(callerTypeInstance, printMessageCallback, exception, logTarget, LogLevel.Warn, callerFnName);
		}

		public static Exception Warn(Type callerType, Func<MessageFormatterHandler, string> printMessageCallback, LogTarget logTarget = LogTarget.All, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsWarnEnabled ? exception : DoLog(callerType, printMessageCallback, exception, logTarget, LogLevel.Warn, callerFnName);
		}
		public static Exception Warn(Type callerType, Func<MessageFormatterLocalizedHandler, string> printMessageCallback, LogTarget logTarget = LogTarget.All, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsWarnEnabled ? exception : DoLog(callerType, printMessageCallback, exception, logTarget, LogLevel.Warn, callerFnName);
		}
		public static Exception Warn(Type callerType, Func<MessageFormatterHandler, CultureInfo, string> printMessageCallback, LogTarget logTarget = LogTarget.All, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsWarnEnabled ? exception : DoLog(callerType, printMessageCallback, exception, logTarget, LogLevel.Warn, callerFnName);
		}

		public static Exception Warn(object callerTypeInstance, Exception exception, LogTarget logTarget = LogTarget.All, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsWarnEnabled ? exception : DoLog(callerTypeInstance, NoCallback, exception, logTarget, LogLevel.Warn, callerFnName);
		}

		public static Exception Warn(Type callerType, Exception exception, LogTarget logTarget = LogTarget.All, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsWarnEnabled ? exception : DoLog(callerType, NoCallback, exception, logTarget, LogLevel.Warn, callerFnName);
		}

		public static Exception Error(object callerTypeInstance, Func<MessageFormatterHandler, string> printMessageCallback, LogTarget logTarget = LogTarget.All, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsErrorEnabled ? exception : DoLog(callerTypeInstance, printMessageCallback, exception, logTarget, LogLevel.Error, callerFnName);
		}
		public static Exception Error(object callerTypeInstance, Func<MessageFormatterLocalizedHandler, string> printMessageCallback, LogTarget logTarget = LogTarget.All, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsErrorEnabled ? exception : DoLog(callerTypeInstance, printMessageCallback, exception, logTarget, LogLevel.Error, callerFnName);
		}
		public static Exception Error(object callerTypeInstance, Func<MessageFormatterHandler, CultureInfo, string> printMessageCallback, LogTarget logTarget = LogTarget.All, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsErrorEnabled ? exception : DoLog(callerTypeInstance, printMessageCallback, exception, logTarget, LogLevel.Error, callerFnName);
		}

		public static Exception Error(Type callerType, Func<MessageFormatterHandler, string> printMessageCallback, LogTarget logTarget = LogTarget.All, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsErrorEnabled ? exception : DoLog(callerType, printMessageCallback, exception, logTarget, LogLevel.Error, callerFnName);
		}
		public static Exception Error(Type callerType, Func<MessageFormatterLocalizedHandler, string> printMessageCallback, LogTarget logTarget = LogTarget.All, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsErrorEnabled ? exception : DoLog(callerType, printMessageCallback, exception, logTarget, LogLevel.Error, callerFnName);
		}
		public static Exception Error(Type callerType, Func<MessageFormatterHandler, CultureInfo, string> printMessageCallback, LogTarget logTarget = LogTarget.All, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsErrorEnabled ? exception : DoLog(callerType, printMessageCallback, exception, logTarget, LogLevel.Error, callerFnName);
		}

		public static Exception Error(object callerTypeInstance, Exception exception, LogTarget logTarget = LogTarget.All, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsErrorEnabled ? exception : DoLog(callerTypeInstance, NoCallback, exception, logTarget, LogLevel.Error, callerFnName);
		}

		public static Exception Error(Type callerType, Exception exception, LogTarget logTarget = LogTarget.All, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsErrorEnabled ? exception : DoLog(callerType, NoCallback, exception, logTarget, LogLevel.Error, callerFnName);
		}

		public static Exception Fatal(object callerTypeInstance, Func<MessageFormatterHandler, string> printMessageCallback, LogTarget logTarget = LogTarget.All, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsFatalEnabled ? exception : DoLog(callerTypeInstance, printMessageCallback, exception, logTarget, LogLevel.Fatal, callerFnName);
		}
		public static Exception Fatal(object callerTypeInstance, Func<MessageFormatterLocalizedHandler, string> printMessageCallback, LogTarget logTarget = LogTarget.All, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsFatalEnabled ? exception : DoLog(callerTypeInstance, printMessageCallback, exception, logTarget, LogLevel.Fatal, callerFnName);
		}
		public static Exception Fatal(object callerTypeInstance, Func<MessageFormatterHandler, CultureInfo, string> printMessageCallback, LogTarget logTarget = LogTarget.All, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsFatalEnabled ? exception : DoLog(callerTypeInstance, printMessageCallback, exception, logTarget, LogLevel.Fatal, callerFnName);
		}

		public static Exception Fatal(Type callerType, Func<MessageFormatterHandler, string> printMessageCallback, LogTarget logTarget = LogTarget.All, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsFatalEnabled ? exception : DoLog(callerType, printMessageCallback, exception, logTarget, LogLevel.Fatal, callerFnName);
		}
		public static Exception Fatal(Type callerType, Func<MessageFormatterLocalizedHandler, string> printMessageCallback, LogTarget logTarget = LogTarget.All, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsFatalEnabled ? exception : DoLog(callerType, printMessageCallback, exception, logTarget, LogLevel.Fatal, callerFnName);
		}
		public static Exception Fatal(Type callerType, Func<MessageFormatterHandler, CultureInfo, string> printMessageCallback, LogTarget logTarget = LogTarget.All, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsFatalEnabled ? exception : DoLog(callerType, printMessageCallback, exception, logTarget, LogLevel.Fatal, callerFnName);
		}

		public static Exception Fatal(object callerTypeInstance, Exception exception, LogTarget logTarget = LogTarget.All, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsFatalEnabled ? exception : DoLog(callerTypeInstance, NoCallback, exception, logTarget, LogLevel.Fatal, callerFnName);
		}

		public static Exception Fatal(Type callerType, LogTarget logTarget = LogTarget.All, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsFatalEnabled ? exception : DoLog(callerType, NoCallback, exception, logTarget, LogLevel.Fatal, callerFnName);
		}
		#endregion

		#region DoLog

		private static Exception DoLog(object callerTypeInstance, object printMessageCallback, Exception exception, LogTarget logTarget, LogLevel logLevel, string callerFnName)
		{
			return DoLog(GetTag(callerTypeInstance, callerFnName), printMessageCallback, exception, logTarget, logLevel);
		}
		private static Exception DoLog(Type callerType, object printMessageCallback, Exception exception, LogTarget logTarget, LogLevel logLevel, string callerFnName)
		{
			return DoLog(GetTag(callerType, callerFnName), printMessageCallback, exception, logTarget, logLevel);
		}

		private static Exception DoLog(
			string tag,
			object messageFormatterHandler,
			Exception exception,
			LogTarget logTarget,
			LogLevel logLevel
			)
		{
			LogData logData;
			Func<MessageFormatterHandler, string> stringFormatHandler;
			Func<MessageFormatterLocalizedHandler, string> resourceManagerHandler;
			Func<MessageFormatterHandler, CultureInfo, string> dynamicLocalizedHandler;

			if(messageFormatterHandler == null)
				logData = new LogData(tag, NoCallback, exception, logTarget, logLevel);
			else if((stringFormatHandler = messageFormatterHandler as Func<MessageFormatterHandler, string>) != null)
				logData = new LogData(tag, stringFormatHandler, exception, logTarget, logLevel);
			else if((resourceManagerHandler = messageFormatterHandler as Func<MessageFormatterLocalizedHandler, string>) != null)
				logData = new LogData(tag, resourceManagerHandler, exception, logTarget, logLevel);
			else if((dynamicLocalizedHandler = messageFormatterHandler as Func<MessageFormatterHandler, CultureInfo, string>) != null)
				logData = new LogData(tag, dynamicLocalizedHandler, exception, logTarget, logLevel);
			else
				throw new ArgumentOutOfRangeException("messageFormatterHandler");

			Core.DoLog(logData);
			return exception;
		}

		#endregion

		#endregion

		#region Helpers
		public static string GetTag(object instance, [CallerMemberName] string callerFnName = null)
		{
			return GetTag(instance.GetType().Name, callerFnName);
		}

		public static string GetTag(Type type, [CallerMemberName] string callerFnName = null)
		{
			return GetTag(type.Name, callerFnName);
		}

		public static string GetTag(string className, [CallerMemberName] string callerFnName = null)
		{
			return "{0}.{1}: ".Formatted(className, callerFnName ?? "(?)");
		}
		#endregion
	}
}