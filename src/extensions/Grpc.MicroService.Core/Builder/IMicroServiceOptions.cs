using Grpc.Server;
using NLog;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grpc.Server
{
    public interface IMicroServiceOptions
    {
        IGrpcServer Server { get; }

        IMicroServiceOptions AddNLogRule(LogLevel minLevel, LogLevel maxLevel, Target target, string loggerNamePattern = "*");

        IMicroServiceOptions AddNLogRule(Target target, string loggerNamePattern = "*");

        IMicroServiceOptions AddNLogRule(LogLevel level, Target target, string loggerNamePattern = "*");
    }
}
