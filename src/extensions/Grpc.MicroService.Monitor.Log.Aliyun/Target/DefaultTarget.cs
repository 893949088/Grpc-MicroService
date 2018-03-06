using Aliyun.Api.LOG;
using Aliyun.Api.LOG.Data;
using Aliyun.Api.LOG.Request;
using Grpc.MicroService.Internal;
using Newtonsoft.Json;
using NLog;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Grpc.MicroService.Log
{
    public class DefaultTarget : TargetWithLayout
    {
        private readonly LogClient _aliyunLogClient;

        public DefaultTarget(string endpoint, string accessKeyId, string accessKeySecret)
        {
            _aliyunLogClient = new LogClient(endpoint, accessKeyId, accessKeySecret);
        }

        protected override void Write(LogEventInfo logEvent)
        {
            var logmodel= JsonConvert.DeserializeObject<AliyunLogModel>(logEvent.Message);
            var content = logmodel.Content;
            var project = logmodel.Project;
            var logstore = logmodel.LogStore;
            var topic = logmodel.Topic;

            var req = new PutLogsRequest(project, logstore)
            {
                LogItems = new List<LogItem>(),
                Topic = topic
            };

            req.LogItems.Add(new LogItem()
            {
                Time = GetTimeSpan(),
                Contents = content.Select(p => new LogContent(p.Key, p.Value)).ToList()
            });

            req.LogItems[0].Contents = req.LogItems[0].Contents.Where(p => !string.IsNullOrWhiteSpace(p.Value)).ToList();
            _aliyunLogClient.PutLogs(req);

        }

        static uint GetTimeSpan()
        {
            DateTime unixTimestampZeroPoint = new DateTime(1970, 01, 01, 0, 0, 0, DateTimeKind.Utc);
            return (uint)((DateTime.UtcNow - unixTimestampZeroPoint).TotalSeconds);
        }
    }
}
