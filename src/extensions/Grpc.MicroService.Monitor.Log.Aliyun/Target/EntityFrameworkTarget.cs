using Aliyun.Api.LOG;
using Aliyun.Api.LOG.Data;
using Aliyun.Api.LOG.Request;
using Newtonsoft.Json;
using NLog;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Grpc.MicroService.Log
{
    public class EntityFrameworkTarget : TargetWithLayout
    {
        private readonly LogClient _aliyunLogClient;

        public EntityFrameworkTarget(string endpoint, string accessKeyId, string accessKeySecret)
        {
            _aliyunLogClient = new LogClient(endpoint, accessKeyId, accessKeySecret);
        }


        protected override void Write(LogEventInfo logEvent)
        {

            var req = new PutLogsRequest("GrpcService", "EntityFramework")
            {
                LogItems = new List<LogItem>(),
                Topic = logEvent.LoggerName
            };

            req.LogItems.Add(new LogItem()
            {
                Time = GetTimeSpan(),
                Contents = new List<LogContent>
                {
                    new LogContent("Message",logEvent.Message),
                    new LogContent("Time",DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"))
                }
            });

            _aliyunLogClient.PutLogs(req);

        }
        static uint GetTimeSpan()
        {
            DateTime unixTimestampZeroPoint = new DateTime(1970, 01, 01, 0, 0, 0, DateTimeKind.Utc);
            return (uint)((DateTime.UtcNow - unixTimestampZeroPoint).TotalSeconds);
        }
    }
}
