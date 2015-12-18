using System;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using Common.Logging.Configuration;
using Common.Logging.Simple;
using Exin.Common.Logging.Core;
using Exin.Common.Logging.CommonLogging.Loggers;

namespace Exin.Common.Logging.CommonLogging
{
	public class AggregateLoggerFactoryAdapter : AbstractSimpleLoggerFactoryAdapter
	{
		public LoggerInstancesArgs LoggerInstances { get; }

		public AggregateLoggerFactoryAdapter()
			: base(null)
		{
		}

		public AggregateLoggerFactoryAdapter(LogLevel logLevel, bool showDateTime, bool showLogName, bool showLevel, string dateTimeFormat, LoggerInstancesArgs instances)
			: base(logLevel, showDateTime, showLogName, showLevel, dateTimeFormat)
		{
			LoggerInstances = instances;
		}

		protected override ILog CreateLogger(string name, LogLevel level, bool showLevel, bool showDateTime, bool showLogName, string dateTimeFormat)
		{
			var loggerInstancesCopy = new LoggerInstancesArgs {
				UiLoggers = LoggerInstances.UiLoggers.ToList(),
				LogLoggers = LoggerInstances.LogLoggers.ToList(),
			};

            ILog log = new AggregateLogger(name, level, showLevel, showDateTime, showLogName, dateTimeFormat, loggerInstancesCopy);
			return log;
		}
	}
}