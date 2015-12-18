using System.Collections.Generic;
using Exin.Common.Logging.Core;

namespace Exin.Common.Logging.CommonLogging
{
	public struct LoggerInstancesArgs
	{
		public IList<IExinLog> UiLoggers { get; set; }
		public IList<IExinLog> LogLoggers { get; set; }
	}
}