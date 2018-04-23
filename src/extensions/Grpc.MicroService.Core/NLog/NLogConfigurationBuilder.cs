using NLog;
using NLog.Config;
using NLog.Targets;
using NLog.Targets.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Grpc.MicroService.NLog
{
    public class NLogConfigurationBuilder
    {
        private readonly LoggingConfiguration _config;
        private readonly Target _failedDefaultTarget;
        private Target _defaultTarget;

        public NLogConfigurationBuilder()
        {
            _config = new LoggingConfiguration();
            _failedDefaultTarget = new FailedDefaultTarget();
            _defaultTarget = new ConsoleTarget()
            {
                Layout = "${longdate} | ${level:uppercase=false:padding=-5} | ${message} ${onexception:${exception:format=tostring} ${newline} ${stacktrace} ${newline}"
            };
        }

        public void AddNLogRule(LogLevel minLevel, LogLevel maxLevel, Target target, string loggerNamePattern, bool final = false)
        {
            if ("*".Equals(loggerNamePattern))
            {
                throw new ArgumentException("loggerNamePattern cannot be *");
            }

            var targetGroup = new FallbackGroupTarget(target, _failedDefaultTarget)
            {
                ReturnToFirstOnSuccess = true
            };
            _config.AddRule(minLevel, maxLevel, new AsyncTargetWrapper(targetGroup), loggerNamePattern, final);
        }

        public void AddNLogRule(Target target, string loggerNamePattern, bool final = false)
        {
            if ("*".Equals(loggerNamePattern))
            {
                throw new ArgumentException("loggerNamePattern cannot be *");
            }

            var targetGroup = new FallbackGroupTarget(target, _failedDefaultTarget)
            {
                ReturnToFirstOnSuccess = true
            };
            _config.AddRuleForAllLevels(new AsyncTargetWrapper(targetGroup), loggerNamePattern, final);
        }

        public void AddNLogRule(LogLevel level, Target target, string loggerNamePattern, bool final = false)
        {
            if ("*".Equals(loggerNamePattern))
            {
                throw new ArgumentException("loggerNamePattern cannot be *");
            }

            var targetGroup = new FallbackGroupTarget(target, _failedDefaultTarget)
            {
                ReturnToFirstOnSuccess = true
            };
            _config.AddRuleForOneLevel(level, new AsyncTargetWrapper(targetGroup), loggerNamePattern, final);
        }

        public void DefaultTarget(Target target)
        {
            this._defaultTarget = target;
        }

        public LoggingConfiguration Build()
        {
            // add default rule
            _config.AddRuleForAllLevels(new AsyncTargetWrapper(_defaultTarget));
            return _config;
        }

    }
}
