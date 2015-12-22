using System;

namespace Exin.Common.Logging.Core
{
    /// These kind of Exceptions are not meant to be thrown.
    /// Their purpose is only logging the actual StackTrace; telling the logger system to do that.
    public class ForStackTraceException : Exception
    {
        public ForStackTraceException() : base("Exception for StackTrace") { }
    }
}