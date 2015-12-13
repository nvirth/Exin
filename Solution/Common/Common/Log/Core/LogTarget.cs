using System;

namespace Common.Log.Core
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