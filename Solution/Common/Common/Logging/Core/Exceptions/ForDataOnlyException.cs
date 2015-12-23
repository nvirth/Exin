using System;

namespace Exin.Common.Logging.Core
{
	/// These kind of Exceptions are not meant to be thrown.
	/// Their purpose is only giving data to the log system in their Data property
	public class ForDataOnlyException : Exception
	{
		public object LogData { get; private set; }

		public ForDataOnlyException(object logData) : base("")
		{
			LogData = logData;
		}
	}
}