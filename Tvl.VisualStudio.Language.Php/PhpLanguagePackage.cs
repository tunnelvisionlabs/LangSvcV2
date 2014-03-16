namespace Tvl.VisualStudio.Language.Php
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;
    using Tvl.VisualStudio.Shell;

    using __VSPHYSICALVIEWATTRIBUTES = Microsoft.VisualStudio.Shell.Interop.__VSPHYSICALVIEWATTRIBUTES;
    using IServiceContainer = System.ComponentModel.Design.IServiceContainer;
    using MessageBox = System.Windows.MessageBox;
    using RuleDependencyChecker = Antlr4.Runtime.Misc.RuleDependencyChecker;
    using VSConstants = Microsoft.VisualStudio.VSConstants;

    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration(PhpConstants.PhpLanguagePackageNameResourceString, PhpConstants.PhpLanguagePackageDetailsResourceString, PhpConstants.PhpLanguagePackageProductVersionString/*, IconResourceID = 400*/)]
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

    [ProvideEditorFactory2(typeof(PhpEditorFactoryWithoutEncoding), 101, CommonPhysicalViewAttributes = (int)__VSPHYSICALVIEWATTRIBUTES.PVA_SupportsPreview)]
    [ProvideLinkedEditorFactory(typeof(PhpEditorFactoryWithEncoding), typeof(PhpEditorFactoryWithoutEncoding), 102, CommonPhysicalViewAttributes = (int)__VSPHYSICALVIEWATTRIBUTES.PVA_None)]

    // don't need to include NameResourceID because it's handled by ProvideEditorFactory
    [ProvideEditorExtension(typeof(PhpEditorFactoryWithoutEncoding), PhpConstants.PhpFileExtension, 50)]
    [ProvideEditorExtension(typeof(PhpEditorFactoryWithoutEncoding), PhpConstants.Php5FileExtension, 50)]
    [ProvideEditorExtension(typeof(PhpEditorFactoryWithEncoding), PhpConstants.PhpFileExtension, 49)]
    [ProvideEditorExtension(typeof(PhpEditorFactoryWithEncoding), PhpConstants.Php5FileExtension, 49)]

    // registering a wildcard extension allows the user to customize file extensions
    [ProvideEditorExtension(typeof(PhpEditorFactoryWithoutEncoding), ".*", 2)]
    [ProvideEditorExtension(typeof(PhpEditorFactoryWithEncoding), ".*", 1)]

    /* If this is missing, then double-clicking on a line in the TVL IntelliSense output
     * window with a PHP file name will open a new window using a different factory rather
     * than reusing the window that's already open for the document.
     */
    [ProvideEditorLogicalView(typeof(PhpEditorFactoryWithoutEncoding), VSConstants.LOGVIEWID.TextView_string)]
    [ProvideEditorLogicalView(typeof(PhpEditorFactoryWithEncoding), VSConstants.LOGVIEWID.TextView_string)]

    [ProvideLanguageExtension(typeof(PhpLanguageInfo), PhpConstants.PhpFileExtension)]
    [ProvideLanguageExtension(typeof(PhpLanguageInfo), PhpConstants.Php5FileExtension)]
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

            RegisterEditorFactory(new PhpEditorFactoryWithoutEncoding(this));
            RegisterEditorFactory(new PhpEditorFactoryWithEncoding(this));

            try
            {
                RuleDependencyChecker.CheckDependencies(typeof(PhpLanguagePackage).Assembly);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Error validating ANTLR rule dependencies");
            }
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
