using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grpc.MicroService.Internal
{
    public class MicroServiceConfiguration : IMicroServiceConfiguration
    {
        public IConfigurationSection Section { get; }

        public MicroServiceConfiguration(IConfigurationSection section)
        {
            Section = section;
        }

        public string GetConnectionString(string name)
        {
            return Section.GetSection("ConnectionStrings")?[name];
        }
    }
}
