namespace Tvl.VisualStudio.Language.StringTemplate4
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;
    using Tvl.VisualStudio.Shell.Extensions;
    using IServiceContainer = System.ComponentModel.Design.IServiceContainer;

    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration(StringTemplateConstants.StringTemplateLanguagePackageNameResourceString, StringTemplateConstants.StringTemplateLanguagePackageDetailsResourceString, StringTemplateConstants.StringTemplateLanguagePackageProductVersionString/*, IconResourceID = 400*/)]
    //[ProvideAutoLoad(StringTemplateConstants.UIContextNoSolution)]
    //[ProvideAutoLoad(StringTemplateConstants.UIContextSolutionExists)]
    [Guid(StringTemplateConstants.StringTemplateLanguagePackageGuidString)]
    [ProvideLanguageService(typeof(StringTemplateLanguageInfo), StringTemplateConstants.StringTemplateLanguageName, StringTemplateConstants.StringTemplateLanguageResourceId,
        //AutoOutlining = true,
        //QuickInfo = true,
        ShowCompletion = true,
        ShowDropDownOptions = true,
        //ShowHotURLs = true,
        //ShowMatchingBrace = true,
        EnableAdvancedMembersOption = false,
        DefaultToInsertSpaces = false,
        //EnableCommenting = true,
        //HideAdvancedMembersByDefault = false,
        EnableLineNumbers = true,
        //CodeSense = true,
        RequestStockColors = true)]
    [ProvideLanguageExtension(typeof(StringTemplateLanguageInfo), StringTemplateConstants.StringTemplateGroupFileExtension)]
    [ProvideLanguageExtension(typeof(StringTemplateLanguageInfo), StringTemplateConstants.StringTemplateGroup4FileExtension)]
    [ProvideLanguageExtension(typeof(StringTemplateLanguageInfo), StringTemplateConstants.StringTemplateTemplateFileExtension)]
    [ProvideLanguageExtension(typeof(StringTemplateLanguageInfo), StringTemplateConstants.StringTemplateTemplate4FileExtension)]
    public class StringTemplateLanguagePackage : Package
    {
        private static StringTemplateLanguagePackage _instance;
        private StringTemplateLanguageInfo _languageInfo;

        public StringTemplateLanguagePackage()
        {
            _instance = this;
        }

        protected override void Initialize()
        {
            base.Initialize();

            // register the language service
            _languageInfo = new StringTemplateLanguageInfo(this.AsVsServiceProvider());
            ((IServiceContainer)this).AddService(typeof(StringTemplateLanguageInfo), _languageInfo, true);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_languageInfo != null)
                {
                    _languageInfo.Dispose();
                    _languageInfo = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}
