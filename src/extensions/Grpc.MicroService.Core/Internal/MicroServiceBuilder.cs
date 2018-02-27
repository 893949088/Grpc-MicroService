using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grpc.MicroService.Internal
{
    public class MicroServiceBuilder : IMicroServiceBuilder
    {

        public MicroServiceBuilder(IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            Services = services;
        }

        public IServiceCollection Services { get; }
    }
}
