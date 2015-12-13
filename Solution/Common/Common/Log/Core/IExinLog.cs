using System;
using Common.Logging;

namespace Common.Log.Core
{
    public interface IExinLog : ILog
    {
        void Write(LogLevel logLevel, object message, Exception exception);
	    bool IsLevelEnabled(LogLevel level);
    }
}