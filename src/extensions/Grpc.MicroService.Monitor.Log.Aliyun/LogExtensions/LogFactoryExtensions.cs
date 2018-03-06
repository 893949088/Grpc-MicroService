using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.Logging
{
    public static class LogFactoryExtensions
    {

        public static ILogger CreateAliyunLogger(this ILoggerFactory loggerFactory)
        {
            return loggerFactory.CreateLogger("AliyunLogger");
        }
    }
}
