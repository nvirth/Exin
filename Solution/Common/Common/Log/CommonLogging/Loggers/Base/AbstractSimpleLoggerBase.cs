using System;
using System.Threading;
using Common.Log.Core;
using Common.Logging;
using Common.Logging.Factory;
using Common.Logging.Simple;
using FormatMessageHandler = Common.Logging.FormatMessageHandler;

namespace Common.Log.CommonLogging.Loggers.Base
{
	public abstract class AbstractSimpleLoggerBase : AbstractSimpleLogger, IExinLog
	{
		public AbstractSimpleLoggerBase(string logName, LogLevel logLevel, bool showlevel, bool showDateTime, bool showLogName, string dateTimeFormat) 
			: base(logName, logLevel, showlevel, showDateTime, showLogName, dateTimeFormat)
		{
		}

		#region IExinLog

		public new bool IsLevelEnabled(LogLevel level)
		{
			return base.IsLevelEnabled(level);
		}

		public void Write(LogLevel logLevel, object message, Exception exception)
		{
			WriteInternal(logLevel, message, exception);
		}

		#endregion


		#region VariablesContext

		private readonly ThreadLocal<IVariablesContext> _threadVariablesContext = new ThreadLocal<IVariablesContext>(() => new DictionaryVariablesContext());
		public override IVariablesContext ThreadVariablesContext => _threadVariablesContext.Value;

		private IVariablesContext _globalVariablesContext;
		public override IVariablesContext GlobalVariablesContext => _globalVariablesContext ?? (_globalVariablesContext = new DictionaryVariablesContext());

		#endregion

		#region MessageFormatter

		/// <returns>Here we create a message formatter object. It's ToString method will be called!</returns>
		protected virtual object CreateMessageFormatter(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback)
		{
			var result = new FormatMessageCallbackFormattedMessage(formatProvider, formatMessageCallback);
			return result;
		}

		/// <returns>Here we create a message formatter object. It's ToString method will be called!</returns>
		protected virtual object CreateMessageFormatter(IFormatProvider formatProvider, string format, params object[] args)
		{
			var result = new StringFormatFormattedMessage(formatProvider, format, args);
			return result;
		}

		#endregion

		#region Overriden logging methods

		public override void Trace(object message)
		{
			if(IsTraceEnabled)
				this.Write(LogLevel.Trace, message, null);
		}

		public override void Trace(object message, Exception exception)
		{
			if(IsTraceEnabled)
				this.Write(LogLevel.Trace, message, exception);
		}

		[StringFormatMethod("format")]
		public override void TraceFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			if(IsTraceEnabled)
				this.Write(LogLevel.Trace, CreateMessageFormatter(formatProvider, format, args), null);
		}

		[StringFormatMethod("format")]
		public override void TraceFormat(IFormatProvider formatProvider, string format, Exception exception, params object[] args)
		{
			if(IsTraceEnabled)
				this.Write(LogLevel.Trace, CreateMessageFormatter(formatProvider, format, args), exception);
		}

		[StringFormatMethod("format")]
		public override void TraceFormat(string format, params object[] args)
		{
			if(IsTraceEnabled)
				this.Write(LogLevel.Trace, CreateMessageFormatter(null, format, args), null);
		}

		[StringFormatMethod("format")]
		public override void TraceFormat(string format, Exception exception, params object[] args)
		{
			if(IsTraceEnabled)
				this.Write(LogLevel.Trace, CreateMessageFormatter(null, format, args), exception);
		}

		public override void Trace(Action<FormatMessageHandler> formatMessageCallback)
		{
			if(IsTraceEnabled)
				this.Write(LogLevel.Trace, CreateMessageFormatter(null, formatMessageCallback), null);
		}

		public override void Trace(Action<FormatMessageHandler> formatMessageCallback, Exception exception)
		{
			if(IsTraceEnabled)
				this.Write(LogLevel.Trace, CreateMessageFormatter(null, formatMessageCallback), exception);
		}

		public override void Trace(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback)
		{
			if(IsTraceEnabled)
				this.Write(LogLevel.Trace, CreateMessageFormatter(formatProvider, formatMessageCallback), null);
		}

		public override void Trace(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback, Exception exception)
		{
			if(IsTraceEnabled)
				this.Write(LogLevel.Trace, CreateMessageFormatter(formatProvider, formatMessageCallback), exception);
		}

		public override void Debug(object message)
		{
			if(IsDebugEnabled)
				this.Write(LogLevel.Debug, message, null);
		}

		public override void Debug(object message, Exception exception)
		{
			if(IsDebugEnabled)
				this.Write(LogLevel.Debug, message, exception);
		}

