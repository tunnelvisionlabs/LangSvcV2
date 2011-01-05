namespace Tvl.VisualStudio.Language.Alloy
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.TextManager.Interop;
    using Tvl.VisualStudio.Shell.Extensions;
    using IConnectionPointContainer = Microsoft.VisualStudio.OLE.Interop.IConnectionPointContainer;
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
    [ProvideLanguageExtension(typeof(AlloyLanguageInfo), AlloyConstants.AlloyFileExtension)]
    public class AlloyLanguagePackage : Package
    {
        private static AlloyLanguagePackage _instance;

        private LanguagePreferences _languagePreferences;
        private IDisposable _languagePreferencesCookie;

        public AlloyLanguagePackage()
        {
            _instance = this;
        }

        public LanguagePreferences LanguagePreferences
        {
            get
            {
                return _languagePreferences;
            }
        }

        protected override void Initialize()
        {
            base.Initialize();

            SVsServiceProvider serviceProvider = this.AsVsServiceProvider();

            // register the language service
            var languageInfo = new AlloyLanguageInfo(serviceProvider);
            ((IServiceContainer)this).AddService(typeof(AlloyLanguageInfo), languageInfo, true);

            IVsTextManager2 textManager = serviceProvider.GetTextManager2();
            LANGPREFERENCES2[] langPreferences = new LANGPREFERENCES2[1];
            langPreferences[0].guidLang = typeof(AlloyLanguageInfo).GUID;
            ErrorHandler.ThrowOnFailure(textManager.GetUserPreferences2(null, null, langPreferences, null));
            _languagePreferences = new LanguagePreferences(langPreferences[0]);

            _languagePreferencesCookie = ((IConnectionPointContainer)textManager).Advise<LanguagePreferences, IVsTextManagerEvents2>(_languagePreferences);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_languagePreferencesCookie != null)
                {
                    _languagePreferencesCookie.Dispose();
                    _languagePreferencesCookie = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}
