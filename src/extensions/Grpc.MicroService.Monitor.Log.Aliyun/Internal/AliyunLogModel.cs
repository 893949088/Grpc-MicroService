using System;
using System.Collections.Generic;
using System.Text;

namespace Grpc.MicroService.Internal
{
    internal class AliyunLogModel
    {
        public string Project { get; set; }

        public string LogStore { get; set; }

        public string Topic { get; set; }

        public Dictionary<string,string> Content { get; set; }
    }
}
