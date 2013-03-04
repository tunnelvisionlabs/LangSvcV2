namespace Tvl.VisualStudio.Language.Php
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;
    using Tvl.VisualStudio.Shell;

    using IServiceContainer = System.ComponentModel.Design.IServiceContainer;

    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration(PhpConstants.PhpLanguagePackageNameResourceString, PhpConstants.PhpLanguagePackageDetailsResourceString, PhpConstants.PhpLanguagePackageProductVersionString/*, IconResourceID = 400*/)]
    //[ProvideAutoLoad(PhpConstants.UIContextNoSolution)]
    //[ProvideAutoLoad(PhpConstants.UIContextSolutionExists)]
    [Guid(PhpConstants.PhpLanguagePackageGuidString)]
    [ProvideLanguageService(typeof(PhpLanguageInfo), PhpConstants.PhpLanguageName, PhpConstants.PhpLanguageResourceId,
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
    [ProvideEditorExtension(typeof(PhpEditorFactory), PhpConstants.Php5FileExtension, 50, NameResourceID = 101)]
    [ProvideEditorExtension(typeof(PhpEditorFactoryWithEncoding), PhpConstants.Php5FileExtension, 50, NameResourceID = 102)]
    [ProvideLanguageExtension(typeof(PhpLanguageInfo), PhpConstants.PhpFileExtension)]
    [ProvideLanguageExtension(typeof(PhpLanguageInfo), PhpConstants.Php5FileExtension)]
    [ProvideBindingPath]
    public class PhpLanguagePackage : Package
    {
        private static PhpLanguagePackage _instance;
        private PhpLanguageInfo _languageInfo;

        public PhpLanguagePackage()
        {
            _instance = this;
        }

        protected override void Initialize()
        {
            base.Initialize();

            // register the language service
            _languageInfo = new PhpLanguageInfo(this.AsVsServiceProvider());
            ((IServiceContainer)this).AddService(typeof(PhpLanguageInfo), _languageInfo, true);

            RegisterEditorFactory(new PhpEditorFactory(this));
            RegisterEditorFactory(new PhpEditorFactoryWithEncoding(this));
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
