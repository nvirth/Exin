namespace Common.Log
{
	public class Config
	{
		public const int JSON = 0;
		public const int XML = 1;

        public static int LogDataMode => JSON;
	}
}
