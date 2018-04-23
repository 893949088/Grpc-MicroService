using Grpc.Server;
using NLog;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grpc.Server
{
    public interface IMicroServiceSetupBuilder
    {
        IGrpcServer Server { get; }

        IMicroServiceSetupBuilder AddNLogRule(LogLevel minLevel, LogLevel maxLevel, Target target, string loggerNamePattern, bool final = false);

        IMicroServiceSetupBuilder AddNLogRule(Target target, string loggerNamePattern, bool final = false);

        IMicroServiceSetupBuilder AddNLogRule(LogLevel level, Target target, string loggerNamePattern, bool final = false);
    }
}
