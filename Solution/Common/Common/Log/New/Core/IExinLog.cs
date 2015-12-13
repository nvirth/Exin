using System;
using Common.Logging;

namespace Common.Log.New.Core
{
    public interface IExinLog : ILog
    {
        void Write(LogLevel logLevel, object message, Exception exception);
	    bool IsLevelEnabled(LogLevel level);
    }
}