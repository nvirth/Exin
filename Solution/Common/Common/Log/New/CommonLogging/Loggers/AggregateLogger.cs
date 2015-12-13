using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using Common.Configuration;
using Common.Logging;
using Common.Utils.Helpers;
using Common.Log.New.CommonLogging.Loggers.Base;
using Common.Log.New.Core;
using FormatMessageHandler = Common.Logging.FormatMessageHandler;

namespace Common.Log.New.CommonLogging
{
	public struct LoggerInstancesArgs
	{
		public IList<IExinLog> UiLoggers { get; set; }
		public IList<IExinLog> LogLoggers { get; set; }
	}

	public class AggregateLogger : AbstractSimpleLoggerBase
	{
		public IList<IExinLog> UiLoggers { get; private set; }
		public IList<IExinLog> LogLoggers { get; private set; }

		public AggregateLogger(string logName, LogLevel logLevel, bool showLevel, bool showDateTime, bool showLogName, string dateTimeFormat, LoggerInstancesArgs instances)
			: base(logName, logLevel, showLevel, showDateTime, showLogName, dateTimeFormat)
		{
			UiLoggers = instances.UiLoggers;
			LogLoggers = instances.LogLoggers;
		}

		protected override object CreateMessageFormatter(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback)
		{
			var result = new LocalizedCallbackMessageFormatter(formatProvider, formatMessageCallback);
			return result;
		}

		protected override object CreateMessageFormatter(IFormatProvider formatProvider, string format, params object[] args)
		{
			// TODO
			throw new NotSupportedException("Non callback based logging is not supported; because the Localization section is not written for it. ");
		}

		protected override void WriteInternal(LogLevel level, object message, Exception e)
		{
			var logToLog = true; // If LogTarget is not set in VariablesContext, we probably used a default
			var logToUi = true;  // ILog method; so by default do log into both target
			var logTarget = ThreadVariablesContext.Get(VariableContextKeys.LogTarget) as LogTarget?;
			if(logTarget.HasValue)
			{
				logToLog = (logTarget.Value & LogTarget.Log) > 0;
				logToUi = (logTarget.Value & LogTarget.Ui) > 0;
			}

			// TODO soften cast after log is finished
			var messageFormatter = (LocalizedCallbackMessageFormatter)message;
			if(logToLog)
			{
				messageFormatter.CultureInfo = Cultures.LogCulture;
				LogLoggers.ForEach(logger => logger.Write(level, message, e));
			}
			if(logToUi)
			{
				messageFormatter.CultureInfo = Cultures.CurrentCulture;
				UiLoggers.ForEach(logger => logger.Write(level, message, e));
			}

			CleanupVariableContext();
		}


		//private bool LogToUi => (bool)(ThreadVariablesContext.Get(VariableContextKeys.LogToUi) ?? false);
		//private bool LogToLog => (bool)(ThreadVariablesContext.Get(VariableContextKeys.LogToLog) ?? false);

		private void CleanupVariableContext()
		{
			// Note: As long as we only put LogToUi and LogToLog into it, this is the faster solution.
			// Once we start to put in other stuff, it has to be replaced with key based removement!
			ThreadVariablesContext.Clear();
		}

		#region New logging API

		public void DoLog(LogData logData)
		{
			if(!IsLevelEnabled(logData.LogLevel))
				return;

			if(logData.LogToLog)
				LogLoggers.ForEach(logger => logger.Write(logData.LogLevel, logData.GetMessageForLog(), logData.Exception));
			if(logData.LogToUi)
				UiLoggers.ForEach(logger => logger.Write(logData.LogLevel, logData.GetMessageForUi(), logData.Exception));
		}

		#endregion

		#region Not supported

		private const string NotSupportedExceptionMsg = "Logging with the default Common.Logging API is not supported. See supported methods in IExinLog. ";

		protected override WriteHandler GetWriteHandler()
		{
			return (level, message, exception) => { throw new NotSupportedException(NotSupportedExceptionMsg); };
		}

		public override void Trace(object message)
		{
			throw new NotSupportedException(NotSupportedExceptionMsg);
		}

		public override void Trace(object message, Exception exception)
		{
			throw new NotSupportedException(NotSupportedExceptionMsg);
		}

		public override void TraceFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			throw new NotSupportedException(NotSupportedExceptionMsg);
		}

		public override void TraceFormat(IFormatProvider formatProvider, string format, Exception exception, params object[] args)
		{
			throw new NotSupportedException(NotSupportedExceptionMsg);
		}

		public override void TraceFormat(string format, params object[] args)
		{
			throw new NotSupportedException(NotSupportedExceptionMsg);
		}

		public override void TraceFormat(string format, Exception exception, params object[] args)
		{
			throw new NotSupportedException(NotSupportedExceptionMsg);
		}

		public override void Trace(Action<FormatMessageHandler> formatMessageCallback)
		{
			throw new NotSupportedException(NotSupportedExceptionMsg);
		}

