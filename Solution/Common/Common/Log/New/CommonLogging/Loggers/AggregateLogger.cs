using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using Common.Configuration;
using Common.Logging.Simple;
using Common.Logging;
using Common.Logging.Factory;
using Common.Utils.Helpers;
using Common.Log.New;
using Common.Log.New.CommonLogging.Loggers.Base;
using Common.Log.New.Core;

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


	}


	public class LocalizedCallbackMessageFormatter
	{
		public CultureInfo CultureInfo { get; set; }

		private bool _isLocalized;

		private readonly Dictionary<CultureInfo, string> _cache = new Dictionary<CultureInfo, string>();
		private string CachedMessage
		{
			get
			{
				var key = _isLocalized ? CultureInfo : CultureInfo.InvariantCulture;
				return _cache.ContainsKey(key) ? _cache[key] : null;
			}
			set
			{
				if(_isLocalized)
					_cache[CultureInfo] = value;
				else
					_cache[CultureInfo.InvariantCulture] = value;
			}
		}

		// --

		private readonly Action<FormatMessageHandler> _formatMessageCallback;
		private readonly IFormatProvider _formatProvider;

		public LocalizedCallbackMessageFormatter(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback)
		{
			_formatProvider = formatProvider;
			_formatMessageCallback = formatMessageCallback;
			CultureInfo = Cultures.DefaultCulture;
		}

		private string FormatMessage(string format, params object[] args)
		{
			_isLocalized = false;

			if(args?.Length > 0)
			{
				var resourceManager = args[0] as ResourceManager;
				if(resourceManager != null)
				{
					_isLocalized = true;

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

			CachedMessage = string.Format(_formatProvider, format, args);
			return CachedMessage;
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