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
		private enum Type
		{
			StringFormat,
			ResourceManager,
			DynamicLocalized,
		}

		public LogLevel LogLevel { get; set; }

		public string Tag { get; private set; }

		public string MessageFormat { get; private set; }
		public object[] MessageFormatArgs { get; private set; }

		public ResourceManager ResourceManager { get; private set; }
		public string ResourceKey { get; private set; }
		public object[] ResourceFormatArgs { get; private set; }

		public Exception Exception { get; private set; }
		public string ManualStrackTrace { get; private set; }
		public object PlusData { get; private set; }

		public LogTarget LogTarget { get; private set; }
		public bool LogToLog => (LogTarget & LogTarget.Log) > 0;
		public bool LogToUi => (LogTarget & LogTarget.Ui) > 0;

		// --
		
		private readonly Type _type;
		private readonly IFormatProvider _formatProvider; // Not in use; but could be if needed
		private readonly Func<MessageFormatterHandler, CultureInfo, string> _printMessageCallbackLocalized;

		private string _plusDataStr;
		public string PlusDataStr => _plusDataStr ?? (_plusDataStr = PlusData != null ? "\r\nData: " + PlusData.SerializeToLog() : "");

		#region Ctor, Init

		public LogData(string tag, Func<MessageFormatterHandler, string> printMessageCallback, Exception exception, LogTarget logTarget, LogLevel logLevel)
		{
			_type = Type.StringFormat;

			Extract(printMessageCallback);
			Init(tag, exception, logTarget, logLevel);

			if(printMessageCallback == null)
				FixEmptyMessage();
		}
		public LogData(string tag, Func<MessageFormatterLocalizedHandler, string> printMessageCallback, Exception exception, LogTarget logTarget, LogLevel logLevel)
		{
			_type = Type.ResourceManager;

			Extract(printMessageCallback);
			Init(tag, exception, logTarget, logLevel);

			if(printMessageCallback == null)
				FixEmptyMessage();
		}
		public LogData(string tag, Func<MessageFormatterHandler, CultureInfo, string> printMessageCallback, Exception exception, LogTarget logTarget, LogLevel logLevel)
		{
			_type = Type.DynamicLocalized;

			_printMessageCallbackLocalized = printMessageCallback;
			Init(tag, exception, logTarget, logLevel);

			if(printMessageCallback == null)
				FixEmptyMessage();
		}

		private void Init(string tag, Exception exception, LogTarget logTarget, LogLevel logLevel)
		{
			Tag = tag;
			LogTarget = logTarget;
			LogLevel = logLevel;

			var dataException = exception as ForDataOnlyException;
			if(dataException != null)
				PlusData = dataException.LogData;
			else
				Exception = exception;
		}

		private void FixEmptyMessage()
		{
			if(MessageFormat == null)
			{
				if(ResourceManager == null && Exception == null)
					throw new InvalidOperationException("This LogData instance is invalid, there is no data in it: MessageFormat == null && ResourceManager == null && Exception == null");
				if(Exception != null)
				{
					MessageFormat = Exception.Message;
					MessageFormatArgs = new object[0];
				}
			}
		}
		
		#endregion

		#region Extract

		private int _formatMessageHandlerCallCount;

		private void Extract(Func<MessageFormatterHandler, string> formatMessageHandlerFunc)
		{
			ExtractSafe(() => formatMessageHandlerFunc?.Invoke(DoExtract));
		}
		private void Extract(Func<MessageFormatterHandler, CultureInfo, string> formatMessageHandlerFunc, CultureInfo currentCulture)
		{
			ExtractSafe(() => formatMessageHandlerFunc(DoExtract, currentCulture));
		}
		private void Extract(Func<MessageFormatterLocalizedHandler, string> formatMessageHandlerFunc)
		{
			ExtractSafe(() => formatMessageHandlerFunc?.Invoke(DoExtract));
		}

		private void ExtractSafe(Action extractAction)
		{
			try
			{
				_formatMessageHandlerCallCount = 0;
				extractAction();
				_formatMessageHandlerCallCount = 0;
			}
			catch(Exception e)
			{
				Log.Warn(this, m => m("LOGGING ERROR: Exception while executing the FormatMessageCallback. "), LogTarget.All, e);
			}
		}

		private string DoExtract(string format, params object[] args)
		{
			EnsureFormatMessageHandlerCalledOnce();

			MessageFormat = format;
			MessageFormatArgs = args;
			return null;
		}
		private string DoExtract(ResourceManager resourceManager, string resourceKey, params object[] args)
		{
			EnsureFormatMessageHandlerCalledOnce();
				
			ResourceManager = resourceManager;
			ResourceKey = resourceKey;
			ResourceFormatArgs = args;
			return null;
		}

		private void EnsureFormatMessageHandlerCalledOnce()
		{
			_formatMessageHandlerCallCount++;

			if(_formatMessageHandlerCallCount == 1)
				return;

			Log.Warn(this, m => m("The formatMessageHandler may only be called once, and has to contain the full log data. "));
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
				if(ManualStrackTrace == null)
					ManualStrackTrace = "{0}StackTrace:{0}{1}".Formatted(Environment.NewLine, Environment.StackTrace);

				fullMessage = "{0}{1}{2}".Formatted(tag, GetCoreMessage(cultureInfo), ManualStrackTrace);
			}
			return fullMessage;
		}

		private string GetCoreMessage(CultureInfo cultureInfo)
		{
			string messageFormat;
			object[] messageFormatArgs;
			switch(_type)
			{
				case Type.StringFormat:
					messageFormat = MessageFormat;
					messageFormatArgs = MessageFormatArgs;
					break;
				case Type.ResourceManager:
					messageFormatArgs = ResourceFormatArgs;
					messageFormat = ResourceManager.GetString(ResourceKey, cultureInfo)
									?? "!!! MISSING RESOURCE KEY: {0}/{1} !!!".Formatted(ResourceManager.BaseName, ResourceKey);
					break;
				case Type.DynamicLocalized:
					Extract(_printMessageCallbackLocalized, cultureInfo);
					messageFormat = MessageFormat;
					messageFormatArgs = MessageFormatArgs;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			var coreMessage = string.Format(messageFormat, messageFormatArgs);
			coreMessage += PlusDataStr;

			return coreMessage;
		}

		#region Cache
		private readonly Dictionary<LogTarget, string> _cache = new Dictionary<LogTarget, string>();
		private string GetCachedMessage(LogTarget logTarget)
		{
			logTarget = _type == Type.StringFormat ? LogTarget.All : logTarget;
			return _cache.ContainsKey(logTarget) ? _cache[logTarget] : null;
		}
		private void SetCachedMessage(LogTarget logTarget, string value)
		{
			logTarget = _type == Type.StringFormat ? LogTarget.All : logTarget;
			_cache[logTarget] = value;
		}
		#endregion

		#endregion
	}
}