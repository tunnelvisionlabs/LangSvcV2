namespace Tvl.VisualStudio.Language.Java.Debugger
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Shell;
    using System.Runtime.InteropServices;
    using Tvl.VisualStudio.Shell;

    [PackageRegistration(UseManagedResourcesOnly = true)]
    [Guid(JavaDebuggerConstants.JavaDebuggerPackageGuidString)]
    [ProvideDebugEngine(typeof(JavaDebugEngine), Constants.JavaLanguageName,
        PortSuppliers = new string[] { "{708C1ECA-FF48-11D2-904F-00C04FA302A1}" },
        ProgramProvider = "{" + JavaDebuggerConstants.JavaProgramProviderGuidString + "}",
        //ProgramProvider = "{4FF9DEF4-8922-4D02-9379-3FFA64D1D639}", // this is the local Win32 program provider
        Attach = true,
        AlwaysLoadProgramProviderLocal = true,
        AlwaysLoadLocal = true,
        AutoSelectPriority = 5,
        Disassembly = true
        //EngineCanWatchProcess = true
        //RemoteDebugging = true
        )]
    [ProvideObject(typeof(JavaDebugEngine))]
    [ProvideObject(typeof(JavaDebugProgramProvider))]
    public class JavaDebuggerPackage : Package
    {
    }
}
