using System;
using System.Collections.Generic;

namespace Edison.Mobile.Common.Logging
{
    public class CommonLogger : ILogger
    {
        readonly BasePlatformLogger platformLogger;

        public CommonLogger(BasePlatformLogger platformLogger) 
        {
            this.platformLogger = platformLogger;
        }

        public void Log(string message, LogLevel logLevel = LogLevel.Info)
        {
            Console.WriteLine($"{logLevel}: {message}");

            platformLogger.Log(message, logLevel);
        }

        public void Log(Exception exception, string message = "", LogLevel logLevel = LogLevel.Error)
        {
            Console.WriteLine($"{logLevel}: {message}, {exception}");

            platformLogger.Log(exception, message, logLevel);
        }

        public void Log(string eventName, Dictionary<string, string> properties, LogLevel logLevel = LogLevel.Info)
        {
            Console.WriteLine($"Event: {eventName}, {logLevel}");

            platformLogger.Log(eventName, properties, logLevel);
        }
    }
}