		public override void Trace(Action<FormatMessageHandler> formatMessageCallback, Exception exception)
		{
			throw new NotSupportedException(NotSupportedExceptionMsg);
		}

		public override void Trace(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback)
		{
			throw new NotSupportedException(NotSupportedExceptionMsg);
		}

		public override void Trace(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback, Exception exception)
		{
			throw new NotSupportedException(NotSupportedExceptionMsg);
		}

		public override void Debug(object message)
		{
			throw new NotSupportedException(NotSupportedExceptionMsg);
		}

		public override void Debug(object message, Exception exception)
		{
			throw new NotSupportedException(NotSupportedExceptionMsg);
		}

		public override void DebugFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			throw new NotSupportedException(NotSupportedExceptionMsg);
		}

		public override void DebugFormat(IFormatProvider formatProvider, string format, Exception exception, params object[] args)
		{
			throw new NotSupportedException(NotSupportedExceptionMsg);
		}

		public override void DebugFormat(string format, params object[] args)
		{
			throw new NotSupportedException(NotSupportedExceptionMsg);
		}

		public override void DebugFormat(string format, Exception exception, params object[] args)
		{
			throw new NotSupportedException(NotSupportedExceptionMsg);
		}

		public override void Debug(Action<FormatMessageHandler> formatMessageCallback)
		{
			throw new NotSupportedException(NotSupportedExceptionMsg);
		}

		public override void Debug(Action<FormatMessageHandler> formatMessageCallback, Exception exception)
		{
			throw new NotSupportedException(NotSupportedExceptionMsg);
		}

		public override void Debug(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback)
		{
			throw new NotSupportedException(NotSupportedExceptionMsg);
		}

		public override void Debug(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback, Exception exception)
		{
			throw new NotSupportedException(NotSupportedExceptionMsg);
		}

		public override void Info(object message)
		{
			throw new NotSupportedException(NotSupportedExceptionMsg);
		}

		public override void Info(object message, Exception exception)
		{
			throw new NotSupportedException(NotSupportedExceptionMsg);
		}

		public override void InfoFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			throw new NotSupportedException(NotSupportedExceptionMsg);
		}

		public override void InfoFormat(IFormatProvider formatProvider, string format, Exception exception, params object[] args)
		{
			throw new NotSupportedException(NotSupportedExceptionMsg);
		}

		public override void InfoFormat(string format, params object[] args)
		{
			throw new NotSupportedException(NotSupportedExceptionMsg);
		}

		public override void InfoFormat(string format, Exception exception, params object[] args)
		{
			throw new NotSupportedException(NotSupportedExceptionMsg);
		}

		public override void Info(Action<FormatMessageHandler> formatMessageCallback)
		{
			throw new NotSupportedException(NotSupportedExceptionMsg);
		}

		public override void Info(Action<FormatMessageHandler> formatMessageCallback, Exception exception)
		{
			throw new NotSupportedException(NotSupportedExceptionMsg);
		}

		public override void Info(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback)
		{
			throw new NotSupportedException(NotSupportedExceptionMsg);
		}

		public override void Info(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback, Exception exception)
		{
			throw new NotSupportedException(NotSupportedExceptionMsg);
		}

		public override void Warn(object message)
		{
			throw new NotSupportedException(NotSupportedExceptionMsg);
		}

		public override void Warn(object message, Exception exception)
		{
			throw new NotSupportedException(NotSupportedExceptionMsg);
		}

		public override void WarnFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			throw new NotSupportedException(NotSupportedExceptionMsg);
		}

		public override void WarnFormat(IFormatProvider formatProvider, string format, Exception exception, params object[] args)
		{
			throw new NotSupportedException(NotSupportedExceptionMsg);
		}

		public override void WarnFormat(string format, params object[] args)
		{
			throw new NotSupportedException(NotSupportedExceptionMsg);
		}

		public override void WarnFormat(string format, Exception exception, params object[] args)
		{
			throw new NotSupportedException(NotSupportedExceptionMsg);
		}

		public override void Warn(Action<FormatMessageHandler> formatMessageCallback)
		{
			throw new NotSupportedException(NotSupportedExceptionMsg);
		}

		public override void Warn(Action<FormatMessageHandler> formatMessageCallback, Exception exception)
		{
			throw new NotSupportedException(NotSupportedExceptionMsg);
		}

		public override void Warn(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback)
		{
			throw new NotSupportedException(NotSupportedExceptionMsg);
		}

		public override void Warn(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback, Exception exception)
		{
			throw new NotSupportedException(NotSupportedExceptionMsg);
		}

		public override void Error(object message)
		{
			throw new NotSupportedException(NotSupportedExceptionMsg);
		}

		public override void Error(object message, Exception exception)
		{
			throw new NotSupportedException(NotSupportedExceptionMsg);
		}

		public override void ErrorFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			throw new NotSupportedException(NotSupportedExceptionMsg);
		}

		public override void ErrorFormat(IFormatProvider formatProvider, string format, Exception exception, params object[] args)
		{
			throw new NotSupportedException(NotSupportedExceptionMsg);
		}

		public override void ErrorFormat(string format, params object[] args)
		{
			throw new NotSupportedException(NotSupportedExceptionMsg);
		}

		public override void ErrorFormat(string format, Exception exception, params object[] args)
		{
			throw new NotSupportedException(NotSupportedExceptionMsg);
		}

		public override void Error(Action<FormatMessageHandler> formatMessageCallback)
		{
			throw new NotSupportedException(NotSupportedExceptionMsg);
		}

		public override void Error(Action<FormatMessageHandler> formatMessageCallback, Exception exception)
		{
			throw new NotSupportedException(NotSupportedExceptionMsg);
		}

		public override void Error(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback)
		{
			throw new NotSupportedException(NotSupportedExceptionMsg);
		}

		public override void Error(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback, Exception exception)
		{
			throw new NotSupportedException(NotSupportedExceptionMsg);
		}

		public override void Fatal(object message)
		{
			throw new NotSupportedException(NotSupportedExceptionMsg);
		}

		public override void Fatal(object message, Exception exception)
		{
			throw new NotSupportedException(NotSupportedExceptionMsg);
		}

		public override void FatalFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			throw new NotSupportedException(NotSupportedExceptionMsg);
		}

		public override void FatalFormat(IFormatProvider formatProvider, string format, Exception exception, params object[] args)
		{
			throw new NotSupportedException(NotSupportedExceptionMsg);
		}

		public override void FatalFormat(string format, params object[] args)
		{
			throw new NotSupportedException(NotSupportedExceptionMsg);
		}

		public override void FatalFormat(string format, Exception exception, params object[] args)
		{
			throw new NotSupportedException(NotSupportedExceptionMsg);
		}

		public override void Fatal(Action<FormatMessageHandler> formatMessageCallback)
		{
			throw new NotSupportedException(NotSupportedExceptionMsg);
		}

		public override void Fatal(Action<FormatMessageHandler> formatMessageCallback, Exception exception)
		{
			throw new NotSupportedException(NotSupportedExceptionMsg);
		}

		public override void Fatal(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback)
		{
			throw new NotSupportedException(NotSupportedExceptionMsg);
		}

		public override void Fatal(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback, Exception exception)
		{
			throw new NotSupportedException(NotSupportedExceptionMsg);
		}

		#endregion
	}

	// TODO separate file
	public class LocalizedCallbackMessageFormatter
	{
		public CultureInfo CultureInfo { get; set; }

		private readonly Dictionary<CultureInfo, string> _cache = new Dictionary<CultureInfo, string>();
		private string CachedMessage
		{
			get { return _cache.ContainsKey(CultureInfo) ? _cache[CultureInfo] : null; }
			set { _cache[CultureInfo] = value; }
		}

		// --

		private readonly Action<FormatMessageHandler> _formatMessageCallback;
		private readonly IFormatProvider _formatProvider;

		public LocalizedCallbackMessageFormatter(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback)
		{
			_formatProvider = formatProvider;
			_formatMessageCallback = formatMessageCallback;
			CultureInfo = CultureInfo.InvariantCulture;
		}

		private string FormatMessage(string format, params object[] args)
		{
			if(args?.Length > 1)
			{
				var formatMessageHandler = args[1] as Func<FormatMessageHandler, string>;
				if(formatMessageHandler != null)
				{
					var innerMessage = formatMessageHandler(FormatInnerMessage);
					args[1] = innerMessage;
				}
			}

			CachedMessage = string.Format(_formatProvider, format, args);
			return CachedMessage;
		}

		private string FormatInnerMessage(string format, params object[] args)
		{
			if(args?.Length > 0)
			{
				var resourceManager = args[0] as ResourceManager; // TODO rather at end
				if(resourceManager != null)
				{
					var resourceKey = format;
					string newMessageFormat;
					try {
						newMessageFormat = resourceManager.GetString(resourceKey, CultureInfo);
					}
					catch(Exception e) {
						newMessageFormat = "!!! MISSING RESOURCE KEY: {0}/{1} !!!".Formatted(resourceManager.BaseName, resourceKey);
					}

					format = newMessageFormat ?? "!!! MISSING RESOURCE KEY: {0}/{1} !!!".Formatted(resourceManager.BaseName, resourceKey);
					args = args.Skip(1).ToArray();
				}
			}

			var innerMessage = string.Format(_formatProvider, format, args);
			return innerMessage;
		}

		public override string ToString()
		{
			if((CachedMessage == null) && (_formatMessageCallback != null))
			{
				_formatMessageCallback(FormatMessage);
			}
			return CachedMessage;
		}
	}
}