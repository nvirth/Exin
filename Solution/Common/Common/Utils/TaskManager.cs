using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Exin.Common.Logging;
using Exin.Common.Logging.Core;

namespace Common.Utils
{
	public static class TaskManager
	{
		private static int RunningTaskCount = 0;
		private static HashSet<Task> RunninTasks = new HashSet<Task>();
		private static object Lock = new object();

		private static void AfterTaskStarted(Task task)
		{
			lock(Lock)
			{
				RunningTaskCount++;
				RunninTasks.Add(task);
				ExinLog.ger.LogInfo("RunningTaskCount: " + RunningTaskCount);
			}

			task.ContinueWith(__task => { // AfterTaskFinished
				lock (Lock)
				{
					RunningTaskCount--;
					RunninTasks.Remove(task);
					ExinLog.ger.LogInfo("RunningTaskCount: " + RunningTaskCount);
				}
			});
		}

		public async static Task WaitBackgroundTasks()
		{
			await Task.Run(() => {
				// Method 1
				//while(true)
				//{
				//	if(RunningTaskCount == 0)
				//		break;

				//	Thread.Sleep(500);
				//}

				// Method 2
				Task[] runningTasks;
				lock (Lock)
				{
					runningTasks = new Task[RunninTasks.Count];
					RunninTasks.CopyTo(runningTasks);
				}

				foreach(var runningTask in runningTasks)
				{
					runningTask.Wait();
				}
			});
		}

		#region Task static Delegated members

		public static Task Run(Action action)
		{
			var result = Task.Run(action);
			AfterTaskStarted(result);
			return result;
		}
		public static Task Run(Action action, CancellationToken cancellationToken)
		{
			var result = Task.Run(action, cancellationToken);
			AfterTaskStarted(result);
			return result;
		}
		public static Task<TResult> Run<TResult>(Func<TResult> function)
		{
			var result = Task.Run(function);
			AfterTaskStarted(result);
			return result;
		}
		public static Task<TResult> Run<TResult>(Func<TResult> function, CancellationToken cancellationToken)
		{
			var result = Task.Run(function, cancellationToken);
			AfterTaskStarted(result);
			return result;
		}
		public static Task Run(Func<Task> function)
		{
			var result = Task.Run(function);
			AfterTaskStarted(result);
			return result;
		}
		public static Task Run(Func<Task> function, CancellationToken cancellationToken)
		{
			var result = Task.Run(function, cancellationToken);
			AfterTaskStarted(result);
			return result;
		}
		public static Task<TResult> Run<TResult>(Func<Task<TResult>> function)
		{
			var result = Task.Run(function);
			AfterTaskStarted(result);
			return result;
		}
		public static Task<TResult> Run<TResult>(Func<Task<TResult>> function, CancellationToken cancellationToken)
		{
			var result = Task.Run(function, cancellationToken);
			AfterTaskStarted(result);
			return result;
		}

		#endregion

	}
}
