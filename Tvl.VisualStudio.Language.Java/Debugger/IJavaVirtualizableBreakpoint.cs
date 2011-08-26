namespace Tvl.VisualStudio.Language.Java.Debugger
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Debugger.Interop;
    using Tvl.Java.DebugInterface;

    public interface IJavaVirtualizableBreakpoint
    {
        void Bind(JavaDebugProgram program, JavaDebugThread thread, IReferenceType type, IEnumerable<string> sourcePaths);
    }
}
