using System;
using System.Collections.Generic;

namespace Edison.Mobile.Common.Logging
{
    public abstract class BasePlatformLogger : ILogger
    {
        public abstract void Log(string message, LogLevel logLevel = LogLevel.Info);

        public abstract void Log(Exception exception, string message = "", LogLevel logLevel = LogLevel.Error);

        public abstract void Log(string eventName, Dictionary<string, string> properties, LogLevel logLevel = LogLevel.Info);
    }
}
