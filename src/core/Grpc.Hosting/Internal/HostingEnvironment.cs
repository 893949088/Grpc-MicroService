using System;
using System.Collections.Generic;
using System.Text;

namespace Grpc.Hosting.Internal
{
    public class HostingEnvironment: IHostingEnvironment
    {
        public string EnvironmentName { get; set; } = Grpc.Hosting.EnvironmentName.Production;

        public string ApplicationName { get; set; }

        public string RootPath { get; set; }

        public void Initialize(string rootPath,GrpcHostOptions options)
        {
            this.RootPath = rootPath;
        }
    }
}
