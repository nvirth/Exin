using System;
using System.Collections.Generic;
using System.Text;
using Common.Log.New.CommonLogging.Loggers.Base;
using Common.Logging.Simple;
using Common.Logging;
using Common.Log.New.Core;

namespace Common.Log.New.CommonLogging
{
	/// <summary>
	/// Sends log messages to <see cref="Console.Out" />.
	/// </summary>
	/// <author>Gilles Bayon</author>
	public class ConsoleColorOutLogger : AbstractSimpleLoggerBase
	{
		private static readonly Dictionary<LogLevel, ConsoleColor> Colors = new Dictionary<LogLevel, ConsoleColor> {
			{ LogLevel.Fatal, ConsoleColor.Red },
			{ LogLevel.Error, ConsoleColor.Yellow },
			{ LogLevel.Warn, ConsoleColor.Magenta },
			{ LogLevel.Info, ConsoleColor.White },
			{ LogLevel.Debug, ConsoleColor.Gray },
			{ LogLevel.Trace, ConsoleColor.DarkGray },
		};

		private readonly bool _useColor;

		#region Ctor

		/// <summary>
		/// Creates and initializes a logger that writes messages to <see cref="Console.Out" />.
		/// </summary>
		/// <param name="logName">The name, usually type name of the calling class, of the logger.</param>
		/// <param name="logLevel">The current logging threshold. Messages recieved that are beneath this threshold will not be logged.</param>
		/// <param name="showLevel">Include the current log level in the log message.</param>
		/// <param name="showDateTime">Include the current time in the log message.</param>
		/// <param name="showLogName">Include the instance name in the log message.</param>
		/// <param name="dateTimeFormat">The date and time format to use in the log message.</param>
		public ConsoleColorOutLogger(string logName, LogLevel logLevel, bool showLevel, bool showDateTime, bool showLogName, string dateTimeFormat)
			: base(logName, logLevel, showLevel, showDateTime, showLogName, dateTimeFormat)
		{
		}

		/// <summary>
		/// Creates and initializes a logger that writes messages to <see cref="Console.Out" />.
		/// </summary>
		/// <param name="logName">The name, usually type name of the calling class, of the logger.</param>
		/// <param name="logLevel">The current logging threshold. Messages recieved that are beneath this threshold will not be logged.</param>
		/// <param name="showLevel">Include the current log level in the log message.</param>
		/// <param name="showDateTime">Include the current time in the log message.</param>
		/// <param name="showLogName">Include the instance name in the log message.</param>
		/// <param name="dateTimeFormat">The date and time format to use in the log message.</param>
		/// <param name="useColor">Use color when writing the log message.</param>
		public ConsoleColorOutLogger(string logName, LogLevel logLevel, bool showLevel, bool showDateTime, bool showLogName, string dateTimeFormat, bool useColor)
			: this(logName, logLevel, showLevel, showDateTime, showLogName, dateTimeFormat)
		{
			_useColor = useColor;
		}

		#endregion

		protected override void WriteInternal(LogLevel level, object message, Exception e)
		{
			var sb = new StringBuilder();
			FormatOutput(sb, level, message, e);
			var logMsg = sb.ToString();

			ConsoleColor color;
			if(_useColor && Colors.TryGetValue(level, out color))
			{
				var originalColor = Console.ForegroundColor;
				try
				{
					Console.ForegroundColor = color;
					Console.Out.WriteLine(logMsg);
					return;
				}
				finally
				{
					Console.ForegroundColor = originalColor;
				}
			}

			Console.Out.WriteLine(logMsg);
		}
	}
}