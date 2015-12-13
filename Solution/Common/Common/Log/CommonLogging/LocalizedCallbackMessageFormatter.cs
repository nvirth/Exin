using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using Common.Logging;
using Common.Utils.Helpers;

namespace Common.Log.CommonLogging
{
	/// <summary>
	/// Note: This class is a custom MessageFormatter. Not in use anymore.
	/// Common.Logging uses "object message" method args; and finally
	/// calls a ToString on them. It uses own MessageFormatters under the hood
	/// within its standard logger method implementations (Trace...Fatal).
	/// 
	/// We use now another approach, see LogData.cs
	/// </summary>
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