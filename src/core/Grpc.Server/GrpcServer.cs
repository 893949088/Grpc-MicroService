using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Server.Internal;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Grpc.Server
{
    public class GrpcServer : IGrpcServer
    {
        public IServiceProvider ApplicationServices { get; set; }

        public IList<Interceptor> ServiceInterceptors { get; }

        public GrpcServer(IServiceProvider serviceProvider)
        {
            this.ApplicationServices = serviceProvider;
            this.ServiceInterceptors = new List<Interceptor>();
        }

        public IEnumerable<ServerServiceDefinition> BuildServiceDefinition()
        {
            var serviceDefinitions = new List<ServerServiceDefinition>();

            var serviceImpls = ApplicationServices.GetServices<IGrpcService>();
            if (serviceImpls == null || !serviceImpls.Any())
            {
                Console.WriteLine("Cannot Resolve GrpcService");
            }

            foreach (var serviceImpl in serviceImpls)
            {
                var serviceType = serviceImpl.GetType();
                var serviceBaseType = serviceType.BaseType.ReflectedType;
                var bindMethod = serviceBaseType.GetMethod("BindService", BindingFlags.Public | BindingFlags.Static);

                var serviceDefinition = bindMethod.Invoke(null, new object[] { serviceImpl }) as ServerServiceDefinition;
                serviceDefinitions.Add(serviceDefinition.Intercept(new ServerMethodInterceptor(serviceType, this.ApplicationServices)));
            }

            return serviceDefinitions;
        }

        public IGrpcServer UseInterceptor(Interceptor interceptor)
        {
            ServiceInterceptors.Add(interceptor);
            return this;
        }

        #region private


        #endregion
    }
}
