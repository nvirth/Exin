namespace Exin.Common.Logging.Core
{
	public static class ExinLog
	{
		public static ExinLogger ger { get; private set; }

		static ExinLog()
		{
			ger = new ExinLogger("Exin");
		}
	}
}