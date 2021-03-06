﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Grpc.Server;
using System;
using System.Collections.Generic;
using System.Text;
using ExampleServer.Business;

namespace ExampleServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IExampleBusiness, ExampleBusiness>();

            services.AddMicroService(builder =>
            {
                builder.RegisterService<ExampleServiceImpl>();
                builder.AddMysql(Configuration.MicroService().GetConnectionString("DbConnectionString"));
                builder.AddAppMetrics(Configuration.MicroService().GetAppMetricsConfig());
            });
        }

        public void Configure(IGrpcServer app, ILoggerFactory loggerFactory)
        {
            app.UseMicroService(config =>
            {
                config.UseZipkinTracer(Configuration.MicroService().GetZipkinCollectorUrl());
                //config.UseAliyunLog(Configuration.MicroService().GetAliyunLogConfig());
                config.UseAppMetrics();
            });
        }
    }
}
