using System;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;
using Color = System.Drawing.Color;
using H = Common.Utils.Helpers.Helpers;
using RichTextBoxWpf = System.Windows.Controls.RichTextBox;
using RichTextBoxWinForms = System.Windows.Forms.RichTextBox;

namespace Common.Utils
{
	public static class MessagePresenterManager
	{
		public static void FetchMessagePresenter(ref MessagePresenter messagePresenter)
		{
			messagePresenter = messagePresenter ?? MessagePresenter.Instance;
		}

		#region Console

		public static void WireToConsole(MessagePresenter messagePresenter = null)
		{
			FetchMessagePresenter(ref messagePresenter);

			messagePresenter.WriteEvent += Console.Write;
			messagePresenter.WriteErrorEvent += H.WriteToConsoleRed;
			messagePresenter.WriteLineEvent += Console.WriteLine;
			messagePresenter.WriteLineSeparatorEvent += WriteLineSeparatorConsoleMethod();
			messagePresenter.WriteExceptionEvent += H.WriteWithInnerMessagesRed;
		}

		public static void DetachFromConsole(MessagePresenter messagePresenter = null)
		{
			FetchMessagePresenter(ref messagePresenter);

			messagePresenter.WriteEvent -= Console.Write;
			messagePresenter.WriteErrorEvent -= H.WriteToConsoleRed;
			messagePresenter.WriteLineEvent -= Console.WriteLine;
			messagePresenter.WriteLineSeparatorEvent -= WriteLineSeparatorConsoleMethod();
			messagePresenter.WriteExceptionEvent -= H.WriteWithInnerMessagesRed;
		}

		private static Action WriteLineSeparatorConsoleMethod()
		{
			return () => Console.WriteLine("--------------------------");
		}

		#endregion

		#region RichTextBox (WPF)

		private const string WpfRtbNewLine = "\r"; // Environment.NewLine causes \r\n (wich is displayed as 2 new row)

		private static string FixWpfRtbNewLines(this string s)
		{
			s = s ?? "";
			var res = s.Replace(Environment.NewLine, WpfRtbNewLine);
			return res;
		}

		public static void WireToRichTextBox(RichTextBoxWpf richTextBox, Dispatcher Dispatcher, MessagePresenter messagePresenter = null)
		{
			FetchMessagePresenter(ref messagePresenter);

			messagePresenter.WriteEvent += s => Dispatcher.Invoke(() => AppendToWpfRichTextbox(s, richTextBox));
			messagePresenter.WriteLineEvent += s => Dispatcher.Invoke(() => AppendToWpfRichTextbox(s, richTextBox, newLineAtEnd: true));
			messagePresenter.WriteLineSeparatorEvent += () => Dispatcher.Invoke(() => AppendToWpfRichTextbox("------------------", richTextBox, newLineAtEnd: true));
			messagePresenter.WriteExceptionEvent += e => Dispatcher.Invoke(() => AppendToWpfRichTextbox(e.Message, richTextBox, Brushes.Red, newLineAtEnd: true));
			messagePresenter.WriteErrorEvent += s => Dispatcher.Invoke(() => AppendToWpfRichTextbox(s, richTextBox, Brushes.Red, newLineAtEnd: true));
		}

		public static void AppendToWpfRichTextbox(string msg, RichTextBoxWpf richTextBox, Brush textColor = null, bool newLineAtEnd = false, bool scrollToEnd = true)
		{
			msg = msg.FixWpfRtbNewLines() ?? "";

			if(textColor == null)
			{
				richTextBox.AppendText(msg);
			}
			else
			{
				var textRange = new TextRange(richTextBox.Document.ContentEnd, richTextBox.Document.ContentEnd);
				var originalTextColor = textRange.GetPropertyValue(TextElement.ForegroundProperty) as Brush;

				textRange.Text = msg;
				textRange.ApplyPropertyValue(TextElement.ForegroundProperty, textColor);

				textRange = new TextRange(richTextBox.Document.ContentEnd, richTextBox.Document.ContentEnd);
				textRange.Text = " ";
				textRange.ApplyPropertyValue(TextElement.ForegroundProperty, originalTextColor ?? Brushes.Black);
			}

			if(newLineAtEnd)
				richTextBox.AppendText(WpfRtbNewLine);

			richTextBox.ScrollToEnd();
		}

		#endregion
	}
}
