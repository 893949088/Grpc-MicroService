using Grpc.Core;
using Grpc.Hosting.Builder;
using Grpc.Hosting.Internal;
using Grpc.Server;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text;

namespace Grpc.Hosting
{
    public class GrpcHostBuilder : IGrpcHostBuilder
    {
        private readonly HostingEnvironment _hostingEnvironment;
        private readonly List<Action<GrpcHostBuilderContext, IServiceCollection>> _configureServicesDelegates;

        private IConfiguration _config;
        private GrpcHostOptions _options;
        private GrpcHostBuilderContext _context;
        private List<Action<GrpcHostBuilderContext, IConfigurationBuilder>> _configureAppConfigurationBuilderDelegates;

        public GrpcHostBuilder()
        {
            _options = new GrpcHostOptions();
            _hostingEnvironment = new HostingEnvironment();
            _configureServicesDelegates = new List<Action<GrpcHostBuilderContext, IServiceCollection>>();
            _configureAppConfigurationBuilderDelegates = new List<Action<GrpcHostBuilderContext, IConfigurationBuilder>>();

            _config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            _context = new GrpcHostBuilderContext
            {
                Configuration = _config
            };
        }

        public string GetSetting(string key)
        {
            return _config[key];
        }

        public IGrpcHostBuilder UseSetting(string key, string value)
        {
            _config[key] = value;
            return this;
        }

        public IGrpcHostBuilder ConfigureServices(Action<IServiceCollection> configureServices)
        {
            if (configureServices == null)
            {
                throw new ArgumentNullException(nameof(configureServices));
            }

            return ConfigureServices((_, services) => configureServices(services));
        }

        public IGrpcHostBuilder ApplicationName(string name)
        {
            _options.ApplicationName = name;
            return this;
        }

        public IGrpcHostBuilder BindPort(string host, int port, ServerCredentials credentials)
        {
            _options.Ports.Add(new ServerPort(host, port, credentials));
            return this;
        }

        public IGrpcHostBuilder BindService()
        {
            return this;
        }

        public IGrpcHostBuilder BindChannelOptions(params ChannelOption[] options)
        {
            _options.ChannelOptions = options;
            return this;
        }

        public IGrpcHostBuilder ConfigureServices(Action<GrpcHostBuilderContext, IServiceCollection> configureServices)
        {
            if (configureServices == null)
            {
                throw new ArgumentNullException(nameof(configureServices));
            }

            _configureServicesDelegates.Add(configureServices);
            return this;
        }

        public IGrpcHost Build()
        {
            var hostingServices = BuildCommonServices();
            var applicationServices = hostingServices.Clone();
            var hostingServiceProvider = GetProviderFromFactory(hostingServices);

            var host = new GrpcHost(
                applicationServices,
                hostingServiceProvider,
                _options,
                _config);

            host.Initialize();

            return host;

            IServiceProvider GetProviderFromFactory(IServiceCollection collection)
            {
                var provider = collection.BuildServiceProvider();
                var factory = provider.GetService<IServiceProviderFactory<IServiceCollection>>();

                if (factory != null)
                {
                    using (provider)
                    {
                        return factory.CreateServiceProvider(factory.CreateBuilder(collection));
                    }
                }

                return provider;
            }
        }

        public IGrpcHostBuilder ConfigureAppConfiguration(Action<GrpcHostBuilderContext, IConfigurationBuilder> configureDelegate)
        {
            if (configureDelegate == null)
            {
                throw new ArgumentNullException(nameof(configureDelegate));
            }

            _configureAppConfigurationBuilderDelegates.Add(configureDelegate);
            return this;
        }

        #region privated

        private IServiceCollection BuildCommonServices()
        {
            // 应用根目录
            var rootPath = ResolveContentRootPath(string.Empty, AppContext.BaseDirectory);



            var services = new ServiceCollection();

            services.AddSingleton<GrpcHostOptions>(_options);
            services.AddSingleton<IConfiguration>(_config);

            services.AddTransient<IGrpcServerFactory, GrpcServerFactory>();
            services.AddOptions();
#if DEBUG
            services.AddLogging(logger => logger.SetMinimumLevel(LogLevel.Trace));
#else
            services.AddLogging();
#endif

            // Conjure up a RequestServices
            services.AddTransient<IStartupFilter, AutoRequestServicesStartupFilter>();
            services.AddTransient<IServiceProviderFactory<IServiceCollection>, DefaultServiceProviderFactory>();

            // Add GrpcContext
            services.AddScoped<GrpcContext>();
            // Aop 动态代理
            //services.AddInterception(builder => builder.SetDynamicProxyFactory());

            // 是否UseStartup
            if (!string.IsNullOrEmpty(_config[GrpcHostDefaults.StartupAssemblyKey]))
            {
                try
                {
                    var startupType = StartupLoader.FindStartupType(_config[GrpcHostDefaults.StartupAssemblyKey]);

                    if (typeof(IStartup).GetTypeInfo().IsAssignableFrom(startupType.GetTypeInfo()))
                    {
                        services.AddSingleton(typeof(IStartup), startupType);
                    }
                    else
                    {
                        services.AddSingleton(typeof(IStartup), sp =>
                        {
                            var methods = StartupLoader.LoadMethods(sp, startupType);
                            return new ConventionBasedStartup(methods);
                        });
                    }
                }
                catch (Exception ex)
                {
                    var capture = ExceptionDispatchInfo.Capture(ex);
                    services.AddSingleton<IStartup>(_ =>
                    {
                        capture.Throw();
                        return null;
                    });
                }
            }

            foreach (var configureServices in _configureServicesDelegates)
            {
                configureServices(_context, services);
            }

            return services;
        }


        private string ResolveContentRootPath(string contentRootPath, string basePath)
        {
            if (string.IsNullOrEmpty(contentRootPath))
            {
                return basePath;
            }
            if (Path.IsPathRooted(contentRootPath))
            {
                return contentRootPath;
            }
            return Path.Combine(Path.GetFullPath(basePath), contentRootPath);
        }

        #endregion
    }
}
