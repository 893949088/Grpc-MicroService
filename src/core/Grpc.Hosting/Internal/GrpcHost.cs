using Grpc.Core;
using Grpc.Server;
using Grpc.Hosting.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Grpc.Core.Interceptors;

namespace Grpc.Hosting.Internal
{
    public class GrpcHost : IGrpcHost
    {
        private readonly Grpc.Core.Server _server;
        private readonly IServiceCollection _applicationServiceCollection;
        private IStartup _startup;

        private readonly IServiceProvider _hostingServiceProvider;
        private readonly GrpcHostOptions _options;
        private readonly IConfiguration _config;

        private IServiceProvider _applicationServices;
        private ILogger<GrpcHost> _logger;

        public GrpcHost(
            IServiceCollection appServices,
            IServiceProvider hostingServiceProvider,
            GrpcHostOptions options,
            IConfiguration config)
        {
            _options = options;
            _config = config;
            _server = new Grpc.Core.Server(options.ChannelOptions);

            _applicationServiceCollection = appServices;
            _hostingServiceProvider = hostingServiceProvider;
        }

        public IServiceProvider Services
        {
            get
            {
                return _applicationServices;
            }
        }

        public void Run()
        {

            var application = BuildApplication();

            foreach (var port in _options.Ports)
            {
                _server.Ports.Add(port);
            }

            foreach (var service in application.BuildServiceDefinition())
            {
                _server.Services.Add(service.Intercept(application.ServiceInterceptors.ToArray()));
            }

            _server.Start();

            _server.ShutdownTask.Wait();

        }

        public void Initialize()
        {
            try
            {
                EnsureApplicationServices();
            }
            catch (Exception ex)
            {
                // EnsureApplicationServices may have failed due to a missing or throwing Startup class.
                if (_applicationServices == null)
                {
                    _applicationServices = _applicationServiceCollection.BuildServiceProvider();
                }
            }
        }



        private void EnsureApplicationServices()
        {
            if (_applicationServices == null)
            {
                EnsureStartup();
                _applicationServices = _startup.ConfigureServices(_applicationServiceCollection);
            }
        }

        private void EnsureStartup()
        {
            if (_startup != null)
            {
                return;
            }

            _startup = _hostingServiceProvider.GetService<IStartup>();

            if (_startup == null)
            {
                throw new InvalidOperationException($"No startup configured. Please specify startup via WebHostBuilder.UseStartup, WebHostBuilder.Configure, injecting {nameof(IStartup)} or specifying the startup assembly via {nameof(GrpcHostDefaults.StartupAssemblyKey)} in the web host configuration.");
            }
        }

        private IGrpcServer BuildApplication()
        {

            var builderFactory = _applicationServices.GetRequiredService<IGrpcServerFactory>();
            var application = builderFactory.CreateApplication();
            
            var startupFilters = _applicationServices.GetService<IEnumerable<IStartupFilter>>();
            Action<IGrpcServer> configure = _startup.Configure;
            foreach (var filter in startupFilters.Reverse())
            {
                configure = filter.Configure(configure);
            }

            configure(application);

            return application;
        }
    }
}
