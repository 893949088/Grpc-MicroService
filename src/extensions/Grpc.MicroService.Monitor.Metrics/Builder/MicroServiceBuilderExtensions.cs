using App.Metrics;
using App.Metrics.Extensions.Configuration;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MicroServiceBuilderExtensions
    {

        public static IMicroServiceBuilder AddAppMetrics(this IMicroServiceBuilder builder, IConfigurationSection section)
        {
            builder.Services.AddMetrics(metricsBuilder =>
            {
                var influxDbSection = section.GetSection("InfluxDbReporter");

                metricsBuilder.Configuration.ReadFrom(section.GetSection("Options"));
                metricsBuilder.Report.ToInfluxDb(options =>
                {
                    options.InfluxDb.BaseUri = new Uri(influxDbSection.GetValue<string>("BaseUri"));
                    options.InfluxDb.Database = influxDbSection.GetValue<string>("Database");
                    options.InfluxDb.UserName = influxDbSection.GetValue<string>("UserName");
                    options.InfluxDb.Password = influxDbSection.GetValue<string>("Password");
                    options.FlushInterval = TimeSpan.FromSeconds(5);
                });
            });
            return builder;
        }
    }
}
