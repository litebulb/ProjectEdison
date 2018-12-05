using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Edison.Tests.Common.Helpers
{
    public class LoggerHelper
    {
        public static Mock<ILogger<T>> CreateLogger<T>()
        {
            var mock = new Mock<ILogger<T>>(MockBehavior.Strict);

            mock.Setup(m => m.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<FormattedLogValues>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<object, Exception, string>>())
                );

            return mock;
        }

        public static bool VerifyDebugLog<T>(string log, Mock<ILogger<T>> mock)
        {
            try
            {
                mock.Verify(m => m.Log(
                LogLevel.Debug,
                It.IsAny<EventId>(),
                It.Is<FormattedLogValues>(v => v.ToString().Contains(log)),
                It.IsAny<Exception>(),
                It.IsAny<Func<object, Exception, string>>())
                );
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool VerifyErrorLog<T>(string log, Mock<ILogger<T>> mock)
        {
            try
            {
                mock.Verify(m => m.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<FormattedLogValues>(v => v.ToString().Contains(log)),
                It.IsAny<Exception>(),
                It.IsAny<Func<object, Exception, string>>())
                );
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
