using Grpc.Server;
using NLog;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grpc.Server
{
    public interface IMicroServiceConfiguration
    {
        IGrpcServer Server { get; }

        IMicroServiceConfiguration AddNLogRule(LogLevel minLevel, LogLevel maxLevel, Target target, string loggerNamePattern = "*");

        IMicroServiceConfiguration AddNLogRule(Target target, string loggerNamePattern = "*");

        IMicroServiceConfiguration AddNLogRule(LogLevel level, Target target, string loggerNamePattern = "*");
    }
}
