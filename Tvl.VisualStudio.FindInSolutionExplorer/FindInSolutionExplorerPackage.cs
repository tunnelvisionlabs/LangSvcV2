namespace Tvl.VisualStudio.FindInSolutionExplorer
{
    using System;
    using System.ComponentModel.Design;
    using System.Linq;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;
    using ErrorHandler = Microsoft.VisualStudio.ErrorHandler;
    using IMenuCommandService = System.ComponentModel.Design.IMenuCommandService;
    using UICONTEXT = Microsoft.VisualStudio.VSConstants.UICONTEXT;

    [Guid(FindInSolutionExplorerConstants.guidFindInSolutionExplorerPackageString)]
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [ProvideMenuResource(1000, 1)]
    [ProvideAutoLoad(UICONTEXT.SolutionExists_string)]
    public class FindInSolutionExplorerPackage : Package
    {
        private static FindInSolutionExplorerPackage _instance;

        private OleMenuCommand _command;

        public FindInSolutionExplorerPackage()
        {
            _instance = this;
        }

        public static FindInSolutionExplorerPackage Instance
        {
            get
            {
                return _instance;
            }
        }

        public SVsServiceProvider ServiceProvider
        {
            get
            {
                return new VsServiceProviderWrapper(this);
            }
        }

        public EnvDTE80.DTE2 ApplicationObject
        {
            get
            {
                return ServiceProvider.GetService(typeof(EnvDTE._DTE)) as EnvDTE80.DTE2;
            }
        }

        protected override void Initialize()
        {
            base.Initialize();
            IMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as IMenuCommandService;

            CommandID id = new CommandID(FindInSolutionExplorerConstants.guidFindInSolutionExplorerCommandSet, FindInSolutionExplorerConstants.cmdidFindInSolutionExplorer);
            EventHandler invokeHandler = HandleInvokeFindInSolutionExplorer;
            EventHandler changeHandler = HandleChangeFindInSolutionExplorer;
            EventHandler beforeQueryStatus = HandleBeforeQueryStatusFindInSolutionExplorer;
            _command = new OleMenuCommand(invokeHandler, changeHandler, beforeQueryStatus, id);
            mcs.AddCommand(_command);
        }

        public static EnvDTE80.Window2 FindWindow(EnvDTE80.Windows2 windows, EnvDTE.vsWindowType vsWindowType)
        {
            return windows.Cast<EnvDTE80.Window2>().FirstOrDefault(w => w.Type == vsWindowType);
        }

        private void HandleInvokeFindInSolutionExplorer(object sender, EventArgs e)
        {
            try
            {
                EnvDTE.Property track = ApplicationObject.get_Properties("Environment", "ProjectsAndSolution").Item("TrackFileSelectionInExplorer");
                if (track.Value is bool && !((bool)track.Value))
                {
                    track.Value = true;
                    track.Value = false;
                }

                // Find the Solution Explorer object
                EnvDTE80.Windows2 windows = ApplicationObject.Windows as EnvDTE80.Windows2;
                EnvDTE80.Window2 solutionExplorer = FindWindow(windows, EnvDTE.vsWindowType.vsWindowTypeSolutionExplorer);
                if (solutionExplorer != null)
                    solutionExplorer.Activate();
            }
            catch (Exception ex)
            {
                if (ErrorHandler.IsCriticalException(ex))
                    throw;
            }
        }

        private void HandleChangeFindInSolutionExplorer(object sender, EventArgs e)
        {
        }

        private void HandleBeforeQueryStatusFindInSolutionExplorer(object sender, EventArgs e)
        {
            try
            {
                EnvDTE.Document doc = ApplicationObject.ActiveDocument;

                _command.Supported = true;

                bool enabled = false;
                EnvDTE.ProjectItem projectItem = doc != null ? doc.ProjectItem : null;
                if (projectItem != null)
                {
                    if (projectItem.Document != null)
                    {
                        // normal project documents
                        enabled = true;
                    }
                    else if (projectItem.ContainingProject != null)
                    {
                        // this applies to files in the "Solution Files" folder
                        enabled = projectItem.ContainingProject.Object != null;
                    }
                }

                _command.Enabled = enabled;
            }
            catch (ArgumentException)
            {
                // stupid thing throws if the active window is a C# project properties pane
                _command.Supported = false;
                _command.Enabled = false;
            }
            catch (Exception ex)
            {
                if (ErrorHandler.IsCriticalException(ex))
                    throw;

                _command.Supported = false;
                _command.Enabled = false;
            }
        }
    }
}
