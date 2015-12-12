using System;
using System.Configuration;
using System.Runtime.CompilerServices;
using Common.Logging;
using System.Threading.Tasks;
using Common.Utils.Helpers;
using Common.Log.New.CommonLogging;

namespace Common.Log.New.Core
{
	public static class Log
	{
		private static IExinLog _core;
		public static IExinLog Core
		{
			get
			{
				if(_core == null)
				{
					LogManager.Adapter = LoggerFactoryAdapter;
					_core = LogManager.GetLogger<object>() as IExinLog;
					if(_core == null)
						throw new ConfigurationErrorsException("LogManager.GetLogger<object>() did not return an IExinLog instance. ");
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

		private static void PrepareForLogTarget(LogTarget logTarget)
		{
			if(logTarget != LogTarget.All)
				Core.ThreadVariablesContext.Set(VariableContextKeys.LogTarget, logTarget);
		}

		#region Extensions

		#region General log at level (for default methods)
		public static void LogAtLevel(Action<FormatMessageHandler> printMessageCallback, LogLevel logLevel, LogTarget logTarget = LogTarget.All)
		{
			if(!Core.IsLevelEnabled(logLevel))
				return;

			PrepareForLogTarget(logTarget);

			switch(logLevel)
			{
				case LogLevel.Trace:
					Trace(printMessageCallback);
					break;
				case LogLevel.Debug:
					Debug(printMessageCallback);
					break;
				case LogLevel.Info:
					Info(printMessageCallback);
					break;
				case LogLevel.Warn:
					Warn(printMessageCallback);
					break;
				case LogLevel.Error:
					Error(printMessageCallback);
					break;
				case LogLevel.Fatal:
					Fatal(printMessageCallback);
					break;
				default:
					throw new NotImplementedException(GetTag(typeof(Log)));
			}
		}
		#endregion

		#region General log at level (for our extensions)
		public static Exception LogAtLevel(object callerTypeInstance, Func<FormatMessageHandler, string> printMessageCallback, LogLevel logLevel,
										   Exception exception = null, LogTarget logTarget = LogTarget.All, [CallerMemberName] string callerFnName = null)
		{
			if(!Core.IsLevelEnabled(logLevel))
				return null;

			switch(logLevel)
			{
				case LogLevel.Trace:
					return DoLog(callerTypeInstance, printMessageCallback, exception, logTarget, callerFnName, Core.Trace, Core.Trace);
				case LogLevel.Debug:
					return DoLog(callerTypeInstance, printMessageCallback, exception, logTarget, callerFnName, Core.Debug, Core.Debug);
				case LogLevel.Info:
					return DoLog(callerTypeInstance, printMessageCallback, exception, logTarget, callerFnName, Core.Info, Core.Info);
				case LogLevel.Warn:
					return DoLog(callerTypeInstance, printMessageCallback, exception, logTarget, callerFnName, Core.Warn, Core.Warn);
				case LogLevel.Error:
					return DoLog(callerTypeInstance, printMessageCallback, exception, logTarget, callerFnName, Core.Error, Core.Error);
				case LogLevel.Fatal:
					return DoLog(callerTypeInstance, printMessageCallback, exception, logTarget, callerFnName, Core.Fatal, Core.Fatal);
				default:
					throw new NotImplementedException(GetTag(typeof(Log)));
			}
		}

		public static Exception LogAtLevel(Type callerType, Func<FormatMessageHandler, string> printMessageCallback, LogLevel logLevel,
										   Exception exception = null, LogTarget logTarget = LogTarget.All, [CallerMemberName] string callerFnName = null)
		{
			if(!Core.IsLevelEnabled(logLevel))
				return null;

			switch(logLevel)
			{
				case LogLevel.Trace:
					return DoLog(callerType, printMessageCallback, exception, logTarget, callerFnName, Core.Trace, Core.Trace);
				case LogLevel.Debug:
					return DoLog(callerType, printMessageCallback, exception, logTarget, callerFnName, Core.Debug, Core.Debug);
				case LogLevel.Info:
					return DoLog(callerType, printMessageCallback, exception, logTarget, callerFnName, Core.Info, Core.Info);
				case LogLevel.Warn:
					return DoLog(callerType, printMessageCallback, exception, logTarget, callerFnName, Core.Warn, Core.Warn);
				case LogLevel.Error:
					return DoLog(callerType, printMessageCallback, exception, logTarget, callerFnName, Core.Error, Core.Error);
				case LogLevel.Fatal:
					return DoLog(callerType, printMessageCallback, exception, logTarget, callerFnName, Core.Fatal, Core.Fatal);
				default:
					throw new NotImplementedException(GetTag(typeof(Log)));
			}
		}

		public static Exception LogAtLevel(object callerTypeInstance, LogLevel logLevel, Exception exception,
										LogTarget logTarget = LogTarget.All, [CallerMemberName] string callerFnName = null)
		{
			if(!Core.IsLevelEnabled(logLevel))
				return null;

			switch(logLevel)
			{
				case LogLevel.Trace:
					return DoLog(callerTypeInstance, m => m(string.Empty), exception, logTarget, callerFnName, Core.Trace, Core.Trace);
				case LogLevel.Debug:
					return DoLog(callerTypeInstance, m => m(string.Empty), exception, logTarget, callerFnName, Core.Debug, Core.Debug);
				case LogLevel.Info:
					return DoLog(callerTypeInstance, m => m(string.Empty), exception, logTarget, callerFnName, Core.Info, Core.Info);
				case LogLevel.Warn:
					return DoLog(callerTypeInstance, m => m(string.Empty), exception, logTarget, callerFnName, Core.Warn, Core.Warn);
				case LogLevel.Error:
					return DoLog(callerTypeInstance, m => m(string.Empty), exception, logTarget, callerFnName, Core.Error, Core.Error);
				case LogLevel.Fatal:
					return DoLog(callerTypeInstance, m => m(string.Empty), exception, logTarget, callerFnName, Core.Fatal, Core.Fatal);
				default:
					throw new NotImplementedException(GetTag(typeof(Log)));
			}
		}

		public static Exception LogAtLevel(Type callerType, LogLevel logLevel, Exception exception, LogTarget logTarget = LogTarget.All, [CallerMemberName] string callerFnName = null)
		{
			if(!Core.IsLevelEnabled(logLevel))
				return null;

			switch(logLevel)
			{
				case LogLevel.Trace:
					return DoLog(callerType, m => m(string.Empty), exception, logTarget, callerFnName, Core.Trace, Core.Trace);
				case LogLevel.Debug:
					return DoLog(callerType, m => m(string.Empty), exception, logTarget, callerFnName, Core.Debug, Core.Debug);
				case LogLevel.Info:
					return DoLog(callerType, m => m(string.Empty), exception, logTarget, callerFnName, Core.Info, Core.Info);
				case LogLevel.Warn:
					return DoLog(callerType, m => m(string.Empty), exception, logTarget, callerFnName, Core.Warn, Core.Warn);
				case LogLevel.Error:
					return DoLog(callerType, m => m(string.Empty), exception, logTarget, callerFnName, Core.Error, Core.Error);
				case LogLevel.Fatal:
					return DoLog(callerType, m => m(string.Empty), exception, logTarget, callerFnName, Core.Fatal, Core.Fatal);
				default:
					throw new NotImplementedException(GetTag(typeof(Log)));
			}
		}
		#endregion

		#region Concrete log at levels
		public static Exception Trace(object callerTypeInstance, Func<FormatMessageHandler, string> printMessageCallback, Exception exception = null,
									LogTarget logTarget = LogTarget.All, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsTraceEnabled ? null : DoLog(callerTypeInstance, printMessageCallback, exception, logTarget, callerFnName, Core.Trace, Core.Trace);
		}

		public static Exception Trace(Type callerType, Func<FormatMessageHandler, string> printMessageCallback, Exception exception = null,
									LogTarget logTarget = LogTarget.All, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsTraceEnabled ? null : DoLog(callerType, printMessageCallback, exception, logTarget, callerFnName, Core.Trace, Core.Trace);
		}

		public static Exception Trace(object callerTypeInstance, Exception exception, LogTarget logTarget = LogTarget.All, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsTraceEnabled ? null : DoLog(callerTypeInstance, m => m(string.Empty), exception, logTarget, callerFnName, Core.Trace, Core.Trace);
		}

		public static Exception Trace(Type callerType, Exception exception, LogTarget logTarget = LogTarget.All, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsTraceEnabled ? null : DoLog(callerType, m => m(string.Empty), exception, logTarget, callerFnName, Core.Trace, Core.Trace);
		}

		public static Exception Debug(object callerTypeInstance, Func<FormatMessageHandler, string> printMessageCallback, Exception exception = null,
									LogTarget logTarget = LogTarget.All, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsDebugEnabled ? null : DoLog(callerTypeInstance, printMessageCallback, exception, logTarget, callerFnName, Core.Debug, Core.Debug);
		}

		public static Exception Debug(Type callerType, Func<FormatMessageHandler, string> printMessageCallback, Exception exception = null,
									LogTarget logTarget = LogTarget.All, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsDebugEnabled ? null : DoLog(callerType, printMessageCallback, exception, logTarget, callerFnName, Core.Debug, Core.Debug);
		}

		public static Exception Debug(object callerTypeInstance, Exception exception, LogTarget logTarget = LogTarget.All, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsDebugEnabled ? null : DoLog(callerTypeInstance, m => m(string.Empty), exception, logTarget, callerFnName, Core.Debug, Core.Debug);
		}

		public static Exception Debug(Type callerType, Exception exception, LogTarget logTarget = LogTarget.All, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsDebugEnabled ? null : DoLog(callerType, m => m(string.Empty), exception, logTarget, callerFnName, Core.Debug, Core.Debug);
		}

		public static Exception Info(object callerTypeInstance, Func<FormatMessageHandler, string> printMessageCallback, Exception exception = null,
								LogTarget logTarget = LogTarget.All, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsInfoEnabled ? null : DoLog(callerTypeInstance, printMessageCallback, exception, logTarget, callerFnName, Core.Info, Core.Info);
		}

		public static Exception Info(Type callerType, Func<FormatMessageHandler, string> printMessageCallback, Exception exception = null,
								LogTarget logTarget = LogTarget.All, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsInfoEnabled ? null : DoLog(callerType, printMessageCallback, exception, logTarget, callerFnName, Core.Info, Core.Info);
		}

		public static Exception Info(object callerTypeInstance, Exception exception, LogTarget logTarget = LogTarget.All, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsInfoEnabled ? null : DoLog(callerTypeInstance, m => m(string.Empty), exception, logTarget, callerFnName, Core.Info, Core.Info);
		}

		public static Exception Info(Type callerType, Exception exception = null, LogTarget logTarget = LogTarget.All, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsInfoEnabled ? null : DoLog(callerType, m => m(string.Empty), exception, logTarget, callerFnName, Core.Info, Core.Info);
		}

		public static Exception Warn(object callerTypeInstance, Func<FormatMessageHandler, string> printMessageCallback, Exception exception = null,
								LogTarget logTarget = LogTarget.All, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsWarnEnabled ? null : DoLog(callerTypeInstance, printMessageCallback, exception, logTarget, callerFnName, Core.Warn, Core.Warn);
		}

		public static Exception Warn(Type callerType, Func<FormatMessageHandler, string> printMessageCallback, Exception exception = null,
								LogTarget logTarget = LogTarget.All, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsWarnEnabled ? null : DoLog(callerType, printMessageCallback, exception, logTarget, callerFnName, Core.Warn, Core.Warn);
		}

		public static Exception Warn(object callerTypeInstance, Exception exception, LogTarget logTarget = LogTarget.All, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsWarnEnabled ? null : DoLog(callerTypeInstance, m => m(string.Empty), exception, logTarget, callerFnName, Core.Warn, Core.Warn);
		}

		public static Exception Warn(Type callerType, Exception exception, LogTarget logTarget = LogTarget.All, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsWarnEnabled ? null : DoLog(callerType, m => m(string.Empty), exception, logTarget, callerFnName, Core.Warn, Core.Warn);
		}

		public static Exception Error(object callerTypeInstance, Func<FormatMessageHandler, string> printMessageCallback, Exception exception = null,
									LogTarget logTarget = LogTarget.All, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsErrorEnabled ? null : DoLog(callerTypeInstance, printMessageCallback, exception, logTarget, callerFnName, Core.Error, Core.Error);
		}

		public static Exception Error(Type callerType, Func<FormatMessageHandler, string> printMessageCallback, Exception exception = null,
									LogTarget logTarget = LogTarget.All, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsErrorEnabled ? null : DoLog(callerType, printMessageCallback, exception, logTarget, callerFnName, Core.Error, Core.Error);
		}

		public static Exception Error(object callerTypeInstance, Exception exception, LogTarget logTarget = LogTarget.All, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsErrorEnabled ? null : DoLog(callerTypeInstance, m => m(string.Empty), exception, logTarget, callerFnName, Core.Error, Core.Error);
		}

		public static Exception Error(Type callerType, Exception exception, LogTarget logTarget = LogTarget.All, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsErrorEnabled ? null : DoLog(callerType, m => m(string.Empty), exception, logTarget, callerFnName, Core.Error, Core.Error);
		}

		public static Exception Fatal(object callerTypeInstance, Func<FormatMessageHandler, string> printMessageCallback, Exception exception = null,
									LogTarget logTarget = LogTarget.All, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsFatalEnabled ? null : DoLog(callerTypeInstance, printMessageCallback, exception, logTarget, callerFnName, Core.Fatal, Core.Fatal);
		}

		public static Exception Fatal(Type callerType, Func<FormatMessageHandler, string> printMessageCallback, Exception exception = null,
									LogTarget logTarget = LogTarget.All, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsFatalEnabled ? null : DoLog(callerType, printMessageCallback, exception, logTarget, callerFnName, Core.Fatal, Core.Fatal);
		}

		public static Exception Fatal(object callerTypeInstance, Exception exception, LogTarget logTarget = LogTarget.All, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsFatalEnabled ? null : DoLog(callerTypeInstance, m => m(string.Empty), exception, logTarget, callerFnName, Core.Fatal, Core.Fatal);
		}

		public static Exception Fatal(Type callerType, Exception exception = null, LogTarget logTarget = LogTarget.All, [CallerMemberName] string callerFnName = null)
		{
			return !Core.IsFatalEnabled ? null : DoLog(callerType, m => m(string.Empty), exception, logTarget, callerFnName, Core.Fatal, Core.Fatal);
		}
		#endregion

		#region DoLog
		private static Exception DoLog(
			object callerTypeInstance,
			Func<FormatMessageHandler, string> printMessageCallback,
			Exception exception,
			LogTarget logTarget,
			string callerFnName,
			Action<Action<FormatMessageHandler>> exceptionlessHandler,
			Action<Action<FormatMessageHandler>, Exception> exceptionHandler
			)
		{
			return DoLog(GetTag(callerTypeInstance, callerFnName), printMessageCallback, exception, logTarget, callerFnName, exceptionlessHandler,
						 exceptionHandler);
		}

		private static Exception DoLog(
			Type callerType,
			Func<FormatMessageHandler, string> printMessageCallback,
			Exception exception,
			LogTarget logTarget,
			string callerFnName,
			Action<Action<FormatMessageHandler>> exceptionlessHandler,
			Action<Action<FormatMessageHandler>, Exception> exceptionHandler
			)
		{
			return DoLog(GetTag(callerType, callerFnName), printMessageCallback, exception, logTarget, callerFnName, exceptionlessHandler, exceptionHandler);
		}

		private static Exception DoLog(
			string tag,
			Func<FormatMessageHandler, string> printMessageCallback,
			Exception exception,
			LogTarget logTarget,
			string callerFnName,
			Action<Action<FormatMessageHandler>> exceptionlessHandler,
			Action<Action<FormatMessageHandler>, Exception> exceptionHandler
			)
		{
			PrepareForLogTarget(logTarget);

			if(exception == null)
			{
				exceptionlessHandler(m => m("{0}{1}", tag, printMessageCallback(string.Format)));
			}
			else
			{
				if(string.IsNullOrEmpty(exception.StackTrace))
				{
					var stackTrace = "{0}StackTrace:{0}{1}".Formatted(Environment.NewLine, Environment.StackTrace);
					exceptionHandler(m => m("{0}{1}{2}", tag, printMessageCallback(string.Format), stackTrace), exception);
				}
				else
				{
					exceptionHandler(m => m("{0}{1}", tag, printMessageCallback(string.Format)), exception);
				}
			}

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

		#region ILog delegated members
		public static void Trace(object message)
		{
			Core.Trace(message);
		}

		public static Exception Trace(object message, Exception exception)
		{
			Core.Trace(message, exception);
			return exception;
		}

		public static void TraceFormat(string format, params object[] args)
		{
			Core.TraceFormat(format, args);
		}

		public static Exception TraceFormat(string format, Exception exception, params object[] args)
		{
			Core.TraceFormat(format, exception, args);
			return exception;
		}

		public static void TraceFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			Core.TraceFormat(formatProvider, format, args);
		}

		public static Exception TraceFormat(IFormatProvider formatProvider, string format, Exception exception, params object[] args)
		{
			Core.TraceFormat(formatProvider, format, exception, args);
			return exception;
		}

		public static void Trace(Action<FormatMessageHandler> formatMessageCallback)
		{
			Core.Trace(formatMessageCallback);
		}

		public static Exception Trace(Action<FormatMessageHandler> formatMessageCallback, Exception exception)
		{
			Core.Trace(formatMessageCallback, exception);
			return exception;
		}

		public static void Trace(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback)
		{
			Core.Trace(formatProvider, formatMessageCallback);
		}

		public static Exception Trace(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback, Exception exception)
		{
			Core.Trace(formatProvider, formatMessageCallback, exception);
			return exception;
		}

		public static void Debug(object message)
		{
			Core.Debug(message);
		}

		public static Exception Debug(object message, Exception exception)
		{
			Core.Debug(message, exception);
			return exception;
		}

		public static void DebugFormat(string format, params object[] args)
		{
			Core.DebugFormat(format, args);
		}

		public static Exception DebugFormat(string format, Exception exception, params object[] args)
		{
			Core.DebugFormat(format, exception, args);
			return exception;
		}

		public static void DebugFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			Core.DebugFormat(formatProvider, format, args);
		}

		public static Exception DebugFormat(IFormatProvider formatProvider, string format, Exception exception, params object[] args)
		{
			Core.DebugFormat(formatProvider, format, exception, args);
			return exception;
		}

		public static void Debug(Action<FormatMessageHandler> formatMessageCallback)
		{
			Core.Debug(formatMessageCallback);
		}

		public static Exception Debug(Action<FormatMessageHandler> formatMessageCallback, Exception exception)
		{
			Core.Debug(formatMessageCallback, exception);
			return exception;
		}

		public static void Debug(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback)
		{
			Core.Debug(formatProvider, formatMessageCallback);
		}

		public static Exception Debug(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback, Exception exception)
		{
			Core.Debug(formatProvider, formatMessageCallback, exception);
			return exception;
		}

		public static void Info(object message)
		{
			Core.Info(message);
		}

		public static Exception Info(object message, Exception exception)
		{
			Core.Info(message, exception);
			return exception;
		}

		public static void InfoFormat(string format, params object[] args)
		{
			Core.InfoFormat(format, args);
		}

		public static Exception InfoFormat(string format, Exception exception, params object[] args)
		{
			Core.InfoFormat(format, exception, args);
			return exception;
		}

		public static void InfoFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			Core.InfoFormat(formatProvider, format, args);
		}

		public static Exception InfoFormat(IFormatProvider formatProvider, string format, Exception exception, params object[] args)
		{
			Core.InfoFormat(formatProvider, format, exception, args);
			return exception;
		}

		public static void Info(Action<FormatMessageHandler> formatMessageCallback)
		{
			Core.Info(formatMessageCallback);
		}

		public static Exception Info(Action<FormatMessageHandler> formatMessageCallback, Exception exception)
		{
			Core.Info(formatMessageCallback, exception);
			return exception;
		}

		public static void Info(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback)
		{
			Core.Info(formatProvider, formatMessageCallback);
		}

		public static Exception Info(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback, Exception exception)
		{
			Core.Info(formatProvider, formatMessageCallback, exception);
			return exception;
		}

		public static void Warn(object message)
		{
			Core.Warn(message);
		}

		public static Exception Warn(object message, Exception exception)
		{
			Core.Warn(message, exception);
			return exception;
		}

		public static void WarnFormat(string format, params object[] args)
		{
			Core.WarnFormat(format, args);
		}

		public static Exception WarnFormat(string format, Exception exception, params object[] args)
		{
			Core.WarnFormat(format, exception, args);
			return exception;
		}

		public static void WarnFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			Core.WarnFormat(formatProvider, format, args);
		}

