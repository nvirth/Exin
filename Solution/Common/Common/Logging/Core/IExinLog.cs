using System;
using Common.Logging;

namespace Exin.Common.Logging.Core
{
    public interface IExinLog : ILog
    {
        void Write(LogLevel logLevel, object message, Exception exception);
	    bool IsLevelEnabled(LogLevel level);
    }
}