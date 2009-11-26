using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JavaLanguageService.Power
{
    public struct PowerActionPolicy
    {
        private PowerAction _action;
        private PowerActionOptions _flags;
        private PowerNotification _eventCode;

        public PowerActionPolicy(PowerAction action, PowerActionOptions options, PowerNotification eventCode)
            : this()
        {
        }
    }
}