		[StringFormatMethod("format")]
		public override void DebugFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			if(IsDebugEnabled)
				this.Write(LogLevel.Debug, CreateMessageFormatter(formatProvider, format, args), null);
		}

		[StringFormatMethod("format")]
		public override void DebugFormat(IFormatProvider formatProvider, string format, Exception exception, params object[] args)
		{
			if(IsDebugEnabled)
				this.Write(LogLevel.Debug, CreateMessageFormatter(formatProvider, format, args), exception);
		}

		[StringFormatMethod("format")]
		public override void DebugFormat(string format, params object[] args)
		{
			if(IsDebugEnabled)
				this.Write(LogLevel.Debug, CreateMessageFormatter(null, format, args), null);
		}

		[StringFormatMethod("format")]
		public override void DebugFormat(string format, Exception exception, params object[] args)
		{
			if(IsDebugEnabled)
				this.Write(LogLevel.Debug, CreateMessageFormatter(null, format, args), exception);
		}

		public override void Debug(Action<FormatMessageHandler> formatMessageCallback)
		{
			if(IsDebugEnabled)
				this.Write(LogLevel.Debug, CreateMessageFormatter(null, formatMessageCallback), null);
		}

		public override void Debug(Action<FormatMessageHandler> formatMessageCallback, Exception exception)
		{
			if(IsDebugEnabled)
				this.Write(LogLevel.Debug, CreateMessageFormatter(null, formatMessageCallback), exception);
		}

		public override void Debug(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback)
		{
			if(IsDebugEnabled)
				this.Write(LogLevel.Debug, CreateMessageFormatter(formatProvider, formatMessageCallback), null);
		}

		public override void Debug(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback, Exception exception)
		{
			if(IsDebugEnabled)
				this.Write(LogLevel.Debug, CreateMessageFormatter(formatProvider, formatMessageCallback), exception);
		}

		public override void Info(object message)
		{
			if(IsInfoEnabled)
				this.Write(LogLevel.Info, message, null);
		}

		public override void Info(object message, Exception exception)
		{
			if(IsInfoEnabled)
				this.Write(LogLevel.Info, message, exception);
		}

		[StringFormatMethod("format")]
		public override void InfoFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			if(IsInfoEnabled)
				this.Write(LogLevel.Info, CreateMessageFormatter(formatProvider, format, args), null);
		}

		[StringFormatMethod("format")]
		public override void InfoFormat(IFormatProvider formatProvider, string format, Exception exception, params object[] args)
		{
			if(IsInfoEnabled)
				this.Write(LogLevel.Info, CreateMessageFormatter(formatProvider, format, args), exception);
		}

		[StringFormatMethod("format")]
		public override void InfoFormat(string format, params object[] args)
		{
			if(IsInfoEnabled)
				this.Write(LogLevel.Info, CreateMessageFormatter(null, format, args), null);
		}

		[StringFormatMethod("format")]
		public override void InfoFormat(string format, Exception exception, params object[] args)
		{
			if(IsInfoEnabled)
				this.Write(LogLevel.Info, CreateMessageFormatter(null, format, args), exception);
		}

		public override void Info(Action<FormatMessageHandler> formatMessageCallback)
		{
			if(IsInfoEnabled)
				this.Write(LogLevel.Info, CreateMessageFormatter(null, formatMessageCallback), null);
		}

		public override void Info(Action<FormatMessageHandler> formatMessageCallback, Exception exception)
		{
			if(IsInfoEnabled)
				this.Write(LogLevel.Info, CreateMessageFormatter(null, formatMessageCallback), exception);
		}

		public override void Info(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback)
		{
			if(IsInfoEnabled)
				this.Write(LogLevel.Info, CreateMessageFormatter(formatProvider, formatMessageCallback), null);
		}

		public override void Info(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback, Exception exception)
		{
			if(IsInfoEnabled)
				this.Write(LogLevel.Info, CreateMessageFormatter(formatProvider, formatMessageCallback), exception);
		}

		public override void Warn(object message)
		{
			if(IsWarnEnabled)
				this.Write(LogLevel.Warn, message, null);
		}

		public override void Warn(object message, Exception exception)
		{
			if(IsWarnEnabled)
				this.Write(LogLevel.Warn, message, exception);
		}

		[StringFormatMethod("format")]
		public override void WarnFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			if(IsWarnEnabled)
				this.Write(LogLevel.Warn, CreateMessageFormatter(formatProvider, format, args), null);
		}

		[StringFormatMethod("format")]
		public override void WarnFormat(IFormatProvider formatProvider, string format, Exception exception, params object[] args)
		{
			if(IsWarnEnabled)
				this.Write(LogLevel.Warn, CreateMessageFormatter(formatProvider, format, args), exception);
		}

		[StringFormatMethod("format")]
		public override void WarnFormat(string format, params object[] args)
		{
			if(IsWarnEnabled)
				this.Write(LogLevel.Warn, CreateMessageFormatter(null, format, args), null);
		}

		[StringFormatMethod("format")]
		public override void WarnFormat(string format, Exception exception, params object[] args)
		{
			if(IsWarnEnabled)
				this.Write(LogLevel.Warn, CreateMessageFormatter(null, format, args), exception);
		}

		public override void Warn(Action<FormatMessageHandler> formatMessageCallback)
		{
			if(IsWarnEnabled)
				this.Write(LogLevel.Warn, CreateMessageFormatter(null, formatMessageCallback), null);
		}

		public override void Warn(Action<FormatMessageHandler> formatMessageCallback, Exception exception)
		{
			if(IsWarnEnabled)
				this.Write(LogLevel.Warn, CreateMessageFormatter(null, formatMessageCallback), exception);
		}

		public override void Warn(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback)
		{
			if(IsWarnEnabled)
				this.Write(LogLevel.Warn, CreateMessageFormatter(formatProvider, formatMessageCallback), null);
		}

		public override void Warn(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback, Exception exception)
		{
			if(IsWarnEnabled)
				this.Write(LogLevel.Warn, CreateMessageFormatter(formatProvider, formatMessageCallback), exception);
		}

		public override void Error(object message)
		{
			if(IsErrorEnabled)
				this.Write(LogLevel.Error, message, null);
		}

		public override void Error(object message, Exception exception)
		{
			if(IsErrorEnabled)
				this.Write(LogLevel.Error, message, exception);
		}

		[StringFormatMethod("format")]
		public override void ErrorFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			if(IsErrorEnabled)
				this.Write(LogLevel.Error, CreateMessageFormatter(formatProvider, format, args), null);
		}

		[StringFormatMethod("format")]
		public override void ErrorFormat(IFormatProvider formatProvider, string format, Exception exception, params object[] args)
		{
			if(IsErrorEnabled)
				this.Write(LogLevel.Error, CreateMessageFormatter(formatProvider, format, args), exception);
		}

		[StringFormatMethod("format")]
		public override void ErrorFormat(string format, params object[] args)
		{
			if(IsErrorEnabled)
				this.Write(LogLevel.Error, CreateMessageFormatter(null, format, args), null);
		}

		[StringFormatMethod("format")]
		public override void ErrorFormat(string format, Exception exception, params object[] args)
		{
			if(IsErrorEnabled)
				this.Write(LogLevel.Error, CreateMessageFormatter(null, format, args), exception);
		}

		public override void Error(Action<FormatMessageHandler> formatMessageCallback)
		{
			if(IsErrorEnabled)
				this.Write(LogLevel.Error, CreateMessageFormatter(null, formatMessageCallback), null);
		}

		public override void Error(Action<FormatMessageHandler> formatMessageCallback, Exception exception)
		{
			if(IsErrorEnabled)
				this.Write(LogLevel.Error, CreateMessageFormatter(null, formatMessageCallback), exception);
		}

		public override void Error(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback)
		{
			if(IsErrorEnabled)
				this.Write(LogLevel.Error, CreateMessageFormatter(formatProvider, formatMessageCallback), null);
		}

		public override void Error(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback, Exception exception)
		{
			if(IsErrorEnabled)
				this.Write(LogLevel.Error, CreateMessageFormatter(formatProvider, formatMessageCallback), exception);
		}

		public override void Fatal(object message)
		{
			if(IsFatalEnabled)
				this.Write(LogLevel.Fatal, message, null);
		}

		public override void Fatal(object message, Exception exception)
		{
			if(IsFatalEnabled)
				this.Write(LogLevel.Fatal, message, exception);
		}

		[StringFormatMethod("format")]
		public override void FatalFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			if(IsFatalEnabled)
				this.Write(LogLevel.Fatal, CreateMessageFormatter(formatProvider, format, args), null);
		}

		[StringFormatMethod("format")]
		public override void FatalFormat(IFormatProvider formatProvider, string format, Exception exception, params object[] args)
		{
			if(IsFatalEnabled)
				this.Write(LogLevel.Fatal, CreateMessageFormatter(formatProvider, format, args), exception);
		}

		[StringFormatMethod("format")]
		public override void FatalFormat(string format, params object[] args)
		{
			if(IsFatalEnabled)
				this.Write(LogLevel.Fatal, CreateMessageFormatter(null, format, args), null);
		}

		[StringFormatMethod("format")]
		public override void FatalFormat(string format, Exception exception, params object[] args)
		{
			if(IsFatalEnabled)
				this.Write(LogLevel.Fatal, CreateMessageFormatter(null, format, args), exception);
		}

		public override void Fatal(Action<FormatMessageHandler> formatMessageCallback)
		{
			if(IsFatalEnabled)
				this.Write(LogLevel.Fatal, CreateMessageFormatter(null, formatMessageCallback), null);
		}

		public override void Fatal(Action<FormatMessageHandler> formatMessageCallback, Exception exception)
		{
			if(IsFatalEnabled)
				this.Write(LogLevel.Fatal, CreateMessageFormatter(null, formatMessageCallback), exception);
		}

		public override void Fatal(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback)
		{
			if(IsFatalEnabled)
				this.Write(LogLevel.Fatal, CreateMessageFormatter(formatProvider, formatMessageCallback), null);
		}

		public override void Fatal(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback, Exception exception)
		{
			if(IsFatalEnabled)
				this.Write(LogLevel.Fatal, CreateMessageFormatter(formatProvider, formatMessageCallback), exception);
		}

		#endregion

	}
}