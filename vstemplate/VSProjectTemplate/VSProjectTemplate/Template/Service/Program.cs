using Grpc.Core;
using Grpc.Hosting;
using System;

namespace $ext_safeprojectname$.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            new GrpcHostBuilder()
                .ApplicationName("$ext_safeprojectname$Service")
                .UseStartup<Startup>()
                .BindPort("127.0.0.1", 1121, ServerCredentials.Insecure)
                .Build()
                .Run();
        }
    }
}
