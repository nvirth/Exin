using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Common.Logging.Simple;
using Common.Logging;
using Common.Log.New.Core;
using System.Windows.Controls;
using System.Windows.Media;
using Common.Utils;

namespace Common.Log.New.CommonLogging
{
	public class WpfRichTextBoxLogger : AbstractSimpleLogger, IExinLog
	{
		// TODO try out colors
		private static readonly Dictionary<LogLevel, Brush> Colors = new Dictionary<LogLevel, Brush> {
			{ LogLevel.Fatal, Brushes.Red },
			{ LogLevel.Error, Brushes.Red },
			{ LogLevel.Warn, Brushes.DarkOrange },
			{ LogLevel.Info, Brushes.Black },
			{ LogLevel.Debug, Brushes.Gray },
			{ LogLevel.Trace, Brushes.DarkGray },
		};

		public RichTextBox RichTextBox { get; }

		public WpfRichTextBoxLogger(string logName, LogLevel logLevel, bool showLevel, bool showDateTime, bool showLogName, string dateTimeFormat, RichTextBox richTextBox)
			: base(logName, logLevel, showLevel, showDateTime, showLogName, dateTimeFormat)
		{
			RichTextBox = richTextBox;
		}

		public void Write(LogLevel logLevel, object message, Exception exception)
		{
			WriteInternal(logLevel, message, exception);
		}

		public new bool IsLevelEnabled(LogLevel level)
		{
			return base.IsLevelEnabled(level);
		}

		protected override void WriteInternal(LogLevel level, object message, Exception e)
		{
			var sb = new StringBuilder();
			FormatOutput(sb, level, message, e);
			var logMsg = sb.ToString();

			Brush color;
			if(!Colors.TryGetValue(level, out color))
				color = Brushes.Black;

			MessagePresenterManager.AppendToWpfRichTextbox(logMsg, RichTextBox, color, 
				newLineAtEnd: true, scrollToEnd: true
			);
        }
	}
}