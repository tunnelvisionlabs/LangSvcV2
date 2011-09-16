namespace Tvl.VisualStudio.Language.Java.Debugger
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;
    using Tvl.VisualStudio.Shell;

    [PackageRegistration(UseManagedResourcesOnly = true)]
    [Guid(JavaDebuggerConstants.JavaDebuggerPackageGuidString)]
    [ProvideDebugEngine(typeof(JavaDebugEngine), Constants.JavaLanguageName,
        PortSuppliers = new string[] { "{708C1ECA-FF48-11D2-904F-00C04FA302A1}" },
        ProgramProvider = JavaDebuggerConstants.JavaProgramProviderGuidString,
        // basic configuration
        AlwaysLoadProgramProviderLocal = true,
        AlwaysLoadLocal = true,
        AutoSelectPriority = 0,
        ExcludeManualSelect = true,
        // feature support
        Attach = false,
        Disassembly = true,
        SuspendThread = true,
        SetNextStatement = false,
        JustInTimeDebugging = false,
        HitCountBreakpoints = false,
        FunctionBreakpoints = false,
        Exceptions = false,
        EditAndContinue = false,
        DumpWriting = false,
        DataBreakpoints = false,
        ConditionalBreakpoints = false,
        RemoteDebugging = false
        )]
    [ProvideObject(typeof(JavaDebugEngine))]
    [ProvideObject(typeof(JavaDebugProgramProvider))]
    public partial class JavaDebuggerPackage : Package
    {
    }
}
