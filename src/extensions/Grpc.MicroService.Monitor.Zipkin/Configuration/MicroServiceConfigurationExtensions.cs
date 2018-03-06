using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.Configuration
{
    public static class MicroServiceConfigurationExtensions
    {
        public static string GetZipkinCollectorUrl(this IMicroServiceConfiguration configuration)
        {
            return configuration.Section.GetSection("Monitor").GetValue<string>("ZipkinCollectorUrl");
        }
    }
}
