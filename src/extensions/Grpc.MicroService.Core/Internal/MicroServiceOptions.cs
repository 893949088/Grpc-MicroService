using System;
using System.Collections.Generic;
using System.Text;
using Grpc.MicroService.NLog;
using Grpc.Server;
using NLog;
using NLog.Targets;

namespace Grpc.MicroService.Internal
{
    public class MicroServiceOptions : IMicroServiceOptions
    {
        public IGrpcServer Server { get; }

        internal readonly NLogConfigurationBuilder NLogBuilder;

        public MicroServiceOptions(IGrpcServer server)
        {
            Server = server;
            NLogBuilder = new NLogConfigurationBuilder();
        }

        public IMicroServiceOptions AddNLogRule(LogLevel minLevel, LogLevel maxLevel, Target target, string loggerNamePattern = "*")
        {
            NLogBuilder.AddNLogRule(minLevel, maxLevel, target, loggerNamePattern);
            return this;
        }

        public IMicroServiceOptions AddNLogRule(Target target, string loggerNamePattern = "*")
        {
            NLogBuilder.AddNLogRule(target, loggerNamePattern);
            return this;
        }

        public IMicroServiceOptions AddNLogRule(LogLevel level, Target target, string loggerNamePattern = "*")
        {
            NLogBuilder.AddNLogRule(level, target, loggerNamePattern);
            return this;
        }
    }
}
