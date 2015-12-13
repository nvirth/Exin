﻿using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using System;
using LogLevel = Common.Logging.LogLevel;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using Common.Log.New.Core;
using System.Linq;
using System.Runtime.CompilerServices;
using Common.Utils.Helpers;

namespace Common.Log.New
{
	public class Log4NetLog
	{
		public const int MaxBackupDays = 90;

		private PatternLayout _layout = new PatternLayout();
		public PatternLayout DefaultLayout { get { return _layout; } }

		private const string LOG_PATTERN = "%date [%thread] %-5level %message%newline";
		public string DefaultPattern { get { return LOG_PATTERN; } }

		public Log4NetLog()
		{
			_layout.ConversionPattern = DefaultPattern;
			_layout.ActivateOptions();
		}

		public void AddAppender(IAppender appender)
		{
			Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();
			hierarchy.Root.AddAppender(appender);
		}

		//public static readonly string AppExecDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
		//public static readonly string AppExecDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
		public const string LogRootDirName = "logs";

		private static string _logRootDirPath;
		public static string LogRootDirPath
		{
			get
			{
				if(string.IsNullOrEmpty(_logRootDirPath))
				{
					string appExecDir;
					try
					{
						appExecDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
					}
					catch(Exception e)
					{
						DoLogInEarlyPhase("Could not set this: AppExecDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); ", LogLevel.Error, e);
						appExecDir = ""; // it will be a local path this way
					}
					_logRootDirPath = Path.Combine(appExecDir, LogRootDirName);
				}
				return _logRootDirPath;
			}
		}

		public static string GetLogDirPath()
		{
			var logDirName = DateTime.Today.ToString("yyyy_MM");       // Must not cache this, it contains date!
			var logDirPath = Path.Combine(LogRootDirPath, logDirName);
			return logDirPath;
		}

		public static string GetLogFilePath()
		{
			var logFileNameStart = DateTime.Today.ToString("ddMMyyyy");
			var logFileName = logFileNameStart + ".log";

			try
			{
				var logDir = new DirectoryInfo(GetLogDirPath());
				logDir.Create();

				var todayFilesCount = logDir
					.EnumerateFiles()
					.Count(fileInfo => fileInfo.Name.StartsWith(logFileNameStart, StringComparison.InvariantCultureIgnoreCase));

				logFileName = "{0}-{1:D6}-{2:X}.log".Formatted(logFileNameStart, todayFilesCount + 1, Guid.NewGuid().GetHashCode());

				var logFilePath = Path.Combine(logDir.FullName, logFileName);
				DoLogInEarlyPhase("The Log4Net logger's file path is:\r\n" + logFilePath, LogLevel.Debug);
				return logFilePath;
			}
			catch(Exception e)
			{
				DoLogInEarlyPhase("Could not calculate the logFilePath correctly. Default one will be used. ", LogLevel.Error, e);
				return logFileName;
			}
		}

		static Log4NetLog()
		{
			var hierarchy = (Hierarchy)LogManager.GetRepository();

			var patternLayout = new PatternLayout() {
				ConversionPattern = LOG_PATTERN,
				//IgnoresException = false, --> for custom rendering
			};
			patternLayout.ActivateOptions();

			// This woud pass the log messages to Debug.WriteLine
			//if(Debugger.IsAttached)
			//{
			//	var tracer = new TraceAppender() {
			//		Layout = patternLayout,
			//	};
			//	tracer.ActivateOptions();
			//	hierarchy.Root.AddAppender(tracer);
			//}

			var roller = new RollingFileAppender() {
				File = GetLogFilePath(),
				AppendToFile = true,
				RollingStyle = RollingFileAppender.RollingMode.Size,
				MaximumFileSize = "10MB",
				StaticLogFileName = false,
				MaxSizeRollBackups = 10,
				Layout = patternLayout,
				PreserveLogFileNameExtension = true,
			};
			roller.ActivateOptions();
			hierarchy.Root.AddAppender(roller);

			RefreshLogsPeriodically(roller);

			hierarchy.Root.Level = LogInit.GetLogLevel().ToLog4Net();
			hierarchy.Configured = true;
		}

