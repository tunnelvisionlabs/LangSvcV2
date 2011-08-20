namespace Tvl.VisualStudio.Language.Java.Debugger.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Debugger.Interop;
    using System.Diagnostics.Contracts;
    using Microsoft.VisualStudio;

    public static class DebugProcessExtensions
    {
        public static IDebugCoreServer2 GetServer(this IDebugProcess2 process)
        {
            Contract.Requires<ArgumentNullException>(process != null, "process");

            IDebugCoreServer2 server;
            ErrorHandler.ThrowOnFailure(process.GetServer(out server));
            return server;
        }

        public static AD_PROCESS_ID GetPhysicalProcessId(this IDebugProcess2 process)
        {
            Contract.Requires<ArgumentNullException>(process != null, "process");

            AD_PROCESS_ID[] processId = new AD_PROCESS_ID[1];
            ErrorHandler.ThrowOnFailure(process.GetPhysicalProcessId(processId));
            return processId[0];
        }

        public static string GetName(this IDebugProcess2 process, enum_GETNAME_TYPE type)
        {
            Contract.Requires<ArgumentNullException>(process != null, "process");

            string name;
            ErrorHandler.ThrowOnFailure(process.GetName(type, out name));
            return name;
        }
    }
}
