namespace Exin.Common.Logging.Core
{
	public struct LogLevels
	{
		// Structs don't have initializers
		// http://stackoverflow.com/questions/333829/why-cant-i-define-a-default-constructor-for-a-struct-in-net
		// That's why the negated property names; for default value will be false
		//
		public bool DoNotLogToUi { get; set; }
		public bool DoNotLogToLog { get; set; }
	}
}