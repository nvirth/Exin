﻿namespace Exin.Common.Logging.Core
{
	public class LogConfig
	{
		public const int JSON = 0;
		public const int XML = 1;

        public static int LogDataMode => JSON;

		// TODO log setup from xml: logDataFormat, logLevel, logPurgeInterval
	}
}
