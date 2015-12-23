using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;

namespace Exin.Common.Logging.Core
{
	internal static class LogHelpers
	{
		// TODO callerFnName is not used
		public static async void DoLogInEarlyPhase(string message, LogLevel logLevel, Exception exception = null, [CallerMemberName] string callerFnName = null)
		{
			var preMsg = exception == null ? message : message + " --- Exception.Message: " + exception.Message;
			if(logLevel > LogLevel.Info)
				Console.Error.WriteLine(preMsg);
			else
				Debug.WriteLine(preMsg);

			await Log.LogInitializedDfd.Task;

			Log.LogAtLevel(m => m(message), logLevel, exception, LogTarget.Log);
		}
	}
}
