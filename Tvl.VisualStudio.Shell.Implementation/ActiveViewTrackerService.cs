namespace Tvl.VisualStudio.Shell.Implementation
{
    using System;
    using System.ComponentModel.Composition;
    using System.Diagnostics.Contracts;
    using System.Windows;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Editor;
    using Microsoft.VisualStudio.Shell.Interop;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.TextManager.Interop;

    [Export(typeof(IActiveViewTrackerService))]
    internal sealed class ActiveViewTrackerService : IActiveViewTrackerService
    {
        [Import]
        private IMonitorSelectionService MonitorSelectionService
        {
            get;
            set;
        }

        [Import]
        private IOpenedViewTrackerService OpenedViewTrackerService
        {
            get;
            set;
        }

        [Import]
        private IVsEditorAdaptersFactoryService VsEditorAdaptersFactoryService
        {
            get;
            set;
        }

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

        public event EventHandler<ViewChangedEventArgs> ViewWithMouseChanged
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        public ITextView ActiveView
        {
            get
            {
                return MonitorSelectionService.CurrentView;
            }
        }

        public ITextView ViewWithMouse
        {
            get
            {
                throw new NotImplementedException();
            }
        }

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
            Contract.Requires<ArgumentNullException>(view != null, "view");

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
