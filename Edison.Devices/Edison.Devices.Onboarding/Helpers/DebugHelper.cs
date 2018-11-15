using System;
using System.Diagnostics;
using Windows.Foundation.Diagnostics;

namespace Edison.Devices.Onboarding.Helpers
{
    internal class DebugHelper
    {
        private static LoggingChannel _logging;

        public static void Init()
        {
            _logging = new LoggingChannel("Edison Onboarding", null, new Guid("2c2799f8-9bcf-48b5-ac2d-f40735112345"));
        }

        public static void LogVerbose(string message)
        {
            Debug.WriteLine(message);
            _logging.LogMessage(message, LoggingLevel.Verbose);
        }

        public static void LogInformation(string message)
        {
            Debug.WriteLine(message);
            _logging.LogMessage(message, LoggingLevel.Information);
        }

        public static void LogWarning(string message)
        {
            Debug.WriteLine(message);
            _logging.LogMessage(message, LoggingLevel.Warning);
        }

        public static void LogError(string message)
        {
            Debug.WriteLine(message);
            _logging.LogMessage(message, LoggingLevel.Error);
        }

        public static void LogCritical(string message)
        {
            Debug.WriteLine(message);
            _logging.LogMessage(message, LoggingLevel.Critical);
        }
    }
}
