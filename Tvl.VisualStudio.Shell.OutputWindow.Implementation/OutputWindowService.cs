namespace Tvl.VisualStudio.Shell.OutputWindow.Implementation
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using Tvl.VisualStudio.Shell.OutputWindow.Interfaces;

    using Dispatcher = System.Windows.Threading.Dispatcher;
    using DispatcherPriority = System.Windows.Threading.DispatcherPriority;
    using ErrorHandler = Microsoft.VisualStudio.ErrorHandler;
    using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
    using IVsOutputWindow = Microsoft.VisualStudio.Shell.Interop.IVsOutputWindow;
    using IVsOutputWindowPane = Microsoft.VisualStudio.Shell.Interop.IVsOutputWindowPane;
    using SVsOutputWindow = Microsoft.VisualStudio.Shell.Interop.SVsOutputWindow;
    using SVsServiceProvider = Microsoft.VisualStudio.Shell.SVsServiceProvider;
    using Thread = System.Threading.Thread;
    using VSConstants = Microsoft.VisualStudio.VSConstants;

    [Export(typeof(IOutputWindowService))]
    internal sealed class OutputWindowService : IOutputWindowService
    {
        public OutputWindowService()
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
        }

        [Import]
        private SVsServiceProvider GlobalServiceProvider
        {
            get;
            set;
        }

        [ImportMany]
        private List<Lazy<OutputWindowDefinition, IOutputWindowDefinitionMetadata>> OutputWindowDefinitions
        {
            get;
            set;
        }

        private Dispatcher Dispatcher
        {
            get;
            set;
        }

        private readonly Dictionary<string, Guid> _outputWindows =
            new Dictionary<string, Guid>()
            {
                { PredefinedOutputWindowPanes.Build, VSConstants.OutputWindowPaneGuid.BuildOutputPane_guid },
                { PredefinedOutputWindowPanes.Debug, VSConstants.OutputWindowPaneGuid.DebugPane_guid },
                { PredefinedOutputWindowPanes.General, VSConstants.OutputWindowPaneGuid.GeneralPane_guid },
            };

        private readonly ConcurrentDictionary<string, IOutputWindowPane> _panes =
            new ConcurrentDictionary<string, IOutputWindowPane>();

        public IOutputWindowPane TryGetPane(string name)
        {
            return _panes.GetOrAdd(name, CreateWindowPaneOnMainThread);
        }

        private IOutputWindowPane CreateWindowPaneOnMainThread(string name)
        {
            if (Dispatcher.Thread == Thread.CurrentThread)
                return CreateWindowPane(name);

            return (IOutputWindowPane)Dispatcher.Invoke(DispatcherPriority.Normal, (Func<string, IOutputWindowPane>)CreateWindowPane, name);
        }

        private IOutputWindowPane CreateWindowPane(string name)
        {
            var olesp = (IOleServiceProvider)GlobalServiceProvider.GetService(typeof(IOleServiceProvider));
            var outputWindow = olesp.TryGetGlobalService<SVsOutputWindow, IVsOutputWindow>();
            if (outputWindow == null)
                return null;

            Guid guid;
            if (!_outputWindows.TryGetValue(name, out guid))
            {
                var definition = OutputWindowDefinitions.FirstOrDefault(lazy => lazy.Metadata.Name.Equals(name));
                if (definition == null)
                    return null;

                guid = Guid.NewGuid();
                // this controls whether the pane is listed in the output panes dropdown list, *not* whether the pane is initially selected
                bool visible = true;
                bool clearWithSolution = false;

                string displayName = definition.Metadata.Name;
                if (definition.Value != null && !string.IsNullOrEmpty(definition.Value.DisplayName))
                    displayName = definition.Value.DisplayName;

                if (ErrorHandler.Failed(ErrorHandler.CallWithCOMConvention(() => outputWindow.CreatePane(ref guid, displayName, Convert.ToInt32(visible), Convert.ToInt32(clearWithSolution)))))
                    return null;

                _outputWindows.Add(definition.Metadata.Name, guid);
            }

            IVsOutputWindowPane vspane = null;
            if (ErrorHandler.Failed(ErrorHandler.CallWithCOMConvention(() => outputWindow.GetPane(ref guid, out vspane))))
                return null;

            IOutputWindowPane pane = new VsOutputWindowPaneAdapter(vspane);
            return pane;
        }
    }
}
