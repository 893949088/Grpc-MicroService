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
        private readonly ConsoleTarget _defaultTarget;

        public NLogConfigurationBuilder()
        {
            _config = new LoggingConfiguration();
            _defaultTarget = new ConsoleTarget()
            {
                Layout = "${longdate} | ${level:uppercase=false:padding=-5} | ${message} ${onexception:${exception:format=tostring} ${newline} ${stacktrace} ${newline}"
            };

        }

        public void AddNLogRule(LogLevel minLevel, LogLevel maxLevel, Target target, string loggerNamePattern = "*")
        {
            _config.AddRule(minLevel, maxLevel, target, loggerNamePattern);
        }

        public void AddNLogRule(Target target, string loggerNamePattern = "*")
        {
            _config.AddRuleForAllLevels(target, loggerNamePattern);
        }

        public void AddNLogRule(LogLevel level, Target target, string loggerNamePattern = "*")
        {
            _config.AddRuleForOneLevel(level, target, loggerNamePattern);
        }

        public LoggingConfiguration Build()
        {

            // add default rule
            if (!_config.LoggingRules.Any())
            {
                _config.AddRuleForAllLevels(new AsyncTargetWrapper(_defaultTarget));
            }
            return _config;
        }

    }
}
