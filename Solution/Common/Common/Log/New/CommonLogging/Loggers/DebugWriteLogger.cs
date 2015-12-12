using System;
using System.Diagnostics;
using System.Text;
using Common.Log.New.CommonLogging.Loggers.Base;
using Common.Logging;
using Common.Logging.Simple;
using Common.Log.New.Core;

namespace Common.Log.New.CommonLogging
{
	/// <summary>
	///     Sends log messages to <see cref="System.Diagnostics.Debug.WriteLine" />.
	/// </summary>
	/// <author>Gilles Bayon</author>
	public class DebugWriteLogger : AbstractSimpleLoggerBase
	{
		#region Ctor
		/// <summary>
		///     Creates and initializes a logger that writes messages to <see cref="Debug.WriteLine" />.
		/// </summary>
		/// <param name="logName">The name, usually type name of the calling class, of the logger.</param>
		/// <param name="logLevel">
		///     The current logging threshold. Messages recieved that are beneath this threshold will not be
		///     logged.
		/// </param>
		/// <param name="showLevel">Include the current log level in the log message.</param>
		/// <param name="showDateTime">Include the current time in the log message.</param>
		/// <param name="showLogName">Include the instance name in the log message.</param>
		/// <param name="dateTimeFormat">The date and time format to use in the log message.</param>
		public DebugWriteLogger(string logName, LogLevel logLevel, bool showLevel, bool showDateTime, bool showLogName, string dateTimeFormat)
            : base(logName, logLevel, showLevel, showDateTime, showLogName, dateTimeFormat) { }
        #endregion

        protected override void WriteInternal(LogLevel level, object message, Exception e)
        {
	        if(!Debugger.IsAttached)
		        return;

			var sb = new StringBuilder();
            FormatOutput(sb, level, message, e);
	        var logMsg = sb.ToString();

	        System.Diagnostics.Debug.WriteLine(logMsg);
        }
	}
}