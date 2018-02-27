using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grpc.Hosting
{
    public class GrpcHostBuilderContext
    {

        public IConfiguration Configuration { get; set; }
    }
}
