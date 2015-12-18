using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using Common.Configuration;
using Common.Logging;
using Common.Utils.Helpers;

namespace Exin.Common.Logging.Core
{
	public class LogData
	{
		public LogLevel LogLevel { get; set; }

		public string Tag { get; private set; }

		public string MessageFormat { get; private set; }
		public object[] MessageFormatArgs { get; private set; }

		public ResourceManager ResourceManager { get; private set; }
		public string ResourceKey { get; private set; }
		public object[] ResourceFormatArgs { get; private set; }

		public Exception Exception { get; private set; }
		public string ManualStrackTrace { get; private set; }

		// Not in use; but could be if needed
		private readonly IFormatProvider _formatProvider;

		public LogTarget LogTarget { get; private set; }
		public bool LogToLog => (LogTarget & LogTarget.Log) > 0;
		public bool LogToUi => (LogTarget & LogTarget.Ui) > 0;

		#region Ctor, Init

		public LogData(string tag, Func<MessageFormatterHandler, string> printMessageCallback, Exception exception, LogTarget logTarget, LogLevel logLevel)
		{
			Extract(printMessageCallback);
            Init(tag, exception, logTarget, logLevel);
		}

		public LogData(string tag, Func<MessageFormatterLocalizedHandler, string> printMessageCallback, Exception exception, LogTarget logTarget, LogLevel logLevel)
		{
			Extract(printMessageCallback);
			Init(tag, exception, logTarget, logLevel);
		}

		private void Init(string tag, Exception exception, LogTarget logTarget, LogLevel logLevel)
		{
			Tag = tag;
			Exception = exception;
			LogTarget = logTarget;
			LogLevel = logLevel;

			//if(exception != null && string.IsNullOrEmpty(exception.StackTrace))
			//	ManualStrackTrace = "{0}StackTrace:{0}{1}".Formatted(Environment.NewLine, Environment.StackTrace);

			if(MessageFormat == null)
			{
				if(ResourceManager == null && exception == null)
					throw new InvalidOperationException("This LogData instance is invalid, there is no data in it: MessageFormat == null && ResourceManager == null && Exception == null");
				if(exception != null)
				{
					MessageFormat = exception.Message;
					MessageFormatArgs = new object[0];
				}
			}
		}

		private void Extract(Func<MessageFormatterHandler, string> formatMessageHandlerFunc)
		{
			formatMessageHandlerFunc?.Invoke(DoExtract);
		}

		private void Extract(Func<MessageFormatterLocalizedHandler, string> formatMessageHandlerFunc)
		{
			formatMessageHandlerFunc?.Invoke(DoExtract);
		}

		private string DoExtract(string format, params object[] args)
		{
			MessageFormat = format;
			MessageFormatArgs = args;
			return null;
		}

		private string DoExtract(ResourceManager resourceManager, string resourceKey, params object[] args)
		{
			ResourceManager = resourceManager;
			ResourceKey = resourceKey;
			ResourceFormatArgs = args;
			return null;
		}

		#endregion

		#region Message

		public override string ToString()
		{
			return GetMessageForLog();
		}
		public string GetMessageForLog()
		{
			return GetFullMessage(LogTarget.Log);
		}
		public string GetMessageForUi()
		{
			return GetFullMessage(LogTarget.Ui);
		}

		private string GetFullMessage(LogTarget logTarget)
		{
			var cachedMessage = GetCachedMessage(logTarget);
			if(cachedMessage != null)
				return cachedMessage;

			CultureInfo cultureInfo;
			string tag;
			switch(logTarget)
			{
				case LogTarget.Log:
					cultureInfo = Cultures.LogCulture;
					tag = Tag;
					break;
				case LogTarget.Ui:
					cultureInfo = Cultures.CurrentCulture;
					tag = "";
					break;
				default:
					throw new ArgumentOutOfRangeException("logTarget");
			}

			var fullMessage = CalcFullMessage(cultureInfo, tag);
			SetCachedMessage(logTarget, fullMessage);

			return fullMessage;
		}
		private string CalcFullMessage(CultureInfo cultureInfo, string tag)
		{
			string fullMessage;
			if(Exception == null || !string.IsNullOrEmpty(Exception.StackTrace))
			{
				fullMessage = "{0}{1}".Formatted(tag, GetCoreMessage(cultureInfo));
			}
			else
			{
				var stackTrace = "{0}StackTrace:{0}{1}".Formatted(Environment.NewLine, Environment.StackTrace);
				fullMessage = "{0}{1}{2}".Formatted(tag, GetCoreMessage(cultureInfo), stackTrace);
			}
			return fullMessage;
		}

		private string GetCoreMessage(CultureInfo cultureInfo)
		{
			var messageFormat = MessageFormat;
			var messageFormatArgs = MessageFormatArgs;
			if(ResourceManager != null)
			{
				messageFormatArgs = ResourceFormatArgs;
				messageFormat = ResourceManager.GetString(ResourceKey, cultureInfo)
				                ?? "!!! MISSING RESOURCE KEY: {0}/{1} !!!".Formatted(ResourceManager.BaseName, ResourceKey);
			}

			var coreMessage = string.Format(messageFormat, messageFormatArgs);
			return coreMessage;
		}

		#region Cache
		private readonly Dictionary<LogTarget, string> _cache = new Dictionary<LogTarget, string>();
		private string GetCachedMessage(LogTarget logTarget)
		{
			return _cache.ContainsKey(logTarget) ? _cache[logTarget] : null;
		}
		private void SetCachedMessage(LogTarget logTarget, string value)
		{
			_cache[logTarget] = value;
		}
		#endregion

		#endregion
	}
}