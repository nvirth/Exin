using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Common.Logging.Simple;
using Common.Logging;
using Common.Log.New.Core;

namespace Common.Log.New.CommonLogging
{
	/// <summary>
	/// Sends log messages to <see cref="Console.Out" />.
	/// </summary>
	/// <author>Gilles Bayon</author>
	public class ConsoleOutLogger : AbstractSimpleLogger, ICommonLoggingAggregate
	{
		private readonly bool useColor;

		/// Dummy ctor, does not initialize the log msg formatting
		public ConsoleOutLogger(string name, bool useColor) : base(name, LogLevel.All, true, true, true, "")
		{
			this.useColor = useColor;
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
		public ConsoleOutLogger(string logName, LogLevel logLevel, bool showLevel, bool showDateTime, bool showLogName, string dateTimeFormat, bool useColor)
			: base(logName, logLevel, showLevel, showDateTime, showLogName, dateTimeFormat)
		{
			this.useColor = useColor;
		}

		public void Write(LogLevel logLevel, object message, Exception exception)
		{
			WriteInternal(logLevel, message, exception);
		}

		/// <summary>
		/// Do the actual logging by constructing the log message using a <see cref="StringBuilder" /> then
		/// sending the output to <see cref="Console.Out" />.
		/// </summary>
		/// <param name="level">The <see cref="LogLevel" /> of the message.</param>
		/// <param name="message">The log message.</param>
		/// <param name="e">An optional <see cref="Exception" /> associated with the message.</param>
		protected override void WriteInternal(LogLevel level, object message, Exception e)
		{
			// Use a StringBuilder for better performance
			var sb = new StringBuilder();
			FormatOutput(sb, level, message, e);

#if DEBUG
			if(useColor)
				WriteInDebugMode(level, sb);
			else
				Console.Out.WriteLine(sb.ToString());
#else
			Console.Out.WriteLine(sb.ToString());
#endif
		}

		private static void WriteInDebugMode(LogLevel level, StringBuilder sb)
		{
			// Print to the appropriate destination
			if(level <= LogLevel.Debug)
			{
				if(Debugger.IsAttached)
					System.Diagnostics.Debug.WriteLine(sb.ToString());
				else
					Console.Out.WriteLine(sb.ToString());
			}
			else if(level == LogLevel.Info)
			{
				Console.Out.WriteLine(sb.ToString());
			}			
			else //if(level >= LogLevel.Warn)
			{
				Console.Error.WriteLine(sb.ToString());
			}
		}
	}
}