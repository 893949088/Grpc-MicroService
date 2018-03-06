using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.Configuration
{
    public interface IMicroServiceConfiguration
    {

        IConfigurationSection Section { get; }

        string GetConnectionString(string name);
    }
}
