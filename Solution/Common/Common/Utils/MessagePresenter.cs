using System;

namespace Common.Utils
{
	public static class MessagePresenter
	{
		public static event Action WriteLineSeparatorEvent;
		public static event Action<string> WriteEvent;
		public static event Action<string> WriteLineEvent;
		public static event Action<string> WriteErrorEvent;
		public static event Action<Exception> WriteExceptionEvent;

		public static void Write(string message)
		{
			var handler = WriteEvent;
			if(handler != null)
				handler(message);
		}

		public static void WriteLine(string message)
		{
			var handler = WriteLineEvent;
			if(handler != null)
				handler(message);
		}

		public static void WriteLineSeparator()
		{
			var handler = WriteLineSeparatorEvent;
			if(handler != null)
				handler();
		}

		public static void WriteError(string message)
		{
			var handler = WriteErrorEvent;
			if(handler != null)
				handler(message);
			else
				WriteLine(message);
		}

		public static void WriteException(Exception e)
		{
			var handler = WriteExceptionEvent;
			if(handler != null)
				handler(e);
			else
				WriteError(e.Message);
		}
	}
}
