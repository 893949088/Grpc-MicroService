using Grpc.Core;
using Grpc.Hosting;
using System;

namespace ExampleServer
{
    class Program
    {
        static void Main(string[] args)
        {
            new GrpcHostBuilder()
                .ApplicationName("ExampleService")
                .UseStartup<Startup>()
                .BindPort("0.0.0.0", 1121, ServerCredentials.Insecure)
                .Build()
                .Run();
        }
    }
}
