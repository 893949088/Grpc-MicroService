using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using zipkin4net;

namespace Microsoft.AspNetCore.Zipkin.Internal
{
    public class ZipkinTracingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _serviceName;


        public ZipkinTracingMiddleware(RequestDelegate next,string serviceName)
        {
            _next = next;
            _serviceName = serviceName;
        }

        public async Task Invoke(HttpContext context)
        {
            var trace = Trace.Create();
            Trace.Current = trace;
            using (var serverTrace = new ServerTrace(_serviceName, context.Request.Method))
            {
                trace.Record(Annotations.Tag("http.host", context.Request.Host.ToString()));
                trace.Record(Annotations.Tag("http.uri", UriHelper.GetDisplayUrl(context.Request)));
                trace.Record(Annotations.Tag("http.path", context.Request.Path));
                await serverTrace.TracedActionAsync(_next.Invoke(context));
            }
        }
    }
}
