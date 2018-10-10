using System;
using System.Collections.Generic;

namespace Edison.Mobile.Common.Logging
{
    public enum LogLevel 
    {
        Info,
        Warning,
        Error,
    }

    public interface ILogger
    {
        void Log(string eventName, Dictionary<string, string> properties, LogLevel logLevel = LogLevel.Info);
        void Log(Exception exception, string message = "", LogLevel logLevel = LogLevel.Error);
        void Log(string message, LogLevel logLevel = LogLevel.Info);
    }
}