		public static Exception WarnFormat(IFormatProvider formatProvider, string format, Exception exception, params object[] args)
		{
			Core.WarnFormat(formatProvider, format, exception, args);
			return exception;
		}

		public static void Warn(Action<FormatMessageHandler> formatMessageCallback)
		{
			Core.Warn(formatMessageCallback);
		}

		public static Exception Warn(Action<FormatMessageHandler> formatMessageCallback, Exception exception)
		{
			Core.Warn(formatMessageCallback, exception);
			return exception;
		}

		public static void Warn(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback)
		{
			Core.Warn(formatProvider, formatMessageCallback);
		}

		public static Exception Warn(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback, Exception exception)
		{
			Core.Warn(formatProvider, formatMessageCallback, exception);
			return exception;
		}

		public static void Error(object message)
		{
			Core.Error(message);
		}

		public static Exception Error(object message, Exception exception)
		{
			Core.Error(message, exception);
			return exception;
		}

		public static void ErrorFormat(string format, params object[] args)
		{
			Core.ErrorFormat(format, args);
		}

		public static Exception ErrorFormat(string format, Exception exception, params object[] args)
		{
			Core.ErrorFormat(format, exception, args);
			return exception;
		}

		public static void ErrorFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			Core.ErrorFormat(formatProvider, format, args);
		}

