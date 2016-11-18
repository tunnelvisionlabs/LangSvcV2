namespace Tvl.VisualStudio.Language.Go
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;
    using Tvl.VisualStudio.Shell;

    using IServiceContainer = System.ComponentModel.Design.IServiceContainer;

    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration(GoConstants.GoLanguagePackageNameResourceString, GoConstants.GoLanguagePackageDetailsResourceString, GoConstants.GoLanguagePackageProductVersionString/*, IconResourceID = 400*/)]
    //[ProvideAutoLoad(GoConstants.UIContextNoSolution)]
    //[ProvideAutoLoad(GoConstants.UIContextSolutionExists)]
    [Guid(GoConstants.GoLanguagePackageGuidString)]
    [ProvideLanguageService(typeof(GoLanguageInfo), GoConstants.GoLanguageName, GoConstants.GoLanguageResourceId,
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
    [ProvideLanguageCodeExpansion(typeof(GoLanguageInfo), GoConstants.GoLanguageName, GoConstants.GoLanguageResourceId, GoConstants.GoLanguageName, @"%InstallRoot%\Go\Snippets\1033\SnippetsIndex.xml",
        SearchPaths = @"%InstallRoot%\Go\Snippets\%LCID%\Go;%MyDocs%\Code Snippets\Go\My Code Snippets\",
        ForceCreateDirs = @"%InstallRoot%\Go\Snippets\%LCID%\Go;%MyDocs%\Code Snippets\Go\My Code Snippets\",
        ShowRoots = false)]
    [ProvideLanguageExtension(typeof(GoLanguageInfo), GoConstants.GoFileExtension)]
    public class GoLanguagePackage : Package
    {
        private static GoLanguagePackage _instance;
        private GoLanguageInfo _languageInfo;

        public GoLanguagePackage()
        {
            _instance = this;
        }

        protected override void Initialize()
        {
            base.Initialize();

            // register the language service
            _languageInfo = new GoLanguageInfo(this.AsVsServiceProvider());
            ((IServiceContainer)this).AddService(typeof(GoLanguageInfo), _languageInfo, true);
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
