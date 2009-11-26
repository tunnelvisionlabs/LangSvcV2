using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JavaLanguageService.Power
{
    public enum PowerAction
    {
        None,
        Reserved,
        Sleep,
        Hibernate,
        Shutdown,
        ShutdownReset,
        ShutdownOff,
        WarmEject
    }
}
