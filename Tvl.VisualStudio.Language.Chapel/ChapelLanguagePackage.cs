namespace Tvl.VisualStudio.Language.Chapel
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;
    using Tvl.VisualStudio.Shell.Extensions;
    using IServiceContainer = System.ComponentModel.Design.IServiceContainer;

    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration(ChapelConstants.ChapelLanguagePackageNameResourceString, ChapelConstants.ChapelLanguagePackageDetailsResourceString, ChapelConstants.ChapelLanguagePackageProductVersionString/*, IconResourceID = 400*/)]
    //[ProvideAutoLoad(Constants.UIContextNoSolution)]
    //[ProvideAutoLoad(Constants.UIContextSolutionExists)]
    [Guid(ChapelConstants.ChapelLanguagePackageGuidString)]
    [ProvideLanguageService(typeof(ChapelLanguageInfo), ChapelConstants.ChapelLanguageName, ChapelConstants.ChapelLanguageResourceId,
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
    [ProvideLanguageExtension(typeof(ChapelLanguageInfo), ChapelConstants.ChapelFileExtension)]
    public class ChapelLanguagePackage : Package
    {
        private static ChapelLanguagePackage _instance;
        private ChapelLanguageInfo _languageInfo;

        public ChapelLanguagePackage()
        {
            _instance = this;
        }

        protected override void Initialize()
        {
            base.Initialize();

            // register the language service
            _languageInfo = new ChapelLanguageInfo(this.AsVsServiceProvider());
            ((IServiceContainer)this).AddService(typeof(ChapelLanguageInfo), _languageInfo, true);
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
