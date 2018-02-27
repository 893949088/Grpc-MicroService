using System;
using System.Collections.Generic;
using System.Text;

namespace Grpc.Hosting
{
    public interface IHostingEnvironment
    {
        string EnvironmentName { get; set; }

        string ApplicationName { get; set; }

        string RootPath { get; set; }

    }
}
