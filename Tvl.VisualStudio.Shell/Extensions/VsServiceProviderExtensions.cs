namespace Tvl.VisualStudio.Shell
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using JetBrains.Annotations;
    using Microsoft.VisualStudio;
#if DEV10ONLY
    using Microsoft.VisualStudio.CallHierarchy.Package.Definitions;
#endif
    using Microsoft.VisualStudio.ComponentModelHost;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using Microsoft.VisualStudio.TextManager.Interop;
    using IGlyphService = Microsoft.VisualStudio.Language.Intellisense.IGlyphService;
    using IOleComponentManager = Microsoft.VisualStudio.OLE.Interop.IOleComponentManager;
    using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
    using SOleComponentManager = Microsoft.VisualStudio.OLE.Interop.SOleComponentManager;

    public static class VsServiceProviderExtensions
    {
        public static IVsActivityLog GetActivityLog([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsActivityLog, IVsActivityLog>();
        }

        public static IVsAddProjectItemDlg GetAddProjectItemDialog([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsAddProjectItemDlg, IVsAddProjectItemDlg>();
        }

        public static IVsAddWebReferenceDlg GetAddWebReferenceDialog([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsAddWebReferenceDlg, IVsAddWebReferenceDlg>();
        }

        public static IVsAppCommandLine GetAppCommandLine([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsAppCommandLine, IVsAppCommandLine>();
        }

        public static IVsAssemblyNameUnification GetAssemblyNameUnification([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsAssemblyNameUnification, IVsAssemblyNameUnification>();
        }

        public static IVsCallBrowser GetCallBrowser([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsCallBrowser, IVsCallBrowser>();
        }

#if DEV10ONLY
        public static ICallHierarchy GetCallHierarchy([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SCallHierarchy, ICallHierarchy>();
        }
#endif

        public static IVsNavigationTool GetClassView([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsClassView, IVsNavigationTool>();
        }

        public static IVsCodeDefView GetCodeDefView([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsCodeDefView, IVsCodeDefView>();
        }

        public static IVsCodeShareHandler GetCodeShareHandler([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsCodeShareHandler, IVsCodeShareHandler>();
        }

        public static IVsCmdNameMapping GetCommandNameMapping([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsCmdNameMapping, IVsCmdNameMapping>();
        }

        public static IVsCommandWindow GetCommandWindow([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsCommandWindow, IVsCommandWindow>();
        }

        public static IVsCommandWindowsCollection GetCommandWindowsCollection([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsCommandWindowsCollection, IVsCommandWindowsCollection>();
        }

        public static IComponentModel GetComponentModel([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SComponentModel, IComponentModel>();
        }

        public static IVsComponentSelectorDlg GetComponentSelectorDialog([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsComponentSelectorDlg, IVsComponentSelectorDlg>();
        }

        public static IVsComponentSelectorDlg2 GetComponentSelectorDialog2([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsComponentSelectorDlg2, IVsComponentSelectorDlg2>();
        }

        public static IVsConfigurationManagerDlg GetConfigurationManagerDialog([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsConfigurationManagerDlg, IVsConfigurationManagerDlg>();
        }

        public static IVsCreateAggregateProject GetCreateAggregateProject([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsCreateAggregateProject, IVsCreateAggregateProject>();
        }

        public static IVsDebuggableProtocol GetDebuggableProtocol([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsDebuggableProtocol, IVsDebuggableProtocol>();
        }

        public static IVsDebugLaunch GetDebugLaunch([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsDebugLaunch, IVsDebugLaunch>();
        }

        public static IVsDetermineWizardTrust GetDetermineWizardTrust([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsDetermineWizardTrust, IVsDetermineWizardTrust>();
        }

        public static IVsDiscoveryService GetDiscoveryService([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsDiscoveryService, IVsDiscoveryService>();
        }

#if false
        // DTE shouldn't be Dte
        [SuppressMessage("Microsoft.Naming", "CA1709")]
        public static DTE GetDTE([NotNull] this SVsServiceProvider serviceProvider)
        {
            if (serviceProvider == null) throw new ArgumentNullException("serviceProvider"); Contract.EndContractBlock();
            {
                return serviceProvider.GetService<EnvDTE._DTE, DTE>();
            }
        }

        // DTE2 shouldn't be Dte2
        [SuppressMessage("Microsoft.Naming", "CA1709")]
        public static DTE2 GetDTE2([NotNull] this SVsServiceProvider serviceProvider)
        {
            if (serviceProvider == null) throw new ArgumentNullException("serviceProvider"); Contract.EndContractBlock();
            {
                return serviceProvider.GetService<EnvDTE._DTE, DTE2>();
            }
        }
#endif

        public static IVsEnumHierarchyItemsFactory GetEnumHierarchyItemsFactory([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsEnumHierarchyItemsFactory, IVsEnumHierarchyItemsFactory>();
        }

        public static IVsErrorList GetErrorList([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsErrorList, IVsErrorList>();
        }

        public static IVsExpansionManager GetExpansionManager([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));

            {
                IVsExpansionManager expMgr;
                var tmgr = serviceProvider.GetTextManager() as IVsTextManager2;
                if (tmgr != null && ErrorHandler.Succeeded(tmgr.GetExpansionManager(out expMgr)))
                    return expMgr;

                return null;
            }
        }

        public static IVsExternalFilesManager GetExternalFilesManager([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsExternalFilesManager, IVsExternalFilesManager>();
        }

        public static IVsFileChangeEx GetFileChange([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsFileChangeEx, IVsFileChangeEx>();
        }

        public static IVsFilterAddProjectItemDlg GetFilterAddProjectItemDialog([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsFilterAddProjectItemDlg, IVsFilterAddProjectItemDlg>();
        }

        public static IVsFilterKeys GetFilterKeys([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsFilterKeys, IVsFilterKeys>();
        }

        public static IVsFindSymbol GetFindSymbol([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsObjectSearch, IVsFindSymbol>();
        }

        public static IVsFontAndColorCacheManager GetFontAndColorCacheManager([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsFontAndColorCacheManager, IVsFontAndColorCacheManager>();
        }

        public static IVsFontAndColorStorage GetFontAndColorStorage([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsFontAndColorStorage, IVsFontAndColorStorage>();
        }

        public static IVsFontAndColorUtilities GetFontAndColorUtilities([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return (IVsFontAndColorUtilities)serviceProvider.GetFontAndColorStorage();
        }

        public static IGlyphService GetGlyphService([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetComponentModel().GetService<IGlyphService>();
        }

        public static IVsHelpSystem GetHelpSystem([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsHelpService, IVsHelpSystem>();
        }

        public static IVsHTMLConverter GetHtmlConverter([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsHTMLConverter, IVsHTMLConverter>();
        }

        public static IVsIME GetIme([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsIME, IVsIME>();
        }

        // spelling as intended
        [SuppressMessage("Microsoft.Naming", "CA1704")]
        public static IVsIntelliMouseHandler GetIntelliMouseHandler([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsIntelliMouseHandler, IVsIntelliMouseHandler>();
        }

        public static IVsIntellisenseEngine GetIntellisenseEngine([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsIntellisenseEngine, IVsIntellisenseEngine>();
        }

        public static IVsIntellisenseProjectHost GetIntellisenseProjectHost([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsIntellisenseProjectHost, IVsIntellisenseProjectHost>();
        }

        public static IVsIntellisenseProjectManager GetIntellisenseProjectManager([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsIntellisenseProjectManager, IVsIntellisenseProjectManager>();
        }

        public static IVsInvisibleEditorManager GetInvisibleEditorManager([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsInvisibleEditorManager, IVsInvisibleEditorManager>();
        }

        public static IVsLaunchPad GetLaunchPad([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsLaunchPad, IVsLaunchPad>();
        }

        public static IVsLaunchPadFactory GetLaunchPadFactory([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsLaunchPadFactory, IVsLaunchPadFactory>();
        }

        public static IVSMDTypeResolutionService GetMDTypeResolutionService([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVSMDTypeResolutionService, IVSMDTypeResolutionService>();
        }

        public static IVsMenuEditor GetMenuEditor([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsMenuEditor, IVsMenuEditor>();
        }

        public static IVsMonitorUserContext GetMonitorUserContext([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsMonitorUserContext, IVsMonitorUserContext>();
        }

        public static IVsMonitorSelection GetMonitorSelection([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<IVsMonitorSelection, IVsMonitorSelection>();
        }

        public static IVsNavigationTool GetObjectBrowser([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsObjBrowser, IVsNavigationTool>();
        }

        public static IOleComponentManager GetOleComponentManager([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SOleComponentManager, IOleComponentManager>();
        }

        public static IVsObjectManager GetObjectManager([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsObjectManager, IVsObjectManager>();
        }

        public static IVsObjectSearch GetObjectSearch([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsObjectSearch, IVsObjectSearch>();
        }

        public static IVsObjectSearchPane GetObjectSearchPane([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetObjectSearch() as IVsObjectSearchPane;
        }

        public static IOleServiceProvider GetOleServiceProvider([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<IOleServiceProvider, IOleServiceProvider>();
        }

        // We don't mean 'Projector'
        [SuppressMessage("Microsoft.Naming", "CA1702")]
        public static IVsOpenProjectOrSolutionDlg GetOpenProjectOrSolutionDialog([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsOpenProjectOrSolutionDlg, IVsOpenProjectOrSolutionDlg>();
        }

        public static IVsOutputWindow GetOutputWindow([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsOutputWindow, IVsOutputWindow>();
        }

        public static IVsParseCommandLine GetParseCommandLine([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsParseCommandLine, IVsParseCommandLine>();
        }

        public static IVsPathVariableResolver GetPathVariableResolver([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsPathVariableResolver, IVsPathVariableResolver>();
        }

        public static IVsPreviewChangesService GetPreviewChangesService([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsPreviewChangesService, IVsPreviewChangesService>();
        }

        public static IVsProfileDataManager GetProfileDataManager([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsProfileDataManager, IVsProfileDataManager>();
        }

        public static IVsProfilesManagerUI GetProfilesManagerUI([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsProfilesManagerUI, IVsProfilesManagerUI>();
        }

        public static IVsPropertyPageFrame GetPropertyPageFrame([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsPropertyPageFrame, IVsPropertyPageFrame>();
        }

        public static IVsQueryEditQuerySave2 GetQueryEditQuerySave([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsQueryEditQuerySave, IVsQueryEditQuerySave2>();
        }

        public static IVsRegisterProjectDebugTargetProvider GetRegisterProjectDebugTargetProvider([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsRegisterDebugTargetProvider, IVsRegisterProjectDebugTargetProvider>();
        }

        public static IVsRegisterEditors GetRegisterEditors([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsRegisterEditors, IVsRegisterEditors>();
        }

        public static IVsRegisterNewDialogFilters GetRegisterNewDialogFilters([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsRegisterNewDialogFilters, IVsRegisterNewDialogFilters>();
        }

        public static IVsRegisterPriorityCommandTarget GetRegisterPriorityCommandTarget([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsRegisterPriorityCommandTarget, IVsRegisterPriorityCommandTarget>();
        }

        public static IVsRegisterProjectTypes GetRegisterProjectTypes([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsRegisterProjectTypes, IVsRegisterProjectTypes>();
        }

        public static IVsResourceManager GetResourceManager([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsResourceManager, IVsResourceManager>();
        }

        public static IVsResourceView GetResourceView([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsResourceView, IVsResourceView>();
        }

        public static IVsRunningDocumentTable GetRunningDocumentTable([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsRunningDocumentTable, IVsRunningDocumentTable>();
        }

        public static IVsSccManager2 GetSourceControlManager([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsSccManager, IVsSccManager2>();
        }

        public static IVsSccToolsOptions GetSourceControlToolsOptions([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsSccToolsOptions, IVsSccToolsOptions>();
        }

        public static IVsSettingsReader GetSettingsReader([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsSettingsReader, IVsSettingsReader>();
        }

        public static IVsShell GetShell([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsShell, IVsShell>();
        }

        public static IVsDebugger2 GetShellDebugger([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsShellDebugger, IVsDebugger2>();
        }

        public static IVsMonitorSelection GetShellMonitorSelection([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsShellMonitorSelection, IVsMonitorSelection>();
        }

        public static IVsSmartOpenScope GetSmartOpenScope([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsSmartOpenScope, IVsSmartOpenScope>();
        }

        public static IVsSolution GetSolution([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsSolution, IVsSolution>();
        }

        public static IVsSolutionBuildManager GetSolutionBuildManager([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsSolutionBuildManager, IVsSolutionBuildManager>();
        }

        [Obsolete("Use VSServices.Solution instead.")]
        public static IVsSolution GetSolutionObject([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsSolutionObject, IVsSolution>();
        }

        public static IVsSolutionPersistence GetSolutionPersistence([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsSolutionPersistence, IVsSolutionPersistence>();
        }

        public static IVsSQLCLRReferences GetSqlClrReferences([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsSQLCLRReferences, IVsSQLCLRReferences>();
        }

        public static IVsStartPageDownload GetStartPageDownload([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsStartPageDownload, IVsStartPageDownload>();
        }

        public static IVsStatusbar GetStatusBar([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsStatusbar, IVsStatusbar>();
        }

        public static IVsStrongNameKeys GetStrongNameKeys([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsStrongNameKeys, IVsStrongNameKeys>();
        }

        public static IVsStructuredFileIO GetStructuredFileIO([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsStructuredFileIO, IVsStructuredFileIO>();
        }

        public static IVsSymbolicNavigationManager GetSymbolicNavigationManager([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsSymbolicNavigationManager, IVsSymbolicNavigationManager>();
        }

        public static IVsTargetFrameworkAssemblies GetTargetFrameworkAssemblies([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsTargetFrameworkAssemblies, IVsTargetFrameworkAssemblies>();
        }

        public static IVsTaskList GetTaskList([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsTaskList, IVsTaskList>();
        }

        public static IVsTextOut GetTextOut([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsTextOut, IVsTextOut>();
        }

        public static IVsTextManager GetTextManager([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<VsTextManagerClass, IVsTextManager>();
        }

        public static IVsTextManager2 GetTextManager2([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));


            return serviceProvider.GetService<VsTextManagerClass, IVsTextManager2>();
        }

        public static IVsThreadedWaitDialog GetThreadedWaitDialog([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsThreadedWaitDialog, IVsThreadedWaitDialog>();
        }

        public static IVsThreadPool GetThreadPool([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsThreadPool, IVsThreadPool>();
        }

        public static IVsToolbox GetToolbox([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsToolbox, IVsToolbox>();
        }

        public static IVsToolboxDataProvider GetToolboxActiveXDataProvider([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsToolboxActiveXDataProvider, IVsToolboxDataProvider>();
        }

        public static IVsToolboxDataProviderRegistry GetToolboxDataProviderRegistry([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsToolboxDataProviderRegistry, IVsToolboxDataProviderRegistry>();
        }

        public static IVsToolsOptions GetToolsOptions([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsToolsOptions, IVsToolsOptions>();
        }

        public static IVsTrackProjectDocuments2 GetTrackProjectDocuments2([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsTrackProjectDocuments, IVsTrackProjectDocuments2>();
        }

        public static IVsTrackProjectDocuments3 GetTrackProjectDocuments3([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsTrackProjectDocuments, IVsTrackProjectDocuments3>();
        }

        public static IVsTrackSelectionEx GetTrackSelection([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsTrackSelectionEx, IVsTrackSelectionEx>();
        }

        public static IVsUIHierWinClipboardHelper GetUIHierarchyWindowClipboardHelper([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsUIHierWinClipboardHelper, IVsUIHierWinClipboardHelper>();
        }

        public static IVsUIShell GetUIShell([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsUIShell, IVsUIShell>();
        }

        public static IVsUIShellDocumentWindowMgr GetUIShellDocumentWindowManager([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsUIShellDocumentWindowMgr, IVsUIShellDocumentWindowMgr>();
        }

        public static IVsUIShellOpenDocument GetUIShellOpenDocument([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsUIShellOpenDocument, IVsUIShellOpenDocument>();
        }

        public static IVsUpgradeLogger GetUpgradeLogger([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsUpgradeLogger, IVsUpgradeLogger>();
        }

        public static IVsWebBrowsingService GetWebBrowsingService([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsWebBrowsingService, IVsWebBrowsingService>();
        }

        public static IVsWebFavorites GetWebFavorites([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsWebFavorites, IVsWebFavorites>();
        }

        public static IVsWebPreview GetWebPreview([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsWebPreview, IVsWebPreview>();
        }

        public static IVsWebProxy GetWebProxy([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsWebProxy, IVsWebProxy>();
        }

        public static IVsWebURLMRU GetWebUrlMru([NotNull] this SVsServiceProvider serviceProvider)
        {
            Requires.NotNull(serviceProvider, nameof(serviceProvider));
            return serviceProvider.GetService<SVsWebURLMRU, IVsWebURLMRU>();
        }
    }
}
