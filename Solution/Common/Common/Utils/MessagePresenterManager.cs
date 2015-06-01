using System;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;
using H = Common.Utils.Helpers.Helpers;

namespace Common.Utils
{
	public static class MessagePresenterManager
	{
		#region Console

		public static void WireToConsole()
		{
			MessagePresenter.WriteEvent += Console.Write;
			MessagePresenter.WriteLineEvent += Console.WriteLine;
			MessagePresenter.WriteLineSeparatorEvent += WriteLineSeparatorConsoleMethod();
			MessagePresenter.WriteExceptionEvent += H.WriteWithInnerMessagesRed;
		}

		public static void DetachFromConsole()
		{
			MessagePresenter.WriteEvent -= Console.Write;
			MessagePresenter.WriteLineEvent -= Console.WriteLine;
			MessagePresenter.WriteLineSeparatorEvent -= WriteLineSeparatorConsoleMethod();
			MessagePresenter.WriteExceptionEvent -= H.WriteWithInnerMessagesRed;
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

		public static void WireToRichTextBox(RichTextBox richTextBox, Dispatcher Dispatcher)
		{
			MessagePresenter.WriteEvent +=
				s => Dispatcher.Invoke(
					() =>
					{
						var msg = s.FixWpfRtbNewLines();
						richTextBox.AppendText(msg ?? "");
						richTextBox.ScrollToEnd();
					});

			MessagePresenter.WriteLineEvent +=
				s => Dispatcher.Invoke(
					() =>
					{
						var msg = s.FixWpfRtbNewLines();
						richTextBox.AppendText(msg ?? "");
						richTextBox.AppendText(WpfRtbNewLine);
						richTextBox.ScrollToEnd();
					});

			MessagePresenter.WriteLineSeparatorEvent +=
				() => Dispatcher.Invoke(
					() =>
					{
						richTextBox.AppendText("------------------");
						richTextBox.AppendText(WpfRtbNewLine);
						richTextBox.ScrollToEnd();
					});

			MessagePresenter.WriteExceptionEvent +=
				e => Dispatcher.Invoke(() => WriteLogRed_Wpf_RichTextbox(e.Message, richTextBox));

			MessagePresenter.WriteErrorEvent +=
				s => Dispatcher.Invoke(() => WriteLogRed_Wpf_RichTextbox(s, richTextBox));
		}

		private static void WriteLogRed_Wpf_RichTextbox(string msg, RichTextBox richTextBox)
		{
			msg = msg.FixWpfRtbNewLines();

			var textRange = new TextRange(richTextBox.Document.ContentEnd, richTextBox.Document.ContentEnd);
			textRange.Text = msg;
			textRange.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Red);

			textRange = new TextRange(richTextBox.Document.ContentEnd, richTextBox.Document.ContentEnd);
			textRange.Text = " ";
			textRange.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Black);

			richTextBox.AppendText(WpfRtbNewLine);
			richTextBox.ScrollToEnd();
		}

		#endregion

		#region RichTextBox (WinForms)

		public static void WireToWinFormsRichTextBox(System.Windows.Forms.RichTextBox richTextBox)
		{
			MessagePresenter.WriteEvent +=
				s => richTextBox.Invoke(
					(Action)(() =>
					{
						richTextBox.AppendText(s ?? "");
						richTextBox.ScrollToCaret();
					}));

			MessagePresenter.WriteLineEvent +=
				s => richTextBox.Invoke(
					(Action)(() =>
					{
						richTextBox.AppendText(s ?? "");
						richTextBox.AppendText(Environment.NewLine);
						richTextBox.ScrollToCaret();
					}));

			MessagePresenter.WriteLineSeparatorEvent +=
				() => richTextBox.Invoke(
					(Action)(() =>
					{
						richTextBox.AppendText("------------------");
						richTextBox.AppendText(Environment.NewLine);
						richTextBox.ScrollToCaret();
					}));

			MessagePresenter.WriteExceptionEvent +=
				e => richTextBox.Invoke((Action)(() => WriteLogRed_WinForms_RichTextbox(e.Message, richTextBox)));

			MessagePresenter.WriteErrorEvent +=
				s => richTextBox.Invoke((Action)(() => WriteLogRed_WinForms_RichTextbox(s, richTextBox)));
		}

		private static void WriteLogRed_WinForms_RichTextbox(string msg, System.Windows.Forms.RichTextBox richTextBox)
		{
			msg = msg ?? "";

			int length = richTextBox.TextLength;  // at end of text
			richTextBox.AppendText(msg);
			richTextBox.SelectionStart = length;
			richTextBox.SelectionLength = msg.Length;
			richTextBox.SelectionColor = System.Drawing.Color.Red;

			richTextBox.AppendText(Environment.NewLine);
			richTextBox.ScrollToCaret();
		}

		#endregion

	}
}
