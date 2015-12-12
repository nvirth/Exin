using System;
using System.Threading;
using Common.Utils;
using log4net;
using log4net.Config;

namespace Common.Log
{
	public class ExinLogger
	{
		private ILog _logger;
		private const string USERNAME_LOGENTRY = "User";
		private const string MAIN_TITLE = "EXIN";

		public ExinLogger(string name = null)
		{
			name = name ?? MAIN_TITLE;

			_logger = LogManager.GetLogger(name);
			XmlConfigurator.Configure();
		}

		public void AddUserIdentityToLog(Exception e)
		{
			var userName = Thread.CurrentPrincipal.Identity.Name;
			userName = string.IsNullOrWhiteSpace(userName) ? "(unauthenticed)" : userName;

			e.AddData(USERNAME_LOGENTRY, userName);
		}

		// TODO logging to UI should be localized, but logging to logs must not!

		//public Exception LogException(string msg, Exception e, object data = null, bool logToUi)
		//{ }
		//
		//public Exception LogException(string msg, Exception e, object data = null)

		public Exception LogException(string msg, Exception e, object data = null, bool logToUi = true)
		{
			if(logToUi)
				MessagePresenter.Instance.WriteError(msg);

			if(data != null)
			{
				var dataSerialized = data.SerializeToLog();

				if(logToUi)
					MessagePresenter.Instance.WriteError(dataSerialized);

				e.AddData(dataSerialized);
			}

			if(logToUi)
				MessagePresenter.Instance.WriteException(e);

			Error(msg, e);

			return e;
		}

		public void LogError(string msg, object data = null, bool logToUi = true)
		{
			if(logToUi)
				MessagePresenter.Instance.WriteError(msg);

			if(data != null)
			{
				var dataSerialized = data.SerializeToLog();
				msg += Environment.NewLine + dataSerialized;

				if(logToUi)
					MessagePresenter.Instance.WriteError(dataSerialized);
			}

			Error(msg);
		}

		public void LogInfo(string msg, object data = null, bool logToUi = true)
		{
			if(logToUi)
				MessagePresenter.Instance.WriteLine(msg);

			if(data != null)
			{
				var dataSerialized = data.SerializeToLog();
				msg += Environment.NewLine + dataSerialized;

				if(logToUi)
					MessagePresenter.Instance.WriteLine(dataSerialized);
			}

			Info(msg);
		}

		#region ILog Adapter

		private void Debug(object message)
		{
			_logger.Debug(message);
		}

		private void Debug(object message, Exception exception)
		{
			AddUserIdentityToLog(exception);
			_logger.Debug(message, exception);
		}

		private void Info(object message)
		{
			_logger.Info(message);
		}

		private void Info(object message, Exception exception)
		{
			AddUserIdentityToLog(exception);
			_logger.Info(message, exception);
		}

		private void Warn(object message)
		{
			_logger.Warn(message);
		}

		private void Warn(object message, Exception exception)
		{
			AddUserIdentityToLog(exception);
			_logger.Warn(message, exception);
		}

		private void Error(object message)
		{
			_logger.Error(message);
		}

		private void Error(object message, Exception exception)
		{
			AddUserIdentityToLog(exception);
			_logger.Error(message, exception);
		}

		private void Fatal(object message)
		{
			_logger.Fatal(message);
		}

		private void Fatal(object message, Exception exception)
		{
			AddUserIdentityToLog(exception);
			_logger.Fatal(message, exception);
		}

		#endregion

	}
}