using System;
using System.Collections.Generic;
using Edison.Mobile.Common.Logging;

namespace Edison.Mobile.iOS.Common.Logging
{
    public class PlatformLogger : BasePlatformLogger
    {
        public override void Log(string message, LogLevel logLevel = LogLevel.Info)
        {

        }

        public override void Log(Exception exception, string message = "", LogLevel logLevel = LogLevel.Error)
        {

        }

        public override void Log(string eventName, Dictionary<string, string> properties, LogLevel logLevel = LogLevel.Info)
        {

        }
    }
}
