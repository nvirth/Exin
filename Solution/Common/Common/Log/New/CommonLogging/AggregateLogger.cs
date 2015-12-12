using System;
using System.Collections.Generic;
using System.Text;
using Common.Logging.Simple;
using Common.Logging;
using Common.Utils.Helpers;
using Common.Log.New;
using Common.Log.New.Core;

namespace Common.Log.New.CommonLogging
{
	public struct LoggerInstancesArgs
	{
		public IList<IExinLog> UiLoggers { get; set; }
		public IList<IExinLog> LogLoggers { get; set; }
	}

	public class AggregateLogger : AbstractSimpleLogger, IExinLog
	{
		public IList<IExinLog> UiLoggers { get; private set; }
		public IList<IExinLog> LogLoggers { get; private set; }

		public AggregateLogger(string logName, LogLevel logLevel, bool showLevel, bool showDateTime, bool showLogName, string dateTimeFormat, LoggerInstancesArgs instances)
			: base(logName, logLevel, showLevel, showDateTime, showLogName, dateTimeFormat)
		{
			UiLoggers = instances.UiLoggers;
			LogLoggers = instances.LogLoggers;
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

			if(logToLog)
				LogLoggers.ForEach(logger => logger.Write(level, message, e));
			if(logToUi)
				UiLoggers.ForEach(logger => logger.Write(level, message, e));

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

		public new bool IsLevelEnabled(LogLevel level)
		{
			return base.IsLevelEnabled(level);
		}

		public void Write(LogLevel logLevel, object message, Exception exception)
		{
			WriteInternal(logLevel, message, exception);
		}
	}
}