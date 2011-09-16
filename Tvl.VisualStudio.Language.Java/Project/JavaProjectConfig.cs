namespace Tvl.VisualStudio.Language.Java.Project
{
    using System;
    using System.Diagnostics.Contracts;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Project;
    using Tvl.VisualStudio.Shell;

    using __VSDBGLAUNCHFLAGS = Microsoft.VisualStudio.Shell.Interop.__VSDBGLAUNCHFLAGS;
    using _PersistStorageType = Microsoft.VisualStudio.Shell.Interop._PersistStorageType;
    using CommandLineBuilder = Microsoft.Build.Utilities.CommandLineBuilder;
    using DEBUG_LAUNCH_OPERATION = Microsoft.VisualStudio.Shell.Interop.DEBUG_LAUNCH_OPERATION;
    using Directory = System.IO.Directory;
    using File = System.IO.File;
    using IVsDebugger2 = Microsoft.VisualStudio.Shell.Interop.IVsDebugger2;
    using IVsUIShell = Microsoft.VisualStudio.Shell.Interop.IVsUIShell;
    using JavaDebugEngine = Tvl.VisualStudio.Language.Java.Debugger.JavaDebugEngine;
    using Path = System.IO.Path;
    using Registry = Microsoft.Win32.Registry;
    using RegistryKey = Microsoft.Win32.RegistryKey;
    using RegistryKeyPermissionCheck = Microsoft.Win32.RegistryKeyPermissionCheck;
    using SecurityException = System.Security.SecurityException;
    using StringComparison = System.StringComparison;
    using SVsShellDebugger = Microsoft.VisualStudio.Shell.Interop.SVsShellDebugger;
    using SVsUIShell = Microsoft.VisualStudio.Shell.Interop.SVsUIShell;

    public class JavaProjectConfig : ProjectConfig
    {
        internal JavaProjectConfig(JavaProjectNode project, string configuration, string platform)
            : base(project, configuration, platform)
        {
            Contract.Requires(project != null);
            Contract.Requires(!string.IsNullOrEmpty(configuration));
            Contract.Requires(!string.IsNullOrEmpty(platform));
        }

        public new JavaProjectNode ProjectManager
        {
            get
            {
                Contract.Ensures(Contract.Result<JavaProjectNode>() != null);
                return (JavaProjectNode)base.ProjectManager;
            }
        }

        public string FindJavaBinary(string fileName, bool developmentKit)
        {
            string vendorBase = GetConfigurationProperty("JvmRegistryBase", _PersistStorageType.PST_PROJECT_FILE, false);

            bool allow64bit = Platform.EndsWith("X64", StringComparison.OrdinalIgnoreCase) || Platform.EndsWith("Any CPU", StringComparison.OrdinalIgnoreCase);
            bool allow32bit = Platform.EndsWith("X86", StringComparison.OrdinalIgnoreCase) || Platform.EndsWith("Any CPU", StringComparison.OrdinalIgnoreCase);

            string softwareRegKey64 = Environment.Is64BitOperatingSystem ? @"SOFTWARE\" : null;
            string softwareRegKey32 = Environment.Is64BitOperatingSystem ? @"SOFTWARE\Wow6432Node\" : @"SOFTWARE\";

            string installation = developmentKit ? "Java Development Kit" : "Java Runtime Environment";

            if (allow64bit && softwareRegKey64 != null)
            {
                string registryRoot = softwareRegKey64 + vendorBase + @"\" + installation;
                string path = FindJavaPath(registryRoot, fileName);
                if (!string.IsNullOrEmpty(path))
                    return path;
            }

            if (allow32bit)
            {
                string registryRoot = softwareRegKey32 + vendorBase + @"\" + installation;
                string path = FindJavaPath(registryRoot, fileName);
                if (!string.IsNullOrEmpty(path))
                    return path;
            }

            return null;
        }

        private static string FindJavaPath(string registryRoot, string fileName)
        {
            try
            {
                using (RegistryKey jdk = Registry.LocalMachine.OpenSubKey(registryRoot, RegistryKeyPermissionCheck.ReadSubTree))
                {
                    if (jdk == null)
                        return null;

                    string currentVersion = jdk.GetValue("CurrentVersion") as string;
                    if (currentVersion == null)
                        return null;

                    using (RegistryKey jdkVersion = jdk.OpenSubKey(currentVersion, RegistryKeyPermissionCheck.ReadSubTree))
                    {
                        if (jdkVersion == null)
                            return null;

                        string javaHome = jdkVersion.GetValue("JavaHome") as string;
                        if (!Directory.Exists(javaHome))
                            return null;

                        string javac = Path.Combine(javaHome, "bin", fileName);
                        if (!File.Exists(javac))
                            return null;

                        return javac;
                    }
                }
            }
            catch (SecurityException)
            {
                return null;
            }
        }

        public override void Invalidate()
        {
            base.Invalidate();
        }

        public override int QueryDebugLaunch(uint flags, out int fCanLaunch)
        {
            fCanLaunch = 1;
            return VSConstants.S_OK;
        }

        public override int DebugLaunch(uint grfLaunch)
        {
            DebugTargetInfo info = new DebugTargetInfo();

            CommandLineBuilder commandLine = new CommandLineBuilder();

            bool x64 = Platform.EndsWith("X64", StringComparison.OrdinalIgnoreCase) || (Platform.EndsWith("Any CPU", StringComparison.OrdinalIgnoreCase) && Environment.Is64BitOperatingSystem);
            string agentBaseFileName = "Tvl.Java.DebugHostWrapper";
            if (x64)
                agentBaseFileName += "X64";

            string agentFolder = Path.GetDirectoryName(typeof(JavaProjectConfig).Assembly.Location);
            string agentFileName = agentBaseFileName + ".dll";
            string agentPath = Path.GetFullPath(Path.Combine(agentFolder, agentFileName));
            commandLine.AppendSwitchIfNotNull("-agentpath:", agentPath);

            string agentArguments = GetConfigurationProperty(JavaConfigConstants.DebugAgentArguments, _PersistStorageType.PST_USER_FILE, false);
            if (!string.IsNullOrEmpty(agentArguments))
                commandLine.AppendTextUnquoted("=" + agentArguments);

            switch (GetConfigurationProperty(JavaConfigConstants.DebugStartAction, _PersistStorageType.PST_USER_FILE, false))
            {
            case "Class":
                string jvmArguments = GetConfigurationProperty(JavaConfigConstants.DebugJvmArguments, _PersistStorageType.PST_USER_FILE, false);
                if (!string.IsNullOrEmpty(jvmArguments))
                    commandLine.AppendTextUnquoted(" " + jvmArguments);

                string startupObject = GetConfigurationProperty(JavaConfigConstants.DebugStartClass, _PersistStorageType.PST_USER_FILE, false);
                if (!string.IsNullOrEmpty(startupObject))
                    commandLine.AppendFileNameIfNotNull(startupObject);

                break;

            default:
                throw new NotImplementedException();
            }

            string debugArgs = GetConfigurationProperty(JavaConfigConstants.DebugExtraArgs, _PersistStorageType.PST_USER_FILE, false);
            if (!string.IsNullOrEmpty(debugArgs))
                commandLine.AppendTextUnquoted(" " + debugArgs);

            string workingDirectory = GetConfigurationProperty(JavaConfigConstants.DebugWorkingDirectory, _PersistStorageType.PST_USER_FILE, false);
            if (string.IsNullOrEmpty(workingDirectory))
                workingDirectory = GetConfigurationProperty(JavaConfigConstants.OutputPath, _PersistStorageType.PST_USER_FILE, false);

            if (!Path.IsPathRooted(workingDirectory))
            {
                workingDirectory = Path.GetFullPath(Path.Combine(this.ProjectManager.ProjectFolder, workingDirectory));
            }

            //List<string> arguments = new List<string>();
            //arguments.Add(@"-agentpath:C:\dev\SimpleC\Tvl.Java.DebugHost\bin\Debug\Tvl.Java.DebugHostWrapper.dll");
            ////arguments.Add(@"-verbose:jni");
            ////arguments.Add(@"-cp");
            ////arguments.Add(@"C:\dev\JavaProjectTest\JavaProject\out\Debug");
            //arguments.Add("tvl.school.ee382v.a3.problem1.program1");
            ////arguments.Add(GetConfigurationProperty("OutputPath", true));
            ////arguments.Add(GetConfigurationProperty(JavaConfigConstants.DebugStartClass, false, _PersistStorageType.PST_USER_FILE));
            ////arguments.Add(GetConfigurationProperty(JavaConfigConstants.DebugExtraArgs, false, _PersistStorageType.PST_USER_FILE));

            //info.Arguments = string.Join(" ", arguments);

            info.Arguments = commandLine.ToString();

            bool useDevelopmentEnvironment = (grfLaunch & (uint)__VSDBGLAUNCHFLAGS.DBGLAUNCH_NoDebug) == 0;
            info.Executable = FindJavaBinary("java.exe", useDevelopmentEnvironment);

            //info.CurrentDirectory = GetConfigurationProperty("WorkingDirectory", false, _PersistStorageType.PST_USER_FILE);
            info.CurrentDirectory = workingDirectory;
            info.SendToOutputWindow = false;
            info.DebugEngines = new Guid[]
                {
                    typeof(JavaDebugEngine).GUID,
                    //VSConstants.DebugEnginesGuids.ManagedOnly_guid,
                    //VSConstants.DebugEnginesGuids.NativeOnly_guid,
                };
            info.PortSupplier = new Guid("{708C1ECA-FF48-11D2-904F-00C04FA302A1}");
            info.LaunchOperation = DEBUG_LAUNCH_OPERATION.DLO_CreateProcess;
            info.LaunchFlags = (__VSDBGLAUNCHFLAGS)grfLaunch;

            var debugger = (IVsDebugger2)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SVsShellDebugger));
            int result = debugger.LaunchDebugTargets(info);

            if (result != VSConstants.S_OK)
            {
                IVsUIShell uishell = (IVsUIShell)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SVsUIShell));
                string message = uishell.GetErrorInfo();
            }

            return result;
        }
    }
}
