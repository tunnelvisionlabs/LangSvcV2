namespace Tvl.VisualStudio.Shell.Implementation
{
    using System;
    using System.ComponentModel.Composition;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell.Interop;
    using Microsoft.VisualStudio.Text.Editor;
    using System.Windows;
    using Microsoft.VisualStudio.Editor;
    using Microsoft.VisualStudio.TextManager.Interop;
    using Microsoft.VisualStudio;

    [Export(typeof(IActiveViewTrackerService))]
    internal sealed class ActiveViewTrackerService : IActiveViewTrackerService
    {
        [Import]
        public IMonitorSelectionService MonitorSelectionService;

        [Import]
        public IOpenedViewTrackerService OpenedViewTrackerService;

        [Import]
        public IVsEditorAdaptersFactoryService VsEditorAdaptersFactoryService;

        public event EventHandler<ViewChangedEventArgs> ViewChanged
        {
            add
            {
                MonitorSelectionService.ViewChanged += value;
            }
            remove
            {
                MonitorSelectionService.ViewChanged -= value;
            }
        }

        public ITextView ActiveView
        {
            get
            {
                return MonitorSelectionService.CurrentView;
            }
        }

        //public ITextView ViewWithMouse
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        public ITextView GetViewUnderPosition(Point screenCoordinates)
        {
            foreach (IWpfTextView view in OpenedViewTrackerService.OpenedViews)
            {
                if (IsViewOnScreen(view))
                {
                    Point location = view.VisualElement.PointToScreen(new Point(0.0, 0.0));
                    Rect rect = new Rect(location, new Size(view.ViewportWidth, view.ViewportHeight));
                    if (rect.Contains(screenCoordinates))
                        return view;
                }
            }

            return null;
        }

        private bool IsViewOnScreen(ITextView view)
        {
            IVsTextView viewAdapter = VsEditorAdaptersFactoryService.GetViewAdapter(view);
            IServiceProvider sp = viewAdapter as IServiceProvider;
            if (sp == null)
                return false;

            IVsWindowFrame frame = sp.GetService(typeof(SVsWindowFrame)) as IVsWindowFrame;
            if (frame == null)
                return false;

            int onScreen = 0;
            if (ErrorHandler.Succeeded(ErrorHandler.CallWithCOMConvention(() => frame.IsOnScreen(out onScreen))))
                return onScreen != 0;

            return false;
        }
    }
}
