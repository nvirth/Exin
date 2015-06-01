using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Common.Annotations;

namespace Common.UiModels.WPF.Base
{
	[Serializable]
	public class NotifyPropertyChanged : INotifyPropertyChanged
	{
		[field: NonSerialized]
		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			var handler = PropertyChanged;
			if(handler != null)
				handler(this, new PropertyChangedEventArgs(propertyName));
		}

		public void CopyPropertyChangedHandlerTo(NotifyPropertyChanged otherInstance)
		{
			// Don't worry, these are immutable :)
			// "You don't need to worry about that. The EventHandler<EventArgs> object is immutable so any change in the list of listeners in either object will cause that object to get a new EventHandler<EventArgs> instance containing the updated invocation list."
			// http://stackoverflow.com/questions/6296277/c-sharp-clone-eventhandler
			//
			otherInstance.PropertyChanged = this.PropertyChanged;
		}
	}
}
