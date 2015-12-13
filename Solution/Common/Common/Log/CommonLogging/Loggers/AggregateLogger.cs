using System;
using System.Collections.Generic;
using Common.Log.CommonLogging.Loggers.Base;
using Common.Log.Core;
using Common.Logging;
using Common.Logging.Factory;
using Common.Utils.Helpers;
using FormatMessageHandler = Common.Logging.FormatMessageHandler;

namespace Common.Log.CommonLogging.Loggers
{
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

		public void DoLog(LogData logData)
		{
			if(!IsLevelEnabled(logData.LogLevel))
				return;

			if(logData.LogToLog)
				LogLoggers.ForEach(logger => logger.Write(logData.LogLevel, logData.GetMessageForLog(), logData.Exception));
			if(logData.LogToUi)
				UiLoggers.ForEach(logger => logger.Write(logData.LogLevel, logData.GetMessageForUi(), logData.Exception));
		}

		#region Not supported

		private const string NotSupportedExceptionMsg = "Logging with the default Common.Logging API is not supported. See supported methods in IExinLog. ";

		protected override void WriteInternal(LogLevel level, object message, Exception e)
		{
			throw new NotSupportedException(NotSupportedExceptionMsg);
		}

		protected override object CreateMessageFormatter(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback)
		{
			throw new NotSupportedException(NotSupportedExceptionMsg);
		}

		protected override object CreateMessageFormatter(IFormatProvider formatProvider, string format, params object[] args)
		{
			throw new NotSupportedException(NotSupportedExceptionMsg);
		}

		protected override AbstractLogger.WriteHandler GetWriteHandler()
		{
			return (level, message, exception) => { throw new NotSupportedException(NotSupportedExceptionMsg); };
		}

		// --

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
}