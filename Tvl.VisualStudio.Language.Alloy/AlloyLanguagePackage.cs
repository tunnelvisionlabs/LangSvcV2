namespace Tvl.VisualStudio.Language.Alloy
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;
    using Tvl.VisualStudio.Shell;

    using IServiceContainer = System.ComponentModel.Design.IServiceContainer;

    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration(AlloyConstants.AlloyLanguagePackageNameResourceString, AlloyConstants.AlloyLanguagePackageDetailsResourceString, AlloyConstants.AlloyLanguagePackageProductVersionString/*, IconResourceID = 400*/)]
    //[ProvideAutoLoad(AlloyConstants.UIContextNoSolution)]
    //[ProvideAutoLoad(AlloyConstants.UIContextSolutionExists)]
    [Guid(AlloyConstants.AlloyLanguagePackageGuidString)]
    [ProvideLanguageService(typeof(AlloyLanguageInfo), AlloyConstants.AlloyLanguageName, AlloyConstants.AlloyLanguageResourceId,
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
    [ProvideLanguageCodeExpansion(typeof(AlloyLanguageInfo), AlloyConstants.AlloyLanguageName, AlloyConstants.AlloyLanguageResourceId, AlloyConstants.AlloyLanguageName, @"%InstallRoot%\Alloy\Snippets\1033\SnippetsIndex.xml",
        SearchPaths = @"%InstallRoot%\Alloy\Snippets\%LCID%\Alloy;%MyDocs%\Code Snippets\Alloy\My Code Snippets\",
        ForceCreateDirs = @"%InstallRoot%\Alloy\Snippets\%LCID%\Alloy;%MyDocs%\Code Snippets\Alloy\My Code Snippets\",
        ShowRoots = false)]
    [ProvideLanguageExtension(typeof(AlloyLanguageInfo), AlloyConstants.AlloyFileExtension)]
    public class AlloyLanguagePackage : Package
    {
        private static AlloyLanguagePackage _instance;
        private AlloyLanguageInfo _languageInfo;

        public AlloyLanguagePackage()
        {
            _instance = this;
        }

        protected override void Initialize()
        {
            base.Initialize();

            // register the language service
            _languageInfo = new AlloyLanguageInfo(this.AsVsServiceProvider());
            ((IServiceContainer)this).AddService(typeof(AlloyLanguageInfo), _languageInfo, true);
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
