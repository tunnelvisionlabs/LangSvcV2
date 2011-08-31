namespace Tvl.VisualStudio.Language.Java.Project
{
    using Path = System.IO.Path;
    using File = System.IO.File;
    using Directory = System.IO.Directory;
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Project;
    using Microsoft.VisualStudio.Shell;
    using Registry = Microsoft.Win32.Registry;
    using RegistryKey = Microsoft.Win32.RegistryKey;
    using RegistryKeyPermissionCheck = Microsoft.Win32.RegistryKeyPermissionCheck;
    using SecurityException = System.Security.SecurityException;

    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration(JavaProjectConstants.JavaProjectPackageNameResourceString, JavaProjectConstants.JavaProjectPackageDetailsResourceString, JavaProjectConstants.JavaProjectPackageProductVersionString/*, IconResourceID = 400*/)]
    [Guid(JavaProjectConstants.JavaProjectPackageGuidString)]
    [ProvideProjectFactory(
        typeof(JavaProjectFactory),
        "Java",
        "Java Project Files (*.javaproj);*.javaproj",
        "javaproj",
        "javaproj",
        "ProjectTemplates",
        LanguageVsTemplate = Constants.JavaLanguageName,
        NewProjectRequireNewFolderVsTemplate = false)]

    [ProvideObject(typeof(PropertyPages.JavaBuildEventsPropertyPage))]
    [ProvideObject(typeof(PropertyPages.JavaBuildPropertyPage))]
    [ProvideObject(typeof(PropertyPages.JavaDebugPropertyPage))]
    [ProvideObject(typeof(PropertyPages.JavaGeneralPropertyPage))]

    public class JavaProjectPackage : ProjectPackage
    {
        private static string _javac64;
        private static string _javac32;
        private static string _java64;
        private static string _java32;

        public override string ProductUserContext
        {
            get
            {
                return "Java";
            }
        }

        protected override void Initialize()
        {
            base.Initialize();

            RegisterProjectFactory(new JavaProjectFactory(this));
        }

        public static string FindJavacPath(bool allow64Bit)
        {
            if (_javac32 == null && _javac64 == null)
            {
                string jdk64bitroot = null;
                string jdk32bitroot = null;

                string jdkroot = @"SOFTWARE\JavaSoft\Java Development Kit";
                string jdkwowroot = @"SOFTWARE\Wow6432Node\JavaSoft\Java Development Kit";
                if (Environment.Is64BitOperatingSystem)
                {
                    jdk64bitroot = jdkroot;
                    jdk32bitroot = jdkwowroot;
                }
                else
                {
                    jdk32bitroot = jdkroot;
                }

                if (jdk64bitroot != null)
                    _javac64 = FindJavaPath(jdk64bitroot, "javac.exe");
                if (jdk32bitroot != null)
                    _javac32 = FindJavaPath(jdk32bitroot, "javac.exe");
            }

            if (allow64Bit && _javac64 != null)
                return _javac64;

            return _javac32;
        }

        public static string FindJavaPath(bool allow64Bit)
        {
            if (_java32 == null && _java64 == null)
            {
                string jre64bitroot = null;
                string jre32bitroot = null;

                string jreroot = @"SOFTWARE\JavaSoft\Java Development Kit";
                string jrewowroot = @"SOFTWARE\Wow6432Node\JavaSoft\Java Development Kit";
                //string jreroot = @"SOFTWARE\JavaSoft\Java Runtime Environment";
                //string jrewowroot = @"SOFTWARE\Wow6432Node\JavaSoft\Java Runtime Environment";
                if (Environment.Is64BitOperatingSystem)
                {
                    jre64bitroot = jreroot;
                    jre32bitroot = jrewowroot;
                }
                else
                {
                    jre32bitroot = jreroot;
                }

                if (jre64bitroot != null)
                    _java64 = FindJavaPath(jre64bitroot, "java.exe");
                if (jre32bitroot != null)
                    _java32 = FindJavaPath(jre32bitroot, "java.exe");
            }

            if (allow64Bit && _java64 != null)
                return _java64;

            return _java32;
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
    }
}
