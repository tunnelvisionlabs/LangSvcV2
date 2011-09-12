namespace Tvl.VisualStudio.Language.Java.Project
{
    using Microsoft.VisualStudio.Project;

    public class JavaConfigProvider : ConfigProvider
    {
        public const string DisplayAnyCPU = "Any CPU";
        public const string DisplayX86 = "X86";
        public const string DisplayX64 = "X64";

        public JavaConfigProvider(JavaProjectNode manager)
            : base(manager)
        {
        }

        protected new JavaProjectNode ProjectManager
        {
            get
            {
                return (JavaProjectNode)base.ProjectManager;
            }
        }

        protected override ProjectConfig CreateProjectConfiguration(string configName, string platform)
        {
            return new JavaProjectConfig(this.ProjectManager, configName, platform);
        }

        public override string GetPlatformNameFromPlatformProperty(string platformProperty)
        {
            switch (platformProperty)
            {
            case JavaProjectFileConstants.AnyCPU:
                return DisplayAnyCPU;

            case JavaProjectFileConstants.X86:
                return DisplayX86;

            case JavaProjectFileConstants.X64:
                return DisplayX64;

            default:
                return base.GetPlatformNameFromPlatformProperty(platformProperty);
            }
        }

        public override string GetPlatformPropertyFromPlatformName(string platformName)
        {
            switch (platformName)
            {
            case DisplayAnyCPU:
                return JavaProjectFileConstants.AnyCPU;

            case DisplayX86:
                return JavaProjectFileConstants.X86;

            case DisplayX64:
                return JavaProjectFileConstants.X64;

            default:
                return base.GetPlatformPropertyFromPlatformName(platformName);
            }
        }
    }
}
