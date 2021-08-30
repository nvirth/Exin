Repro
-----

In `MainSettings.xml`, set `<UiLoggerLevel>Debug</UiLoggerLevel>`

--> Maybe the Debug log (running process count) and the Info log (saving the summaries, months) want to be written into the RichTextBox at the same time!

Exception while debugging
-------------------------

Managed Debugging Assistant 'FatalExecutionEngineError' has detected a problem in 'C:\Norbi\Development\Exin\Solution\WPF\bin\Release\Exin.vshost.exe'.

Additional information: The runtime has encountered a fatal error. The address of the error was at 0x5734a9e0, on thread 0x3470. The error code is 0x80131623. This error may be a bug in the CLR or in the unsafe or non-verifiable portions of user code. Common sources of this bug include user marshaling errors for COM-interop or PInvoke, which may corrupt the stack.


Event log
---------

Application: Exin.exe
Framework Version: v4.0.30319
Description: The application requested process termination through System.Environment.FailFast(string message).
Message: Unrecoverable system error.
Stack:
   at System.Environment.FailFast(System.String)
   at MS.Internal.Invariant.FailFast(System.String, System.String)
   at System.Windows.Documents.SplayTreeNode.get_Role()
   at System.Windows.Documents.SplayTreeNode.RotateLeft()
   at System.Windows.Documents.SplayTreeNode.Splay()
   at System.Windows.Documents.TextPointer.GetPointerContextBackward(System.Windows.Documents.TextTreeNode, System.Windows.Documents.ElementEdge)
   at System.Windows.Documents.TextPointer.GetPointerContext(System.Windows.Documents.LogicalDirection)
   at System.Windows.Documents.TextPointer.System.Windows.Documents.ITextPointer.GetPointerContext(System.Windows.Documents.LogicalDirection)
   at System.Windows.Documents.TextPointerBase.IsAtCaretUnitBoundary(System.Windows.Documents.ITextPointer)
   at System.Windows.Documents.TextPointerBase.IsAtNormalizedPosition(System.Windows.Documents.ITextPointer, Boolean)
   at System.Windows.Documents.TextPointerBase.NormalizePosition(System.Windows.Documents.ITextPointer, System.Windows.Documents.LogicalDirection, Boolean)
   at System.Windows.Documents.TextPointer.MoveToInsertionPosition(System.Windows.Documents.LogicalDirection)
   at System.Windows.Documents.TextPointer.System.Windows.Documents.ITextPointer.MoveToInsertionPosition(System.Windows.Documents.LogicalDirection)
   at System.Windows.Documents.TextPointer.System.Windows.Documents.ITextPointer.GetInsertionPosition(System.Windows.Documents.LogicalDirection)
   at System.Windows.Documents.TextRangeBase.GetNormalizedPosition(System.Windows.Documents.ITextRange, System.Windows.Documents.ITextPointer, System.Windows.Documents.LogicalDirection)
   at System.Windows.Documents.TextRangeBase.CreateNormalizedTextSegment(System.Windows.Documents.ITextRange, System.Windows.Documents.ITextPointer, System.Windows.Documents.ITextPointer)
   at System.Windows.Documents.TextRangeBase.SelectPrivate(System.Windows.Documents.ITextRange, System.Windows.Documents.ITextPointer, System.Windows.Documents.ITextPointer, Boolean, Boolean)
   at System.Windows.Documents.TextRangeBase.Select(System.Windows.Documents.ITextRange, System.Windows.Documents.ITextPointer, System.Windows.Documents.ITextPointer, Boolean)
   at System.Windows.Documents.TextRange..ctor(System.Windows.Documents.ITextPointer, System.Windows.Documents.ITextPointer, Boolean)
   at System.Windows.Documents.TextRange..ctor(System.Windows.Documents.TextPointer, System.Windows.Documents.TextPointer)
   at Common.Utils.MessagePresenterManager.AppendToWpfRichTextbox(System.String, System.Windows.Controls.RichTextBox, System.Windows.Media.Brush, Boolean, Boolean)
   at Exin.Common.Logging.CommonLogging.Loggers.WpfRichTextBoxLogger.WriteInternal(Common.Logging.LogLevel, System.Object, System.Exception)
   at Exin.Common.Logging.CommonLogging.Loggers.Base.AbstractSimpleLoggerBase.Write(Common.Logging.LogLevel, System.Object, System.Exception)
   at Exin.Common.Logging.CommonLogging.Loggers.AggregateLogger+<>c__DisplayClass9_0.<DoLog>b__1(Exin.Common.Logging.Core.IExinLog)
   at Common.Utils.Helpers.Helpers.ForEach[[System.__Canon, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]](System.Collections.Generic.IEnumerable`1<System.__Canon>, System.Action`1<System.__Canon>)
   at Exin.Common.Logging.CommonLogging.Loggers.AggregateLogger.DoLog(Exin.Common.Logging.Core.LogData)
   at Exin.Common.Logging.Core.Log.DoLog(System.String, System.Object, System.Exception, Exin.Common.Logging.Core.LogTarget, Common.Logging.LogLevel)
   at Exin.Common.Logging.Core.Log.DoLog(System.Type, System.Object, System.Exception, Exin.Common.Logging.Core.LogTarget, Common.Logging.LogLevel, System.String)
   at Exin.Common.Logging.Core.Log.Debug(System.Type, System.Func`2<Exin.Common.Logging.Core.MessageFormatterLocalizedHandler,System.String>, Exin.Common.Logging.Core.LogTarget, System.Exception, System.String)
   at Common.Utils.TaskManager+<>c__DisplayClass3_0.<AfterTaskStarted>b__1(System.Threading.Tasks.Task)
   at System.Threading.Tasks.ContinuationTaskFromTask.InnerInvoke()
   at System.Threading.Tasks.Task.Execute()
   at System.Threading.Tasks.Task.ExecutionContextCallback(System.Object)
   at System.Threading.ExecutionContext.RunInternal(System.Threading.ExecutionContext, System.Threading.ContextCallback, System.Object, Boolean)
   at System.Threading.ExecutionContext.Run(System.Threading.ExecutionContext, System.Threading.ContextCallback, System.Object, Boolean)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(System.Threading.Tasks.Task ByRef)
   at System.Threading.Tasks.Task.ExecuteEntry(Boolean)
   at System.Threading.Tasks.Task.System.Threading.IThreadPoolWorkItem.ExecuteWorkItem()
   at System.Threading.ThreadPoolWorkQueue.Dispatch()
   at System.Threading._ThreadPoolWaitCallback.PerformWaitCallback()
