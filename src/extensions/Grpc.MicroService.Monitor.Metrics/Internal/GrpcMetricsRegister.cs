using App.Metrics;
using App.Metrics.Timer;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grpc.MicroService.Internal
{
    internal static class GrpcMetricsRegister
    {

        public static class ApdexScores
        {
        }

        public static class Counters
        {

        }

        public static class Gauges
        {

        }

        public static class Histograms
        {

        }

        public static class Meters
        {

        }
        
        public static class Timers
        {
            public static readonly TimerOptions GrpcRequestTransactionDuration = new TimerOptions
            {
                Name = "Grpc Duration",
                MeasurementUnit = Unit.Requests
            };

        }
    }
}
