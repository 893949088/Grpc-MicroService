using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grpc.Hosting.Internal
{
    public class GrpcHostOptions
    {
        public string ApplicationName { get; set; }

        public List<ServerPort> Ports { get; set; }

        public IEnumerable<ChannelOption> ChannelOptions { get; set; }

        public GrpcHostOptions()
        {
            this.Ports = new List<ServerPort>();
        }
    }
}
