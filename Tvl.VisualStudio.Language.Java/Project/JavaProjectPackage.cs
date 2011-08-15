namespace Tvl.VisualStudio.Language.Java.Project
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Project;
    using Microsoft.VisualStudio.Shell;

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
    public class JavaProjectPackage : ProjectPackage
    {
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
    }
}
