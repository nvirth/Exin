using System.Collections.Generic;
using Common.Log.Core;

namespace Common.Log.CommonLogging
{
	public struct LoggerInstancesArgs
	{
		public IList<IExinLog> UiLoggers { get; set; }
		public IList<IExinLog> LogLoggers { get; set; }
	}
}