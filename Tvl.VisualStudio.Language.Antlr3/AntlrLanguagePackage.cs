namespace Tvl.VisualStudio.Language.Antlr3
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using Tvl.VisualStudio.Language.Antlr3.Project;
    using Tvl.VisualStudio.Shell;
    using Antlr4Constants = Tvl.VisualStudio.Language.AntlrV4.Antlr4Constants;
    using Antlr4LanguageInfo = Tvl.VisualStudio.Language.AntlrV4.Antlr4LanguageInfo;
    using AntlrIntellisenseOptions = Tvl.VisualStudio.Language.Antlr3.OptionsPages.AntlrIntellisenseOptions;
    using IServiceContainer = System.ComponentModel.Design.IServiceContainer;
    using ObjectExtenders = EnvDTE.ObjectExtenders;
    using Path = System.IO.Path;
    using PrjKind = VSLangProj.PrjKind;

    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration(AntlrConstants.AntlrLanguagePackageNameResourceString, AntlrConstants.AntlrLanguagePackageDetailsResourceString, AntlrConstants.AntlrLanguagePackageProductVersionString/*, IconResourceID = 400*/)]
    //[ProvideAutoLoad(AntlrConstants.UIContextNoSolution)]
    //[ProvideAutoLoad(AntlrConstants.UIContextSolutionExists)]
    [Guid(AntlrConstants.AntlrLanguagePackageGuidString)]
    [ProvideLanguageService(typeof(AntlrLanguageInfo), AntlrConstants.AntlrLanguageName, AntlrConstants.AntlrLanguageResourceId,
        ShowCompletion = true,
        ShowDropDownOptions = true,
        EnableAdvancedMembersOption = false,
        DefaultToInsertSpaces = false,
        //HideAdvancedMembersByDefault = false,
        RequestStockColors = true,
        //DefaultToNonHotURLs = false,

        // These items are not used. The default values are shown, but ignored.
        //ShowHotURLs = false,

        // These items were only used by MPF language services framework. The default values are shown, but ignored.
        //AutoOutlining = true,
        //CodeSense = true,
        //CodeSenseDelay = 1000,
        //EnableAsyncCompletion = true,
        //EnableCommenting = true,
        //EnableFormatSelection = false,
        //MatchBraces = true,
        //MatchBracesAtCaret = true,
        //MaxErrorMessages = 10,
        //QuickInfo = true,
        //ShowMatchingBrace = true,

        // this item is last since it's always true
        EnableLineNumbers = true)]
    [ProvideLanguageCodeExpansion(typeof(AntlrLanguageInfo), AntlrConstants.AntlrLanguageName, AntlrConstants.AntlrLanguageResourceId, AntlrConstants.AntlrLanguageName, @"%InstallRoot%\ANTLR\Snippets\1033\SnippetsIndex.xml",
        SearchPaths = @"%InstallRoot%\ANTLR\Snippets\%LCID%\ANTLR;%MyDocs%\Code Snippets\ANTLR\My Code Snippets\",
        ForceCreateDirs = @"%InstallRoot%\ANTLR\Snippets\%LCID%\ANTLR;%MyDocs%\Code Snippets\ANTLR\My Code Snippets\",
        ShowRoots = false)]
    [ProvideLanguageExtension(typeof(AntlrLanguageInfo), AntlrConstants.AntlrFileExtension)]
    [ProvideLanguageExtension(typeof(AntlrLanguageInfo), AntlrConstants.AntlrFileExtension2)]

    [ProvideLanguageEditorOptionPage(typeof(AntlrIntellisenseOptions), AntlrConstants.AntlrLanguageName, "", "IntelliSense", "#210")]

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

    [ProvideLanguageService(typeof(Antlr4LanguageInfo), Antlr4Constants.AntlrLanguageName, Antlr4Constants.AntlrLanguageResourceId,
        ShowCompletion = false,
        ShowDropDownOptions = true,
        EnableAdvancedMembersOption = false,
        DefaultToInsertSpaces = false,
        ShowSmartIndent = true,
        //HideAdvancedMembersByDefault = false,
        RequestStockColors = true,
        //DefaultToNonHotURLs = false,

        // These items are not used. The default values are shown, but ignored.
        //ShowHotURLs = false,

        // These items were only used by MPF language services framework. The default values are shown, but ignored.
        //AutoOutlining = true,
        //CodeSense = true,
        //CodeSenseDelay = 1000,
        //EnableAsyncCompletion = true,
        //EnableCommenting = true,
        //EnableFormatSelection = false,
        //MatchBraces = true,
        //MatchBracesAtCaret = true,
        //MaxErrorMessages = 10,
        //QuickInfo = true,
        //ShowMatchingBrace = true,

        // this item is last since it's always true
        EnableLineNumbers = true)]
    [ProvideLanguageCodeExpansion(typeof(Antlr4LanguageInfo), Antlr4Constants.AntlrLanguageName, Antlr4Constants.AntlrLanguageResourceId, Antlr4Constants.AntlrLanguageName, @"%InstallRoot%\ANTLR4\Snippets\1033\SnippetsIndex.xml",
        SearchPaths = @"%InstallRoot%\ANTLR4\Snippets\%LCID%\ANTLR4;%MyDocs%\Code Snippets\ANTLR4\My Code Snippets\",
        ForceCreateDirs = @"%InstallRoot%\ANTLR4\Snippets\%LCID%\ANTLR4;%MyDocs%\Code Snippets\ANTLR4\My Code Snippets\",
        ShowRoots = false)]
    [ProvideLanguageExtension(typeof(Antlr4LanguageInfo), Antlr4Constants.AntlrFileExtension)]

    [ProvideDebuggerException(typeof(Antlr4.Runtime.FailedPredicateException))]
    [ProvideDebuggerException(typeof(Antlr4.Runtime.InputMismatchException))]
    [ProvideDebuggerException(typeof(Antlr4.Runtime.LexerNoViableAltException))]
    [ProvideDebuggerException(typeof(Antlr4.Runtime.NoViableAltException))]
    [ProvideDebuggerException(typeof(Antlr4.Runtime.RecognitionException))]
    [ProvideDebuggerException(typeof(Antlr4.Runtime.Misc.ParseCanceledException))]

    [ProvideAutoLoad(VSConstants.UICONTEXT.CSharpProject_string)]
    public class AntlrLanguagePackage
        : Package
        , IVsTrackProjectDocumentsEvents2
    {
        private static AntlrLanguagePackage _instance;
        private AntlrLanguageInfo _languageInfo;
        private Antlr4LanguageInfo _languageInfo4;
        private GrammarFileObjectExtenderProvider _grammarFileObjectExtenderProvider;
        private int _grammarFileObjectExtenderCookie;
        private uint _trackDocumentsEventsCookie;

        public AntlrLanguagePackage()
        {
            _instance = this;
        }

        public AntlrIntellisenseOptions IntellisenseOptions
        {
            get
            {
                return GetDialogPage<AntlrIntellisenseOptions>();
            }
        }

        protected T GetDialogPage<T>()
            where T : DialogPage
        {
            return (T)base.GetDialogPage(typeof(T));
        }

        protected override void Initialize()
        {
            base.Initialize();

            var serviceProvider = this.AsVsServiceProvider();

            // register the language service
            _languageInfo = new AntlrLanguageInfo(this.AsVsServiceProvider());
            _languageInfo4 = new Antlr4LanguageInfo(this.AsVsServiceProvider());

            ((IServiceContainer)this).AddService(typeof(AntlrLanguageInfo), _languageInfo, true);
            ((IServiceContainer)this).AddService(typeof(Antlr4LanguageInfo), _languageInfo4, true);

            ObjectExtenders objectExtenders = (ObjectExtenders)GetService(typeof(ObjectExtenders));
            _grammarFileObjectExtenderProvider = new GrammarFileObjectExtenderProvider();
            string extenderCatId = Microsoft.VisualStudio.VSConstants.CATID.CSharpFileProperties_string;
            string extenderName = GrammarFileObjectExtenderProvider.Name;
            string localizedName = GrammarFileObjectExtenderProvider.Name;
            _grammarFileObjectExtenderCookie = objectExtenders.RegisterExtenderProvider(extenderCatId, extenderName, _grammarFileObjectExtenderProvider, localizedName);

            IVsTrackProjectDocuments2 trackDocuments2 = serviceProvider.GetTrackProjectDocuments2();
            trackDocuments2.AdviseTrackProjectDocumentsEvents(this, out _trackDocumentsEventsCookie);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_trackDocumentsEventsCookie != 0)
                {
                    var serviceProvider = this.AsVsServiceProvider();
                    IVsTrackProjectDocuments2 trackDocuments = serviceProvider.GetTrackProjectDocuments2();
                    trackDocuments.UnadviseTrackProjectDocumentsEvents(_trackDocumentsEventsCookie);
                    _trackDocumentsEventsCookie = 0;
                }

                if (_grammarFileObjectExtenderCookie != 0)
                {
                    ObjectExtenders objectExtenders = (ObjectExtenders)GetService(typeof(ObjectExtenders));
                    objectExtenders.UnregisterExtenderProvider(_grammarFileObjectExtenderCookie);
                    _grammarFileObjectExtenderCookie = 0;
                }

                if (_languageInfo != null)
                {
                    _languageInfo.Dispose();
                    _languageInfo = null;
                }

                if (_languageInfo4 != null)
                {
                    _languageInfo4.Dispose();
                    _languageInfo4 = null;
                }
            }

            base.Dispose(disposing);
        }

        #region IVsTrackProjectDocumentsEvents2 Members

        public int OnAfterAddDirectoriesEx(int cProjects, int cDirectories, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSADDDIRECTORYFLAGS[] rgFlags)
        {
            return VSConstants.E_NOTIMPL;
        }

        public int OnAfterAddFilesEx(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSADDFILEFLAGS[] rgFlags)
        {
            /* need to make the following alterations:
             *  1. set the Build Action of *.g and *.g3 to Antlr3
             *  2. set the Build Action of *.g4 to Antlr4
             *  3. set the Custom Tool of *.g, *.g3, and *.g4 to MSBuild:Compile
             *  4. set the Custom Tool Namespace of *.g4 to $(RootNamespace) + relative folder path
             *  5. set DependentUpon of *.g.cs, *.g3.cs, and *.g4.cs to the matching *.g, *.g3, or *.g4 file
             */

            for (int i = 0; i < cProjects; i++)
            {
                IVsProject project = rgpProjects[i];
                int projectFiles = (i == cProjects - 1) ? cFiles : rgFirstIndices[i + 1];
                projectFiles -= rgFirstIndices[i];

                if (!IsCSharpProject(project))
                    continue;

                for (int j = 0; j < projectFiles; j++)
                {
                    string currentFile = rgpszMkDocuments[rgFirstIndices[i] + j];
                    if (string.IsNullOrEmpty(currentFile))
                        continue;

                    bool grammarFile =
                        currentFile.EndsWith(".tokens", StringComparison.OrdinalIgnoreCase)
                        || currentFile.EndsWith(".g", StringComparison.OrdinalIgnoreCase)
                        || currentFile.EndsWith(".g3", StringComparison.OrdinalIgnoreCase)
                        || currentFile.EndsWith(".g4", StringComparison.OrdinalIgnoreCase);

                    bool grammarHelperFile = !grammarFile &&
                        (currentFile.EndsWith(".g.cs", StringComparison.OrdinalIgnoreCase)
                        || currentFile.EndsWith(".g3.cs", StringComparison.OrdinalIgnoreCase)
                        || currentFile.EndsWith(".g4.cs", StringComparison.OrdinalIgnoreCase)
                        || currentFile.EndsWith(".g.lexer.cs", StringComparison.OrdinalIgnoreCase)
                        || currentFile.EndsWith(".g3.lexer.cs", StringComparison.OrdinalIgnoreCase)
                        || currentFile.EndsWith(".g4.lexer.cs", StringComparison.OrdinalIgnoreCase)
                        || currentFile.EndsWith(".g.parser.cs", StringComparison.OrdinalIgnoreCase)
                        || currentFile.EndsWith(".g3.parser.cs", StringComparison.OrdinalIgnoreCase)
                        || currentFile.EndsWith(".g4.parser.cs", StringComparison.OrdinalIgnoreCase));

                    if (grammarFile)
                    {
                        OnAfterAddedGrammarFile(project, currentFile);
                    }
                    else if (grammarHelperFile)
                    {
                        OnAfterAddedGrammarHelperFile(project, currentFile);
                    }
                }
            }

            return VSConstants.S_OK;
        }

        private static readonly Guid CSharpProjectTypeGuid = Guid.Parse(PrjKind.prjKindCSharpProject);

        private static bool IsCSharpProject(IVsProject project)
        {
            IVsAggregatableProject aggregatableProject = project as IVsAggregatableProject;
            if (aggregatableProject == null)
                return false;

            string guidsString = null;
            if (ErrorHandler.Failed(ErrorHandler.CallWithCOMConvention(() => aggregatableProject.GetAggregateProjectTypeGuids(out guidsString))))
                return false;

            if (string.IsNullOrWhiteSpace(guidsString))
                return false;

            string[] guids = guidsString.Split(';');
            foreach (var guidString in guids)
            {
                Guid guid;
                if (Guid.TryParse(guidString, out guid) && guid == CSharpProjectTypeGuid)
                    return true;
            }

            return false;
        }

        private void OnAfterAddedGrammarFile(IVsProject project, string currentFile)
        {
            int found;
            VSDOCUMENTPRIORITY[] priority = new VSDOCUMENTPRIORITY[1];
            uint itemId;
            if (ErrorHandler.Failed(project.IsDocumentInProject(currentFile, out found, priority, out itemId)))
                return;

            if (found == 0 || priority[0] != VSDOCUMENTPRIORITY.DP_Standard)
                return;

            string desiredItemType = "Antlr3";
            if (string.Equals(Path.GetExtension(currentFile), ".tokens", StringComparison.OrdinalIgnoreCase))
                desiredItemType = "AntlrTokens";
            else if (string.Equals(Path.GetExtension(currentFile), ".g4", StringComparison.OrdinalIgnoreCase))
                desiredItemType = "Antlr4";

            IVsHierarchy hierarchy = project as IVsHierarchy;
            if (hierarchy != null)
            {
                object browseObject = null;
                PropertyDescriptorCollection propertyDescriptors = null;
                int hr = ErrorHandler.CallWithCOMConvention(() => hierarchy.GetProperty(itemId, (int)__VSHPROPID.VSHPROPID_BrowseObject, out browseObject));
                if (ErrorHandler.Succeeded(hr))
                    propertyDescriptors = TypeDescriptor.GetProperties(browseObject);

                object obj;
                hr = hierarchy.GetProperty(itemId, (int)__VSHPROPID4.VSHPROPID_BuildAction, out obj);
                if (ErrorHandler.Succeeded(hr))
                {
                    string buildAction = obj != null ? obj.ToString() : null;
                    if (string.IsNullOrWhiteSpace(buildAction) || string.Equals(buildAction, "None", StringComparison.OrdinalIgnoreCase))
                    {
                        hr = ErrorHandler.CallWithCOMConvention(() => hierarchy.SetProperty(itemId, (int)__VSHPROPID4.VSHPROPID_BuildAction, desiredItemType));
                    }
                }

                if (ErrorHandler.Failed(hr) && propertyDescriptors != null)
                {
                    PropertyDescriptor itemTypeDescriptor = propertyDescriptors["ItemType"] ?? propertyDescriptors["BuildAction"];
                    if (itemTypeDescriptor != null)
                    {
                        obj = itemTypeDescriptor.GetValue(browseObject);
                        string buildAction = (string)itemTypeDescriptor.Converter.ConvertToInvariantString(obj);
                        if (string.IsNullOrWhiteSpace(buildAction) || string.Equals(buildAction, "None", StringComparison.OrdinalIgnoreCase))
                        {
                            try
                            {
                                obj = itemTypeDescriptor.Converter.ConvertFromInvariantString(desiredItemType);
                                itemTypeDescriptor.SetValue(browseObject, obj);
                            }
                            catch (NotSupportedException)
                            {
                            }
                        }
                    }
                }

                if (propertyDescriptors != null)
                {
                    PropertyDescriptor customToolDescriptor = propertyDescriptors["CustomTool"];
                    if (customToolDescriptor != null)
                    {
                        obj = customToolDescriptor.GetValue(browseObject);
                        string customTool = customToolDescriptor.Converter.ConvertToInvariantString(obj);
                        if (string.IsNullOrWhiteSpace(customTool))
                        {
                            try
                            {
                                obj = customToolDescriptor.Converter.ConvertToInvariantString("MSBuild:Compile");
                                customToolDescriptor.SetValue(browseObject, obj);
                            }
                            catch (NotSupportedException)
                            {
                            }
                        }
                    }

                    PropertyDescriptor customToolNamespaceDescriptor = propertyDescriptors["CustomToolNamespace"];
                    if (customToolNamespaceDescriptor != null)
                    {
                        object defaultNamespace;
                        hr = hierarchy.GetProperty(itemId, (int)__VSHPROPID.VSHPROPID_DefaultNamespace, out defaultNamespace);
                        if (ErrorHandler.Succeeded(hr) && !string.IsNullOrEmpty(defaultNamespace as string))
                        {
                            obj = customToolNamespaceDescriptor.GetValue(browseObject);
                            string customToolNamespace = customToolNamespaceDescriptor.Converter.ConvertToInvariantString(obj);
                            if (string.IsNullOrWhiteSpace(customToolNamespace))
                            {
                                try
                                {
                                    obj = customToolNamespaceDescriptor.Converter.ConvertToInvariantString(defaultNamespace);
                                    customToolNamespaceDescriptor.SetValue(browseObject, obj);
                                }
                                catch (NotSupportedException)
                                {
                                }
                            }
                        }
                    }
                }
            }
        }

        private void OnAfterAddedGrammarHelperFile(IVsProject project, string currentFile)
        {
            int found;
            VSDOCUMENTPRIORITY[] priority = new VSDOCUMENTPRIORITY[1];
            uint itemId;
            if (ErrorHandler.Failed(project.IsDocumentInProject(currentFile, out found, priority, out itemId)))
                return;

            if (found == 0 || priority[0] != VSDOCUMENTPRIORITY.DP_Standard)
                return;

            IVsHierarchy hierarchy = project as IVsHierarchy;
            IVsBuildPropertyStorage buildPropertyStorage = project as IVsBuildPropertyStorage;
            if (hierarchy != null && buildPropertyStorage != null)
            {
                string dependentUpon;
                if (ErrorHandler.Failed(buildPropertyStorage.GetItemAttribute(itemId, "DependentUpon", out dependentUpon)))
                {
                    string[] stripExtensions = { ".cs", ".lexer", ".parser" };
                    string parentFileName = Path.GetFileName(currentFile);
                    while (!string.IsNullOrWhiteSpace(parentFileName) && Array.IndexOf(stripExtensions, Path.GetExtension(parentFileName).ToLowerInvariant()) >= 0)
                    {
                        parentFileName = Path.GetFileNameWithoutExtension(parentFileName);
                    }

                    int hr = buildPropertyStorage.SetItemAttribute(itemId, "DependentUpon", parentFileName);
                }
            }
        }

        public int OnAfterRemoveDirectories(int cProjects, int cDirectories, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSREMOVEDIRECTORYFLAGS[] rgFlags)
        {
            return VSConstants.E_NOTIMPL;
        }

        public int OnAfterRemoveFiles(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSREMOVEFILEFLAGS[] rgFlags)
        {
            return VSConstants.E_NOTIMPL;
        }

        public int OnAfterRenameDirectories(int cProjects, int cDirs, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgszMkOldNames, string[] rgszMkNewNames, VSRENAMEDIRECTORYFLAGS[] rgFlags)
        {
            return VSConstants.E_NOTIMPL;
        }

        public int OnAfterRenameFiles(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgszMkOldNames, string[] rgszMkNewNames, VSRENAMEFILEFLAGS[] rgFlags)
        {
            return VSConstants.E_NOTIMPL;
        }

        public int OnAfterSccStatusChanged(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, uint[] rgdwSccStatus)
        {
            return VSConstants.E_NOTIMPL;
        }

        public int OnQueryAddDirectories(IVsProject pProject, int cDirectories, string[] rgpszMkDocuments, VSQUERYADDDIRECTORYFLAGS[] rgFlags, VSQUERYADDDIRECTORYRESULTS[] pSummaryResult, VSQUERYADDDIRECTORYRESULTS[] rgResults)
        {
            return VSConstants.E_NOTIMPL;
        }

        public int OnQueryAddFiles(IVsProject pProject, int cFiles, string[] rgpszMkDocuments, VSQUERYADDFILEFLAGS[] rgFlags, VSQUERYADDFILERESULTS[] pSummaryResult, VSQUERYADDFILERESULTS[] rgResults)
        {
            return VSConstants.E_NOTIMPL;
        }

        public int OnQueryRemoveDirectories(IVsProject pProject, int cDirectories, string[] rgpszMkDocuments, VSQUERYREMOVEDIRECTORYFLAGS[] rgFlags, VSQUERYREMOVEDIRECTORYRESULTS[] pSummaryResult, VSQUERYREMOVEDIRECTORYRESULTS[] rgResults)
        {
            return VSConstants.E_NOTIMPL;
        }

        public int OnQueryRemoveFiles(IVsProject pProject, int cFiles, string[] rgpszMkDocuments, VSQUERYREMOVEFILEFLAGS[] rgFlags, VSQUERYREMOVEFILERESULTS[] pSummaryResult, VSQUERYREMOVEFILERESULTS[] rgResults)
        {
            return VSConstants.E_NOTIMPL;
        }

        public int OnQueryRenameDirectories(IVsProject pProject, int cDirs, string[] rgszMkOldNames, string[] rgszMkNewNames, VSQUERYRENAMEDIRECTORYFLAGS[] rgFlags, VSQUERYRENAMEDIRECTORYRESULTS[] pSummaryResult, VSQUERYRENAMEDIRECTORYRESULTS[] rgResults)
        {
            return VSConstants.E_NOTIMPL;
        }

        public int OnQueryRenameFiles(IVsProject pProject, int cFiles, string[] rgszMkOldNames, string[] rgszMkNewNames, VSQUERYRENAMEFILEFLAGS[] rgFlags, VSQUERYRENAMEFILERESULTS[] pSummaryResult, VSQUERYRENAMEFILERESULTS[] rgResults)
        {
            return VSConstants.E_NOTIMPL;
        }

        #endregion
    }
}
