using Grpc.MicroService.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.Configuration
{
    public static class MicroServiceConfigurationExtensions
    {
        public static IConfigurationSection GetAppMetricsConfig(this IMicroServiceConfiguration configuration)
        {
            return configuration.Section.GetSection("Monitor").GetSection("AppMetrics");
        }
    }
}
