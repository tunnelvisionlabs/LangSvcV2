namespace Tvl.VisualStudio.Language.Java.Project
{
    using Microsoft.VisualStudio.Project;

    public class JavaConfigProvider : ConfigProvider
    {
        public const string DisplayHotspotAnyCPU = "Hotspot Any CPU";
        public const string DisplayHotspotX86 = "Hotspot X86";
        public const string DisplayHotspotX64 = "Hotspot X64";
        public const string DisplayJRockitAnyCPU = "JRockit Any CPU";
        public const string DisplayJRockitX86 = "JRockit X86";
        public const string DisplayJRockitX64 = "JRockit X64";

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
            case JavaProjectFileConstants.HotspotAnyCPU:
                return DisplayHotspotAnyCPU;

            case JavaProjectFileConstants.HotspotX86:
                return DisplayHotspotX86;

            case JavaProjectFileConstants.HotspotX64:
                return DisplayHotspotX64;

            case JavaProjectFileConstants.JRockitAnyCPU:
                return DisplayJRockitAnyCPU;

            case JavaProjectFileConstants.JRockitX86:
                return DisplayJRockitX86;

            case JavaProjectFileConstants.JRockitX64:
                return DisplayJRockitX64;

            default:
                return base.GetPlatformNameFromPlatformProperty(platformProperty);
            }
        }

        public override string GetPlatformPropertyFromPlatformName(string platformName)
        {
            switch (platformName)
            {
            case DisplayHotspotAnyCPU:
                return JavaProjectFileConstants.HotspotAnyCPU;

            case DisplayHotspotX86:
                return JavaProjectFileConstants.HotspotX86;

            case DisplayHotspotX64:
                return JavaProjectFileConstants.HotspotX64;

            case DisplayJRockitAnyCPU:
                return JavaProjectFileConstants.JRockitAnyCPU;

            case DisplayJRockitX86:
                return JavaProjectFileConstants.JRockitX86;

            case DisplayJRockitX64:
                return JavaProjectFileConstants.JRockitX64;

            default:
                return base.GetPlatformPropertyFromPlatformName(platformName);
            }
        }
    }
}
