using System;

namespace Common.Log.New.Core
{
    /// <summary>
    ///     These kind of Exceptions are not meant to be thrown; their purpose is only for saving the actual StackTrace.
    /// </summary>
    public class ForStackTraceException : Exception
    {
        #region Constructors
        public ForStackTraceException() : base("Exception for StackTrace") { }
        #endregion
    }
}