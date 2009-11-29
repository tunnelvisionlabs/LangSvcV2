namespace Tvl.VisualStudio.Text.Implementation
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.TextManager.Interop;

    using OLECMDF = Microsoft.VisualStudio.OLE.Interop.OLECMDF;
    using VsCommands2K = Microsoft.VisualStudio.VSConstants.VSStd2KCmdID;

    [ComVisible(true)]
    internal class CommenterFilter : TextViewCommandFilter
    {
        public CommenterFilter(IVsTextView textViewAdapter, ITextView textView, ICommenter commenter)
            : base(textViewAdapter)
        {
            this.TextView = textView;
            this.Commenter = commenter;
        }

        public ITextView TextView
        {
            get;
            private set;
        }

        public ICommenter Commenter
        {
            get;
            private set;
        }

        protected override OLECMDF QueryCommandStatus(ref Guid guidCmdGroup, uint cmdId)
        {
            if (guidCmdGroup == typeof(VsCommands2K).GUID)
            {
                VsCommands2K cmd = (VsCommands2K)cmdId;
                switch (cmd)
                {
                case VsCommands2K.COMMENT_BLOCK:
                case VsCommands2K.UNCOMMENT_BLOCK:
                    if (TextView.TextBuffer.CheckEditAccess())
                        return OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_ENABLED;
                    else
                        return OLECMDF.OLECMDF_SUPPORTED;
                }
            }

            return base.QueryCommandStatus(ref guidCmdGroup, cmdId);
        }

        protected override bool HandlePreExec(ref Guid guidCmdGroup, uint cmdId, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            if (guidCmdGroup == typeof(VsCommands2K).GUID)
            {
                VsCommands2K cmd = (VsCommands2K)cmdId;
                switch (cmd)
                {
                case VsCommands2K.COMMENT_BLOCK:
                    this.CommentSelection();
                    return true;

                case VsCommands2K.UNCOMMENT_BLOCK:
                    this.UncommentSelection();
                    return true;
                }
            }

            return base.HandlePreExec(ref guidCmdGroup, cmdId, nCmdexecopt, pvaIn, pvaOut);
        }

        protected void CommentSelection()
        {
            bool reversed = TextView.Selection.IsReversed;
            var newSelection = Commenter.CommentSpans(TextView.Selection.SelectedSpans);
            // TODO: detect rectangle selection if present
            if (newSelection.Count > 0)
                TextView.Selection.Select(newSelection[0], reversed);
        }

        protected void UncommentSelection()
        {
            bool reversed = TextView.Selection.IsReversed;
            var newSelection = Commenter.UncommentSpans(TextView.Selection.SelectedSpans);
            // TODO: detect rectangle selection if present
            if (newSelection.Count > 0)
                TextView.Selection.Select(newSelection[0], reversed);
        }
    }
}
