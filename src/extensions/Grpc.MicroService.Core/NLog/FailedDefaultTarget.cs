using NLog;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grpc.MicroService.NLog
{

    internal class FailedDefaultTarget : TargetWithLayout
    {

        public FailedDefaultTarget()
        {

        }

        protected override void Write(LogEventInfo logEvent)
        {
            Console.WriteLine($"NLog::{logEvent.TimeStamp.ToString()}::{logEvent.Level}::{logEvent.Message}");
        }
    }
}
