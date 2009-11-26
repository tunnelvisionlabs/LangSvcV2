namespace JavaLanguageService.Panes
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using JavaLanguageService.Extensions;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell.Interop;
    using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

    [Export(typeof(IOutputWindowService))]
    internal sealed class OutputWindowService : IOutputWindowService
    {
        [Import]
        public IServiceProvider GlobalServiceProvider;

        [ImportMany]
        internal List<Lazy<OutputWindowDefinition, IOutputWindowDefinitionMetadata>> OutputWindowDefinitions;

        private readonly Dictionary<string, Guid> _outputWindows =
            new Dictionary<string, Guid>()
            {
                { PredefinedOutputWindowPanes.Build, VSConstants.GUID_BuildOutputWindowPane },
                { PredefinedOutputWindowPanes.Debug, VSConstants.GUID_OutWindowDebugPane },
                { PredefinedOutputWindowPanes.General, VSConstants.GUID_OutWindowGeneralPane },
            };

        private readonly Dictionary<string, IOutputWindowPane> _panes = new Dictionary<string, IOutputWindowPane>();

        public IOutputWindowPane TryGetPane(string name)
        {
            IOutputWindowPane pane = null;
            if (_panes.TryGetValue(name, out pane))
                return pane;

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

                if (ErrorHandler.Failed(ErrorHandler.CallWithCOMConvention(() => outputWindow.CreatePane(ref guid, definition.Metadata.Name, Convert.ToInt32(visible), Convert.ToInt32(clearWithSolution)))))
                    return null;

                _outputWindows.Add(definition.Metadata.Name, guid);
            }

            IVsOutputWindowPane vspane = null;
            if (ErrorHandler.Failed(ErrorHandler.CallWithCOMConvention(() => outputWindow.GetPane(ref guid, out vspane))))
                return null;

            pane = new VsOutputWindowPaneAdapter(vspane);
            _panes[name] = pane;
            return pane;
        }
    }
}
