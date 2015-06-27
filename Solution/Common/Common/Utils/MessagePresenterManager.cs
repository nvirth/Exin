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

			messagePresenter.WriteEvent +=
				s => Dispatcher.Invoke(
					() =>
					{
						var msg = s.FixWpfRtbNewLines();
						richTextBox.AppendText(msg ?? "");
						richTextBox.ScrollToEnd();
					});

			messagePresenter.WriteLineEvent +=
				s => Dispatcher.Invoke(
					() =>
					{
						var msg = s.FixWpfRtbNewLines();
						richTextBox.AppendText(msg ?? "");
						richTextBox.AppendText(WpfRtbNewLine);
						richTextBox.ScrollToEnd();
					});

			messagePresenter.WriteLineSeparatorEvent +=
				() => Dispatcher.Invoke(
					() =>
					{
						richTextBox.AppendText("------------------");
						richTextBox.AppendText(WpfRtbNewLine);
						richTextBox.ScrollToEnd();
					});

			messagePresenter.WriteExceptionEvent +=
				e => Dispatcher.Invoke(() => WriteLogRed_Wpf_RichTextbox(e.Message, richTextBox));

			messagePresenter.WriteErrorEvent +=
				s => Dispatcher.Invoke(() => WriteLogRed_Wpf_RichTextbox(s, richTextBox));
		}

		private static void WriteLogRed_Wpf_RichTextbox(string msg, RichTextBoxWpf richTextBox)
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

		public static void WireToWinFormsRichTextBox(RichTextBoxWinForms richTextBox, MessagePresenter messagePresenter = null)
		{
			FetchMessagePresenter(ref messagePresenter);

			messagePresenter.WriteEvent +=
				s => richTextBox.Invoke(
					(Action)(() =>
					{
						richTextBox.AppendText(s ?? "");
						richTextBox.ScrollToCaret();
					}));

			messagePresenter.WriteLineEvent +=
				s => richTextBox.Invoke(
					(Action)(() =>
					{
						richTextBox.AppendText(s ?? "");
						richTextBox.AppendText(Environment.NewLine);
						richTextBox.ScrollToCaret();
					}));

			messagePresenter.WriteLineSeparatorEvent +=
				() => richTextBox.Invoke(
					(Action)(() =>
					{
						richTextBox.AppendText("------------------");
						richTextBox.AppendText(Environment.NewLine);
						richTextBox.ScrollToCaret();
					}));

			messagePresenter.WriteExceptionEvent +=
				e => richTextBox.Invoke((Action)(() => WriteLogRed_WinForms_RichTextbox(e.Message, richTextBox)));

			messagePresenter.WriteErrorEvent +=
				s => richTextBox.Invoke((Action)(() => WriteLogRed_WinForms_RichTextbox(s, richTextBox)));
		}

		private static void WriteLogRed_WinForms_RichTextbox(string msg, RichTextBoxWinForms richTextBox)
		{
			msg = msg ?? "";

			int length = richTextBox.TextLength;  // at end of text
			richTextBox.AppendText(msg);
			richTextBox.SelectionStart = length;
			richTextBox.SelectionLength = msg.Length;
			richTextBox.SelectionColor = Color.Red;

			richTextBox.AppendText(Environment.NewLine);
			richTextBox.ScrollToCaret();
		}

		#endregion

	}
}
