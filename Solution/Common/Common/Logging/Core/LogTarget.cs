using System;

namespace Exin.Common.Logging.Core
{
	[Flags]
	public enum LogTarget
	{
		//None = 0,
		Ui = 1,
		Log = 2,
		All = 3,
	}
}