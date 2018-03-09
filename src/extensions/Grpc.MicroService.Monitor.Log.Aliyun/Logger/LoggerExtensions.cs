using Grpc.MicroService.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.Logging
{
    public static class LoggerExtensions
    {

        public static void LogInformation(this ILogger logger, string project, string logstore, string topic, Dictionary<string, string> content)
        {
            logger.LogInformation(JsonConvert.SerializeObject(new AliyunLogModel
            {
                Content = content,
                Project = project,
                LogStore = logstore,
                Topic = topic
            }));
        }

        public static void LogError(this ILogger logger, string project, string logstore, string topic, Exception exception)
        {
            //logger.LogError(exception.ToString(), args: new object[] { project, logstore, topic });
        }
    }
}
