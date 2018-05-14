namespace Tvl.VisualStudio.Language.Intellisense
{
    using JetBrains.Annotations;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using Microsoft.VisualStudio.TextManager.Interop;

    public class FileSystemNavigateToTarget : INavigateToTarget
    {
        public FileSystemNavigateToTarget([NotNull] string fileName, int line, int column, [NotNull] ServiceProvider serviceProvider)
        {
            Requires.NotNullOrEmpty(fileName, nameof(fileName));
            Requires.NotNull(serviceProvider, nameof(serviceProvider));

            FileName = fileName;
            Line = line;
            Column = column;
            ServiceProvider = serviceProvider;
        }

        public string FileName
        {
            get;
            protected set;
        }

        public int Line
        {
            get;
            protected set;
        }

        public int Column
        {
            get;
            protected set;
        }

        public ServiceProvider ServiceProvider
        {
            get;
            private set;
        }

        public virtual void NavigateTo()
        {
            // Open the referenced document, and scroll to the given location.
            IVsUIHierarchy hierarchy;
            uint itemID;
            IVsWindowFrame frame;
            IVsTextView view;

            VsShellUtilities.OpenDocument(ServiceProvider, FileName, VSConstants.LOGVIEWID.Code_guid, out hierarchy, out itemID, out frame, out view);
            if (view != null)
            {
                TextSpan span = new TextSpan()
                {
                    iStartLine = Line,
                    iStartIndex = Column,
                    iEndLine = Line,
                    iEndIndex = Column
                };
                ErrorHandler.ThrowOnFailure(view.EnsureSpanVisible(span));
                ErrorHandler.ThrowOnFailure(view.SetSelection(span.iStartLine, span.iStartIndex, span.iEndLine, span.iEndIndex));
            }
        }
    }
}
