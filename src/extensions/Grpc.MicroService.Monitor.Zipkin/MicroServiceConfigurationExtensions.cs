using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using zipkin4net;
using zipkin4net.Tracers.Zipkin;
using zipkin4net.Transport.Http;
using Grpc.MicroService.Internal;

namespace Grpc.Server
{
    public static class MicroServiceConfigurationExtensions
    {

        public static IMicroServiceConfiguration UseZipkinTracer(this IMicroServiceConfiguration config, string zipkinCollectorUrl)
        {
            var loggerFactory = config.Server.ApplicationServices.GetService<ILoggerFactory>();

            TraceManager.SamplingRate = 1.0f;
            var logger = new TracingLogger(loggerFactory, "zipkin4net");
            var httpSender = new HttpZipkinSender(zipkinCollectorUrl, "application/json");
            var tracer = new ZipkinTracer(httpSender, new JSONSpanSerializer(), new Statistics());
            TraceManager.RegisterTracer(tracer);
            TraceManager.Start(logger);

            config.Server.UseInterceptor(new ZipkinTracingServerInterceptor(config.Server.ApplicationServices));

            return config;
        }
    }
}
