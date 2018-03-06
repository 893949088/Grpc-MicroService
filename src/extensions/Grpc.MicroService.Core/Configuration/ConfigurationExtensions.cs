using Grpc.MicroService.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.Configuration
{
    public static class ConfigurationExtensions
    {

        public static IMicroServiceConfiguration MicroService(this IConfiguration configuration)
        {
            return new MicroServiceConfiguration(configuration.GetSection("MicroServiceConfig"));
        }
    }
    
}
