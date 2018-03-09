using App.Metrics;
using App.Metrics.Histogram;
using App.Metrics.Reporting.InfluxDB;
using App.Metrics.Scheduling;
using Grpc.MicroService.Internal;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Grpc.Server
{
    public static class MicroServiceSetupBuilderExtensions
    {
        public static IMicroServiceSetupBuilder UseAppMetrics(this IMicroServiceSetupBuilder config)
        {
            var metrics = config.Server.ApplicationServices.GetService<IMetricsRoot>();

            config.Server.UseInterceptor(new GrpcMetricsServerInterceptor(config.Server.ApplicationServices));

            new AppMetricsTaskScheduler(TimeSpan.FromSeconds(3),
                async () =>
                {
                    await Task.WhenAll(metrics.ReportRunner.RunAllAsync());
                }).Start();
            return config;
        }
    }
}
