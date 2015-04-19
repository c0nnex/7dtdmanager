using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DTDManager.Interfaces
{
    public interface ILogger
    {
        //
        // Summary:
        //     Gets a value indicating whether logging is enabled for the Debug level.
        //
        // Returns:
        //     A value of true if logging is enabled for the Debug level, otherwise it returns
        //     false.
         bool IsDebugEnabled { get; }
        //
        // Summary:
        //     Gets a value indicating whether logging is enabled for the Error level.
        //
        // Returns:
        //     A value of true if logging is enabled for the Error level, otherwise it returns
        //     false.
         bool IsErrorEnabled { get; }
        //
        // Summary:
        //     Gets a value indicating whether logging is enabled for the Fatal level.
        //
        // Returns:
        //     A value of true if logging is enabled for the Fatal level, otherwise it returns
        //     false.
         bool IsFatalEnabled { get; }
        //
        // Summary:
        //     Gets a value indicating whether logging is enabled for the Info level.
        //
        // Returns:
        //     A value of true if logging is enabled for the Info level, otherwise it returns
        //     false.
         bool IsInfoEnabled { get; }
        //
        // Summary:
        //     Gets a value indicating whether logging is enabled for the Trace level.
        //
        // Returns:
        //     A value of true if logging is enabled for the Trace level, otherwise it returns
        //     false.
         bool IsTraceEnabled { get; }
        //
        // Summary:
        //     Gets a value indicating whether logging is enabled for the Warn level.
        //
        // Returns:
        //     A value of true if logging is enabled for the Warn level, otherwise it returns
        //     false.
         bool IsWarnEnabled { get; }

        void Trace(string message, params object[] args);
        void Debug(string message, params object[] args);
        void Info(string message, params object[] args);
        void Warn(string message, params object[] args);
        void Error(string message, params object[] args);
    }
}
