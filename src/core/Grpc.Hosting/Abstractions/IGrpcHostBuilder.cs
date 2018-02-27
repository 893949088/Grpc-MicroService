using Grpc.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grpc.Hosting
{
    public interface IGrpcHostBuilder
    {
        
        IGrpcHost Build();

        IGrpcHostBuilder ConfigureAppConfiguration(Action<GrpcHostBuilderContext, IConfigurationBuilder> configureDelegate);

        IGrpcHostBuilder ConfigureServices(Action<IServiceCollection> configureServices);

        IGrpcHostBuilder ConfigureServices(Action<GrpcHostBuilderContext, IServiceCollection> configureServices);
        
        string GetSetting(string key);

        IGrpcHostBuilder UseSetting(string key, string value);

        IGrpcHostBuilder ApplicationName(string name);

        IGrpcHostBuilder BindPort(string host, int port, ServerCredentials credentials);

        IGrpcHostBuilder BindChannelOptions(params ChannelOption[] options);
    }
}
