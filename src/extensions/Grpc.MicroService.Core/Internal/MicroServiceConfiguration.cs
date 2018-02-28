using System;
using System.Collections.Generic;
using System.Text;
using Grpc.MicroService.NLog;
using Grpc.Server;
using NLog;
using NLog.Targets;

namespace Grpc.MicroService.Internal
{
    public class MicroServiceConfiguration : IMicroServiceConfiguration
    {
        public IGrpcServer Server { get; }

        internal readonly NLogConfigurationBuilder NLogBuilder;

        public MicroServiceConfiguration(IGrpcServer server)
        {
            Server = server;
            NLogBuilder = new NLogConfigurationBuilder();
        }

        public IMicroServiceConfiguration AddNLogRule(LogLevel minLevel, LogLevel maxLevel, Target target, string loggerNamePattern = "*")
        {
            NLogBuilder.AddNLogRule(minLevel, maxLevel, target, loggerNamePattern);
            return this;
        }

        public IMicroServiceConfiguration AddNLogRule(Target target, string loggerNamePattern = "*")
        {
            NLogBuilder.AddNLogRule(target, loggerNamePattern);
            return this;
        }

        public IMicroServiceConfiguration AddNLogRule(LogLevel level, Target target, string loggerNamePattern = "*")
        {
            NLogBuilder.AddNLogRule(level, target, loggerNamePattern);
            return this;
        }
    }
}
