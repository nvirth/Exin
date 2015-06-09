using System;

namespace Common.Utils
{
	public class MessagePresenter
	{
		public static readonly MessagePresenter Instance = new MessagePresenter();

		public bool IsMuted { get; set; }

		public event Action WriteLineSeparatorEvent;
		public event Action<string> WriteEvent;
		public event Action<string> WriteLineEvent;
		public event Action<string> WriteErrorEvent;
		public event Action<Exception> WriteExceptionEvent;

		public void FetchHandlersFrom(MessagePresenter otherInstance)
		{
			// Don't worry, these are immutable :)
			// "You don't need to worry about that. The EventHandler<EventArgs> object is immutable so any change in the list of listeners in either object will cause that object to get a new EventHandler<EventArgs> instance containing the updated invocation list."
			// http://stackoverflow.com/questions/6296277/c-sharp-clone-eventhandler
			//
			otherInstance.WriteLineSeparatorEvent = this.WriteLineSeparatorEvent;
			otherInstance.WriteEvent = this.WriteEvent;
			otherInstance.WriteLineEvent = this.WriteLineEvent;
			otherInstance.WriteErrorEvent = this.WriteErrorEvent;
			otherInstance.WriteExceptionEvent = this.WriteExceptionEvent;
		}

		public void Write(string message)
		{
			if(IsMuted)
				return;

			var handler = WriteEvent;
			handler?.Invoke(message);
		}

		public void WriteLine(string message)
		{
			if(IsMuted)
				return;

			var handler = WriteLineEvent;
			handler?.Invoke(message);
		}

		public void WriteLineSeparator()
		{
			if(IsMuted)
				return;

			var handler = WriteLineSeparatorEvent;
			handler?.Invoke();
		}

		public void WriteError(string message)
		{
			if(IsMuted)
				return;

			var handler = WriteErrorEvent ?? WriteLine;
			handler?.Invoke(message);
		}

		public void WriteException(Exception e)
		{
			if(IsMuted)
				return;

			var handler = WriteExceptionEvent;
			if(handler != null)
				handler(e);
			else
				WriteError(e.Message);
		}
	}
}
