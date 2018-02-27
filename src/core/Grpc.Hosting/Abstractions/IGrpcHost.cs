using System;
using System.Collections.Generic;
using System.Text;

namespace Grpc.Hosting
{
    public interface IGrpcHost
    {

        IServiceProvider Services { get; }

        void Run();


    }
}
