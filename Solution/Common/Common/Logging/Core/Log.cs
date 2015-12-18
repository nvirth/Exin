using System;
using System.Configuration;
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
		public static Exception LogAtLevel(Func<MessageFormatterHandler, string> printMessageCallback, LogLevel logLevel, Exception exception = null, LogTarget logTarget = LogTarget.Log)
		{
			if(!Core.IsLevelEnabled(logLevel))
				return null;

			switch(logLevel)
			{
				case LogLevel.Trace: return DoLog("", printMessageCallback, null, exception, logTarget, LogLevel.Trace);
				case LogLevel.Debug: return DoLog("", printMessageCallback, null, exception, logTarget, LogLevel.Debug);
				case LogLevel.Info: return DoLog("", printMessageCallback, null, exception, logTarget, LogLevel.Info);
				case LogLevel.Warn: return DoLog("", printMessageCallback, null, exception, logTarget, LogLevel.Warn);
				case LogLevel.Error: return DoLog("", printMessageCallback, null, exception, logTarget, LogLevel.Error);
				case LogLevel.Fatal: return DoLog("", printMessageCallback, null, exception, logTarget, LogLevel.Fatal);
				default:
					throw new NotImplementedException(GetTag(typeof(Log)));
			}
		}
		public static Exception LogAtLevel(Func<MessageFormatterLocalizedHandler, string> printMessageCallback, LogLevel logLevel, Exception exception = null, LogTarget logTarget = LogTarget.Log)
		{
			if(!Core.IsLevelEnabled(logLevel))
				return null;

			switch(logLevel)
			{
				case LogLevel.Trace: return DoLog("", null, printMessageCallback, exception, logTarget, LogLevel.Trace);
				case LogLevel.Debug: return DoLog("", null, printMessageCallback, exception, logTarget, LogLevel.Debug);
				case LogLevel.Info: return DoLog("", null, printMessageCallback, exception, logTarget, LogLevel.Info);
				case LogLevel.Warn: return DoLog("", null, printMessageCallback, exception, logTarget, LogLevel.Warn);
				case LogLevel.Error: return DoLog("", null, printMessageCallback, exception, logTarget, LogLevel.Error);
				case LogLevel.Fatal: return DoLog("", null, printMessageCallback, exception, logTarget, LogLevel.Fatal);
				default:
					throw new NotImplementedException(GetTag(typeof(Log)));
			}
		}
		#endregion

		#region General log at level (for our extensions)
		public static Exception LogAtLevel(object callerTypeInstance, Func<MessageFormatterHandler, string> printMessageCallback, LogLevel logLevel, LogTarget logTarget = LogTarget.Log, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			if(!Core.IsLevelEnabled(logLevel))
				return null;

			switch(logLevel)
			{
				case LogLevel.Trace:
					return DoLog(callerTypeInstance, printMessageCallback, exception, logTarget, LogLevel.Trace, callerFnName);
				case LogLevel.Debug:
					return DoLog(callerTypeInstance, printMessageCallback, exception, logTarget, LogLevel.Debug, callerFnName);
				case LogLevel.Info:
					return DoLog(callerTypeInstance, printMessageCallback, exception, logTarget, LogLevel.Info, callerFnName);
				case LogLevel.Warn:
					return DoLog(callerTypeInstance, printMessageCallback, exception, logTarget, LogLevel.Warn, callerFnName);
				case LogLevel.Error:
					return DoLog(callerTypeInstance, printMessageCallback, exception, logTarget, LogLevel.Error, callerFnName);
				case LogLevel.Fatal:
					return DoLog(callerTypeInstance, printMessageCallback, exception, logTarget, LogLevel.Fatal, callerFnName);
				default:
					throw new NotImplementedException(GetTag(typeof(Log)));
			}
		}
		public static Exception LogAtLevel(object callerTypeInstance, Func<MessageFormatterLocalizedHandler, string> printMessageCallback, LogLevel logLevel, LogTarget logTarget = LogTarget.Log, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			if(!Core.IsLevelEnabled(logLevel))
				return null;

			switch(logLevel)
			{
				case LogLevel.Trace:
					return DoLog(callerTypeInstance, printMessageCallback, exception, logTarget, LogLevel.Trace, callerFnName);
				case LogLevel.Debug:
					return DoLog(callerTypeInstance, printMessageCallback, exception, logTarget, LogLevel.Debug, callerFnName);
				case LogLevel.Info:
					return DoLog(callerTypeInstance, printMessageCallback, exception, logTarget, LogLevel.Info, callerFnName);
				case LogLevel.Warn:
					return DoLog(callerTypeInstance, printMessageCallback, exception, logTarget, LogLevel.Warn, callerFnName);
				case LogLevel.Error:
					return DoLog(callerTypeInstance, printMessageCallback, exception, logTarget, LogLevel.Error, callerFnName);
				case LogLevel.Fatal:
					return DoLog(callerTypeInstance, printMessageCallback, exception, logTarget, LogLevel.Fatal, callerFnName);
				default:
					throw new NotImplementedException(GetTag(typeof(Log)));
			}
		}
		public static Exception LogAtLevel(object callerTypeInstance, LogLevel logLevel, Exception exception, LogTarget logTarget = LogTarget.Log, [CallerMemberName] string callerFnName = null)
		{
			if(!Core.IsLevelEnabled(logLevel))
				return null;

			switch(logLevel)
			{
				case LogLevel.Trace:
					return DoLog(callerTypeInstance, NoCallback, exception, logTarget, LogLevel.Trace, callerFnName);
				case LogLevel.Debug:
					return DoLog(callerTypeInstance, NoCallback, exception, logTarget, LogLevel.Debug, callerFnName);
				case LogLevel.Info:
					return DoLog(callerTypeInstance, NoCallback, exception, logTarget, LogLevel.Info, callerFnName);
				case LogLevel.Warn:
					return DoLog(callerTypeInstance, NoCallback, exception, logTarget, LogLevel.Warn, callerFnName);
				case LogLevel.Error:
					return DoLog(callerTypeInstance, NoCallback, exception, logTarget, LogLevel.Error, callerFnName);
				case LogLevel.Fatal:
					return DoLog(callerTypeInstance, NoCallback, exception, logTarget, LogLevel.Fatal, callerFnName);
				default:
					throw new NotImplementedException(GetTag(typeof(Log)));
			}
		}

		public static Exception LogAtLevel(Type callerType, Func<MessageFormatterHandler, string> printMessageCallback, LogLevel logLevel, LogTarget logTarget = LogTarget.Log, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			if(!Core.IsLevelEnabled(logLevel))
				return null;

			switch(logLevel)
			{
				case LogLevel.Trace:
					return DoLog(callerType, printMessageCallback, exception, logTarget, LogLevel.Trace, callerFnName);
				case LogLevel.Debug:
					return DoLog(callerType, printMessageCallback, exception, logTarget, LogLevel.Debug, callerFnName);
				case LogLevel.Info:
					return DoLog(callerType, printMessageCallback, exception, logTarget, LogLevel.Info, callerFnName);
				case LogLevel.Warn:
					return DoLog(callerType, printMessageCallback, exception, logTarget, LogLevel.Warn, callerFnName);
				case LogLevel.Error:
					return DoLog(callerType, printMessageCallback, exception, logTarget, LogLevel.Error, callerFnName);
				case LogLevel.Fatal:
					return DoLog(callerType, printMessageCallback, exception, logTarget, LogLevel.Fatal, callerFnName);
				default:
					throw new NotImplementedException(GetTag(typeof(Log)));
			}
		}
		public static Exception LogAtLevel(Type callerType, Func<MessageFormatterLocalizedHandler, string> printMessageCallback, LogLevel logLevel, LogTarget logTarget = LogTarget.Log, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			if(!Core.IsLevelEnabled(logLevel))
				return null;

			switch(logLevel)
			{
				case LogLevel.Trace:
					return DoLog(callerType, printMessageCallback, exception, logTarget, LogLevel.Trace, callerFnName);
				case LogLevel.Debug:
					return DoLog(callerType, printMessageCallback, exception, logTarget, LogLevel.Debug, callerFnName);
				case LogLevel.Info:
					return DoLog(callerType, printMessageCallback, exception, logTarget, LogLevel.Info, callerFnName);
				case LogLevel.Warn:
					return DoLog(callerType, printMessageCallback, exception, logTarget, LogLevel.Warn, callerFnName);
				case LogLevel.Error:
					return DoLog(callerType, printMessageCallback, exception, logTarget, LogLevel.Error, callerFnName);
				case LogLevel.Fatal:
					return DoLog(callerType, printMessageCallback, exception, logTarget, LogLevel.Fatal, callerFnName);
				default:
					throw new NotImplementedException(GetTag(typeof(Log)));
			}
		}
		public static Exception LogAtLevel(Type callerType, LogLevel logLevel, Exception exception, LogTarget logTarget = LogTarget.Log, [CallerMemberName] string callerFnName = null)
		{
			if(!Core.IsLevelEnabled(logLevel))
				return null;

			switch(logLevel)
			{
				case LogLevel.Trace:
					return DoLog(callerType, NoCallback, exception, logTarget, LogLevel.Trace, callerFnName);
				case LogLevel.Debug:
					return DoLog(callerType, NoCallback, exception, logTarget, LogLevel.Debug, callerFnName);
				case LogLevel.Info:
					return DoLog(callerType, NoCallback, exception, logTarget, LogLevel.Info, callerFnName);
				case LogLevel.Warn:
					return DoLog(callerType, NoCallback, exception, logTarget, LogLevel.Warn, callerFnName);
				case LogLevel.Error:
					return DoLog(callerType, NoCallback, exception, logTarget, LogLevel.Error, callerFnName);
				case LogLevel.Fatal:
					return DoLog(callerType, NoCallback, exception, logTarget, LogLevel.Fatal, callerFnName);
				default:
					throw new NotImplementedException(GetTag(typeof(Log)));
			}
		}
		#endregion

		#region Concrete log at levels
		public static Exception Trace(object callerTypeInstance, Func<MessageFormatterHandler, string> printMessageCallback, LogTarget logTarget = LogTarget.Log, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsTraceEnabled ? null : DoLog(callerTypeInstance, printMessageCallback, exception, logTarget, LogLevel.Trace, callerFnName);
		}
		public static Exception Trace(object callerTypeInstance, Func<MessageFormatterLocalizedHandler, string> printMessageCallback, LogTarget logTarget = LogTarget.Log, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsTraceEnabled ? null : DoLog(callerTypeInstance, printMessageCallback, exception, logTarget, LogLevel.Trace, callerFnName);
		}

		public static Exception Trace(Type callerType, Func<MessageFormatterHandler, string> printMessageCallback, LogTarget logTarget = LogTarget.Log, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsTraceEnabled ? null : DoLog(callerType, printMessageCallback, exception, logTarget, LogLevel.Trace, callerFnName);
		}
		public static Exception Trace(Type callerType, Func<MessageFormatterLocalizedHandler, string> printMessageCallback, LogTarget logTarget = LogTarget.Log, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsTraceEnabled ? null : DoLog(callerType, printMessageCallback, exception, logTarget, LogLevel.Trace, callerFnName);
		}

		public static Exception Trace(object callerTypeInstance, Exception exception, LogTarget logTarget = LogTarget.Log, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsTraceEnabled ? null : DoLog(callerTypeInstance, NoCallback, exception, logTarget, LogLevel.Trace, callerFnName);
		}

		public static Exception Trace(Type callerType, Exception exception, LogTarget logTarget = LogTarget.Log, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsTraceEnabled ? null : DoLog(callerType, NoCallback, exception, logTarget, LogLevel.Trace, callerFnName);
		}

		public static Exception Debug(object callerTypeInstance, Func<MessageFormatterHandler, string> printMessageCallback, LogTarget logTarget = LogTarget.Log, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsDebugEnabled ? null : DoLog(callerTypeInstance, printMessageCallback, exception, logTarget, LogLevel.Debug, callerFnName);
		}
		public static Exception Debug(object callerTypeInstance, Func<MessageFormatterLocalizedHandler, string> printMessageCallback, LogTarget logTarget = LogTarget.Log, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsDebugEnabled ? null : DoLog(callerTypeInstance, printMessageCallback, exception, logTarget, LogLevel.Debug, callerFnName);
		}

		public static Exception Debug(Type callerType, Func<MessageFormatterHandler, string> printMessageCallback, LogTarget logTarget = LogTarget.Log, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsDebugEnabled ? null : DoLog(callerType, printMessageCallback, exception, logTarget, LogLevel.Debug, callerFnName);
		}
		public static Exception Debug(Type callerType, Func<MessageFormatterLocalizedHandler, string> printMessageCallback, LogTarget logTarget = LogTarget.Log, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsDebugEnabled ? null : DoLog(callerType, printMessageCallback, exception, logTarget, LogLevel.Debug, callerFnName);
		}

		public static Exception Debug(object callerTypeInstance, Exception exception, LogTarget logTarget = LogTarget.Log, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsDebugEnabled ? null : DoLog(callerTypeInstance, NoCallback, exception, logTarget, LogLevel.Debug, callerFnName);
		}

		public static Exception Debug(Type callerType, Exception exception, LogTarget logTarget = LogTarget.Log, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsDebugEnabled ? null : DoLog(callerType, NoCallback, exception, logTarget, LogLevel.Debug, callerFnName);
		}

		public static Exception Info(object callerTypeInstance, Func<MessageFormatterHandler, string> printMessageCallback, LogTarget logTarget = LogTarget.Log, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsInfoEnabled ? null : DoLog(callerTypeInstance, printMessageCallback, exception, logTarget, LogLevel.Info, callerFnName);
		}
		public static Exception Info(object callerTypeInstance, Func<MessageFormatterLocalizedHandler, string> printMessageCallback, LogTarget logTarget = LogTarget.Log, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsInfoEnabled ? null : DoLog(callerTypeInstance, printMessageCallback, exception, logTarget, LogLevel.Info, callerFnName);
		}

		public static Exception Info(Type callerType, Func<MessageFormatterHandler, string> printMessageCallback, LogTarget logTarget = LogTarget.Log, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsInfoEnabled ? null : DoLog(callerType, printMessageCallback, exception, logTarget, LogLevel.Info, callerFnName);
		}
		public static Exception Info(Type callerType, Func<MessageFormatterLocalizedHandler, string> printMessageCallback, LogTarget logTarget = LogTarget.Log, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsInfoEnabled ? null : DoLog(callerType, printMessageCallback, exception, logTarget, LogLevel.Info, callerFnName);
		}

		public static Exception Info(object callerTypeInstance, Exception exception, LogTarget logTarget = LogTarget.Log, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsInfoEnabled ? null : DoLog(callerTypeInstance, NoCallback, exception, logTarget, LogLevel.Info, callerFnName);
		}

		public static Exception Info(Type callerType, LogTarget logTarget = LogTarget.Log, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsInfoEnabled ? null : DoLog(callerType, NoCallback, exception, logTarget, LogLevel.Info, callerFnName);
		}

		public static Exception Warn(object callerTypeInstance, Func<MessageFormatterHandler, string> printMessageCallback, LogTarget logTarget = LogTarget.Log, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsWarnEnabled ? null : DoLog(callerTypeInstance, printMessageCallback, exception, logTarget, LogLevel.Warn, callerFnName);
		}
		public static Exception Warn(object callerTypeInstance, Func<MessageFormatterLocalizedHandler, string> printMessageCallback, LogTarget logTarget = LogTarget.Log, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsWarnEnabled ? null : DoLog(callerTypeInstance, printMessageCallback, exception, logTarget, LogLevel.Warn, callerFnName);
		}

		public static Exception Warn(Type callerType, Func<MessageFormatterHandler, string> printMessageCallback, LogTarget logTarget = LogTarget.Log, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsWarnEnabled ? null : DoLog(callerType, printMessageCallback, exception, logTarget, LogLevel.Warn, callerFnName);
		}
		public static Exception Warn(Type callerType, Func<MessageFormatterLocalizedHandler, string> printMessageCallback, LogTarget logTarget = LogTarget.Log, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsWarnEnabled ? null : DoLog(callerType, printMessageCallback, exception, logTarget, LogLevel.Warn, callerFnName);
		}

		public static Exception Warn(object callerTypeInstance, Exception exception, LogTarget logTarget = LogTarget.Log, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsWarnEnabled ? null : DoLog(callerTypeInstance, NoCallback, exception, logTarget, LogLevel.Warn, callerFnName);
		}

		public static Exception Warn(Type callerType, Exception exception, LogTarget logTarget = LogTarget.Log, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsWarnEnabled ? null : DoLog(callerType, NoCallback, exception, logTarget, LogLevel.Warn, callerFnName);
		}

		public static Exception Error(object callerTypeInstance, Func<MessageFormatterHandler, string> printMessageCallback, LogTarget logTarget = LogTarget.Log, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsErrorEnabled ? null : DoLog(callerTypeInstance, printMessageCallback, exception, logTarget, LogLevel.Error, callerFnName);
		}
		public static Exception Error(object callerTypeInstance, Func<MessageFormatterLocalizedHandler, string> printMessageCallback, LogTarget logTarget = LogTarget.Log, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsErrorEnabled ? null : DoLog(callerTypeInstance, printMessageCallback, exception, logTarget, LogLevel.Error, callerFnName);
		}

		public static Exception Error(Type callerType, Func<MessageFormatterHandler, string> printMessageCallback, LogTarget logTarget = LogTarget.Log, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsErrorEnabled ? null : DoLog(callerType, printMessageCallback, exception, logTarget, LogLevel.Error, callerFnName);
		}
		public static Exception Error(Type callerType, Func<MessageFormatterLocalizedHandler, string> printMessageCallback, LogTarget logTarget = LogTarget.Log, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsErrorEnabled ? null : DoLog(callerType, printMessageCallback, exception, logTarget, LogLevel.Error, callerFnName);
		}

		public static Exception Error(object callerTypeInstance, Exception exception, LogTarget logTarget = LogTarget.Log, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsErrorEnabled ? null : DoLog(callerTypeInstance, NoCallback, exception, logTarget, LogLevel.Error, callerFnName);
		}

		public static Exception Error(Type callerType, Exception exception, LogTarget logTarget = LogTarget.Log, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsErrorEnabled ? null : DoLog(callerType, NoCallback, exception, logTarget, LogLevel.Error, callerFnName);
		}

		public static Exception Fatal(object callerTypeInstance, Func<MessageFormatterHandler, string> printMessageCallback, LogTarget logTarget = LogTarget.Log, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsFatalEnabled ? null : DoLog(callerTypeInstance, printMessageCallback, exception, logTarget, LogLevel.Fatal, callerFnName);
		}
		public static Exception Fatal(object callerTypeInstance, Func<MessageFormatterLocalizedHandler, string> printMessageCallback, LogTarget logTarget = LogTarget.Log, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsFatalEnabled ? null : DoLog(callerTypeInstance, printMessageCallback, exception, logTarget, LogLevel.Fatal, callerFnName);
		}

		public static Exception Fatal(Type callerType, Func<MessageFormatterHandler, string> printMessageCallback, LogTarget logTarget = LogTarget.Log, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsFatalEnabled ? null : DoLog(callerType, printMessageCallback, exception, logTarget, LogLevel.Fatal, callerFnName);
		}
		public static Exception Fatal(Type callerType, Func<MessageFormatterLocalizedHandler, string> printMessageCallback, LogTarget logTarget = LogTarget.Log, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsFatalEnabled ? null : DoLog(callerType, printMessageCallback, exception, logTarget, LogLevel.Fatal, callerFnName);
		}

		public static Exception Fatal(object callerTypeInstance, Exception exception, LogTarget logTarget = LogTarget.Log, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsFatalEnabled ? null : DoLog(callerTypeInstance, NoCallback, exception, logTarget, LogLevel.Fatal, callerFnName);
		}

		public static Exception Fatal(Type callerType, LogTarget logTarget = LogTarget.Log, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsFatalEnabled ? null : DoLog(callerType, NoCallback, exception, logTarget, LogLevel.Fatal, callerFnName);
		}
		#endregion

		#region DoLog

		private static Exception DoLog(object callerTypeInstance, Func<MessageFormatterHandler, string> printMessageCallback, Exception exception, LogTarget logTarget, LogLevel logLevel, string callerFnName)
		{
			return DoLog(GetTag(callerTypeInstance, callerFnName), printMessageCallback, null, exception, logTarget, logLevel);
		}
		private static Exception DoLog(object callerTypeInstance, Func<MessageFormatterLocalizedHandler, string> printMessageCallback, Exception exception, LogTarget logTarget, LogLevel logLevel, string callerFnName)
		{
			return DoLog(GetTag(callerTypeInstance, callerFnName), null, printMessageCallback, exception, logTarget, logLevel);
		}

		private static Exception DoLog(Type callerType, Func<MessageFormatterHandler, string> printMessageCallback, Exception exception, LogTarget logTarget, LogLevel logLevel, string callerFnName)
		{
			return DoLog(GetTag(callerType, callerFnName), printMessageCallback, null, exception, logTarget, logLevel);
		}
		private static Exception DoLog(Type callerType, Func<MessageFormatterLocalizedHandler, string> printMessageCallback, Exception exception, LogTarget logTarget, LogLevel logLevel, string callerFnName)
		{
			return DoLog(GetTag(callerType, callerFnName), null, printMessageCallback, exception, logTarget, logLevel);
		}

		private static Exception DoLog(
			string tag,
			Func<MessageFormatterHandler, string> messageFormatterHandler,
			Func<MessageFormatterLocalizedHandler, string> messageFormatterLocalizedHandler,
			Exception exception,
			LogTarget logTarget,
			LogLevel logLevel
			)
		{
			var logData = messageFormatterHandler == null
				? new LogData(tag, messageFormatterLocalizedHandler, exception, logTarget, logLevel)
				: new LogData(tag, messageFormatterHandler, exception, logTarget, logLevel);

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