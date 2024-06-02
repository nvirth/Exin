using Exin.Common.Logging.Core;
using Localization;
using System;
using System.Windows;

namespace WPF.Utils
{
	public static class ClipboardExtensions
	{
		public static void SetText(string text, TextDataFormat format = TextDataFormat.UnicodeText)
		{
			try
			{
				Clipboard.SetText(text, format);
			}
			catch(Exception e)
			{
				Log.Error(typeof(ClipboardExtensions),
					m => m(Localized.ResourceManager, LocalizedKeys.Could_not_copy_to_clipboard),
					LogTarget.All,
					e
				);
			}
		}
	}
}
