using System;
using System.Collections.Generic;
using System.Text;
using Grpc.MicroService.NLog;
using Grpc.Server;
using NLog;
using NLog.Targets;

namespace Grpc.MicroService.Internal
{
    public class MicroServiceSetupBuilder : IMicroServiceSetupBuilder
    {
        public IGrpcServer Server { get; }

        internal readonly NLogConfigurationBuilder NLogBuilder;

        public MicroServiceSetupBuilder(IGrpcServer server)
        {
            Server = server;
            NLogBuilder = new NLogConfigurationBuilder();
        }

        public IMicroServiceSetupBuilder AddNLogRule(LogLevel minLevel, LogLevel maxLevel, Target target, string loggerNamePattern, bool final)
        {
            NLogBuilder.AddNLogRule(minLevel, maxLevel, target, loggerNamePattern, final);
            return this;
        }

        public IMicroServiceSetupBuilder AddNLogRule(Target target, string loggerNamePattern, bool final)
        {
            NLogBuilder.AddNLogRule(target, loggerNamePattern, final);
            return this;
        }

        public IMicroServiceSetupBuilder AddNLogRule(LogLevel level, Target target, string loggerNamePattern, bool final)
        {
            NLogBuilder.AddNLogRule(level, target, loggerNamePattern, final);
            return this;
        }

    }
}
