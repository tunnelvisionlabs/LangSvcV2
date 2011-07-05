namespace Tvl.VisualStudio.Language.Antlr3
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;
    using Tvl.VisualStudio.Shell.Extensions;
    using IServiceContainer = System.ComponentModel.Design.IServiceContainer;
    using Tvl.VisualStudio.Shell;

    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration(AntlrConstants.AntlrLanguagePackageNameResourceString, AntlrConstants.AntlrLanguagePackageDetailsResourceString, AntlrConstants.AntlrLanguagePackageProductVersionString/*, IconResourceID = 400*/)]
    //[ProvideAutoLoad(AntlrConstants.UIContextNoSolution)]
    //[ProvideAutoLoad(AntlrConstants.UIContextSolutionExists)]
    [Guid(AntlrConstants.AntlrLanguagePackageGuidString)]
    [ProvideLanguageService(typeof(AntlrLanguageInfo), AntlrConstants.AntlrLanguageName, AntlrConstants.AntlrLanguageResourceId,
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
    [ProvideLanguageExtension(typeof(AntlrLanguageInfo), AntlrConstants.AntlrFileExtension)]
    [ProvideLanguageExtension(typeof(AntlrLanguageInfo), AntlrConstants.AntlrFileExtension2)]

    [ProvideDebuggerException(typeof(Antlr.Runtime.EarlyExitException))]
    [ProvideDebuggerException(typeof(Antlr.Runtime.FailedPredicateException))]
    [ProvideDebuggerException(typeof(Antlr.Runtime.MismatchedNotSetException))]
    [ProvideDebuggerException(typeof(Antlr.Runtime.MismatchedRangeException))]
    [ProvideDebuggerException(typeof(Antlr.Runtime.MismatchedSetException))]
    [ProvideDebuggerException(typeof(Antlr.Runtime.MismatchedTokenException))]
    [ProvideDebuggerException(typeof(Antlr.Runtime.MismatchedTreeNodeException))]
    [ProvideDebuggerException(typeof(Antlr.Runtime.MissingTokenException))]
    [ProvideDebuggerException(typeof(Antlr.Runtime.NoViableAltException))]
    [ProvideDebuggerException(typeof(Antlr.Runtime.RecognitionException))]
    [ProvideDebuggerException(typeof(Antlr.Runtime.UnwantedTokenException))]

    [ProvideDebuggerException(typeof(Antlr.Runtime.Tree.RewriteCardinalityException))]
    [ProvideDebuggerException(typeof(Antlr.Runtime.Tree.RewriteEarlyExitException))]
    [ProvideDebuggerException(typeof(Antlr.Runtime.Tree.RewriteEmptyStreamException))]

    public class AntlrLanguagePackage : Package
    {
        private static AntlrLanguagePackage _instance;
        private AntlrLanguageInfo _languageInfo;

        public AntlrLanguagePackage()
        {
            _instance = this;
        }

        protected override void Initialize()
        {
            base.Initialize();

            // register the language service
            _languageInfo = new AntlrLanguageInfo(this.AsVsServiceProvider());
            ((IServiceContainer)this).AddService(typeof(AntlrLanguageInfo), _languageInfo, true);
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