		private static void RefreshLogsPeriodically(RollingFileAppender roller)
		{
			Task.Run(() =>
			{
				while(true)
				{
					PurgeOldLogFiles();

					// -- Refresh FilePath in every new day

					var todayMidnight = DateTime.Today.AddDays(1).AddMinutes(1); // +1 min for safety sake
					var diff = todayMidnight - DateTime.Now;

					DoLogInEarlyPhase("The Log4Net logger's file path will be refreshed in: {0}h {1}m {2}s".Formatted(diff.Hours, diff.Minutes, diff.Seconds), LogLevel.Debug);
					Thread.Sleep(diff);

					roller.File = GetLogFilePath();
					roller.ActivateOptions();
				}
			});
		}

		private static async void DoLogInEarlyPhase(string message, LogLevel logLevel, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			var preMsg = exception == null ? message : message + " --- Exception.Message: " + exception.Message;
			if(logLevel > LogLevel.Info)
				Console.Error.WriteLine(preMsg);
			else
				Debug.WriteLine(preMsg);

			await Core.Log.LogInitializedDfd.Task;

			Core.Log.LogAtLevel(typeof(Log4NetLog), m => m(message), logLevel, LogTarget.Log, exception);
		}

		private static void PurgeOldLogFiles()
		{
			try
			{
				var oldestAllowedDate = DateTime.Today.AddDays(MaxBackupDays * -1);

				var oldFilesToRemove = Directory
					.EnumerateFiles(LogRootDirPath, "*.log", SearchOption.AllDirectories)
					.Select(filePath => new FileInfo(filePath))
					.Where(fileInfo => fileInfo.CreationTime.Date < oldestAllowedDate)
					.ToList();

				oldFilesToRemove.ForEach(fileInfo => fileInfo.Delete());
				DoLogInEarlyPhase("Removed {0} log files older than {1} days. ".Formatted(oldFilesToRemove.Count, MaxBackupDays), LogLevel.Debug);

				var emptiedDirs = oldFilesToRemove
					.Select(fileInfo => fileInfo.DirectoryName)
					.Distinct()
					.Where(dirPath => !Directory.EnumerateFileSystemEntries(dirPath).Any())
					.ToList();

				emptiedDirs.ForEach(Directory.Delete);
				DoLogInEarlyPhase("Removed {0} emptied log folders. ".Formatted(emptiedDirs.Count), LogLevel.Debug);
			}
			catch(Exception e)
			{
				DoLogInEarlyPhase("Could not delete old log files. ", LogLevel.Error, e);
			}
		}

		public static ILog Create()
		{
			return LogManager.GetLogger("Log4Net");
		}
	}

	public static class Log4NetHelpers
	{
		public static Level ToLog4Net(this LogLevel logLevel)
		{
			switch(logLevel)
			{
				case LogLevel.All:
					return Level.All;
				case LogLevel.Trace:
					return Level.Trace;
				case LogLevel.Debug:
					return Level.Debug;
				case LogLevel.Info:
					return Level.Info;
				case LogLevel.Warn:
					return Level.Warn;
				case LogLevel.Error:
					return Level.Error;
				case LogLevel.Fatal:
					return Level.Fatal;
				default:
					throw new ArgumentOutOfRangeException("logLevel", (object)logLevel, "unknown log level");
			}
		}
		public static LogLevel ToCommonLogging(this Level logLevel)
		{
			if(logLevel == Level.All)
				return LogLevel.All;
			else if(logLevel == Level.Trace)
				return LogLevel.Trace;
			else if(logLevel == Level.Debug)
				return LogLevel.Debug;
			else if(logLevel == Level.Info)
				return LogLevel.Info;
			else if(logLevel == Level.Warn)
				return LogLevel.Warn;
			else if(logLevel == Level.Error)
				return LogLevel.Error;
			else if(logLevel == Level.Fatal)
				return LogLevel.Fatal;
			else
				throw new ArgumentOutOfRangeException("logLevel", (object)logLevel, "unknown log level");
		}
	}
}