		public static Exception ErrorFormat(IFormatProvider formatProvider, string format, Exception exception, params object[] args)
		{
			Core.ErrorFormat(formatProvider, format, exception, args);
			return exception;
		}

		public static void Error(Action<FormatMessageHandler> formatMessageCallback)
		{
			Core.Error(formatMessageCallback);
		}

		public static Exception Error(Action<FormatMessageHandler> formatMessageCallback, Exception exception)
		{
			Core.Error(formatMessageCallback, exception);
			return exception;
		}

		public static void Error(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback)
		{
			Core.Error(formatProvider, formatMessageCallback);
		}

		public static Exception Error(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback, Exception exception)
		{
			Core.Error(formatProvider, formatMessageCallback, exception);
			return exception;
		}

		public static void Fatal(object message)
		{
			Core.Fatal(message);
		}

		public static Exception Fatal(object message, Exception exception)
		{
			Core.Fatal(message, exception);
			return exception;
		}

		public static void FatalFormat(string format, params object[] args)
		{
			Core.FatalFormat(format, args);
		}

		public static Exception FatalFormat(string format, Exception exception, params object[] args)
		{
			Core.FatalFormat(format, exception, args);
			return exception;
		}

		public static void FatalFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			Core.FatalFormat(formatProvider, format, args);
		}

		public static Exception FatalFormat(IFormatProvider formatProvider, string format, Exception exception, params object[] args)
		{
			Core.FatalFormat(formatProvider, format, exception, args);
			return exception;
		}

		public static void Fatal(Action<FormatMessageHandler> formatMessageCallback)
		{
			Core.Fatal(formatMessageCallback);
		}

		public static Exception Fatal(Action<FormatMessageHandler> formatMessageCallback, Exception exception)
		{
			Core.Fatal(formatMessageCallback, exception);
			return exception;
		}

		public static void Fatal(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback)
		{
			Core.Fatal(formatProvider, formatMessageCallback);
		}

		public static Exception Fatal(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback, Exception exception)
		{
			Core.Fatal(formatProvider, formatMessageCallback, exception);
			return exception;
		}

		public static bool IsTraceEnabled => Core.IsTraceEnabled;
		public static bool IsDebugEnabled => Core.IsDebugEnabled;
		public static bool IsErrorEnabled => Core.IsErrorEnabled;
		public static bool IsFatalEnabled => Core.IsFatalEnabled;
		public static bool IsInfoEnabled => Core.IsInfoEnabled;
		public static bool IsWarnEnabled => Core.IsWarnEnabled;
		public static IVariablesContext GlobalVariablesContext => Core.GlobalVariablesContext;
		public static IVariablesContext ThreadVariablesContext => Core.ThreadVariablesContext;

		#endregion
	}
}