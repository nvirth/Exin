using System;
using Exin.Common.Logging.Log4Net;
using Common.Logging;
using Common.Logging.Factory;
using Exin.Common.Logging.Core;
using log4net.Core;

namespace Exin.Common.Logging.CommonLogging.Loggers
{
	public class Log4NetLogger : AbstractLogger, IExinLog
	{
		private static readonly Type DeclaringType = typeof(Log4NetLogger);
		private readonly log4net.Core.ILogger _logger;

		public override bool IsTraceEnabled => _logger.IsEnabledFor(Level.Trace);
		public override bool IsDebugEnabled => _logger.IsEnabledFor(Level.Debug);
		public override bool IsInfoEnabled => _logger.IsEnabledFor(Level.Info);
		public override bool IsWarnEnabled => _logger.IsEnabledFor(Level.Warn);
		public override bool IsErrorEnabled => _logger.IsEnabledFor(Level.Error);
		public override bool IsFatalEnabled => _logger.IsEnabledFor(Level.Fatal);

		public Log4NetLogger(ILoggerWrapper log)
		{
			_logger = log.Logger;
		}

		public void Write(LogLevel logLevel, object message, Exception exception)
		{
			WriteInternal(logLevel, message, exception);
		}

		public bool IsLevelEnabled(LogLevel level)
		{
			return _logger.IsEnabledFor(level.ToLog4Net());
		}

		protected override void WriteInternal(LogLevel logLevel, object message, Exception exception)
		{
			var level = logLevel.ToLog4Net();
			_logger.Log(DeclaringType, level, message, exception);
		}
	}
}
