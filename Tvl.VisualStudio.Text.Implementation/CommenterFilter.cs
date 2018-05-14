namespace Tvl.VisualStudio.Text.Implementation
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using JetBrains.Annotations;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.TextManager.Interop;
    using Tvl.VisualStudio.Shell;

    using VsCommands2K = Microsoft.VisualStudio.VSConstants.VSStd2KCmdID;

    [ComVisible(true)]
    internal class CommenterFilter : TextViewCommandFilter
    {
        public CommenterFilter([NotNull] IVsTextView textViewAdapter, [NotNull] ITextView textView, [NotNull] ICommenter commenter)
            : base(textViewAdapter)
        {
            Debug.Assert(textViewAdapter != null);
            Requires.NotNull(textView, nameof(textView));
            Requires.NotNull(commenter, nameof(commenter));

            this.TextView = textView;
            this.Commenter = commenter;
            textView.Closed += (sender, e) => Dispose();
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

        protected override CommandStatus QueryCommandStatus(ref Guid group, uint id)
        {
            if (group == typeof(VsCommands2K).GUID)
            {
                VsCommands2K cmd = (VsCommands2K)id;
                switch (cmd)
                {
                case VsCommands2K.COMMENT_BLOCK:
                case VsCommands2K.UNCOMMENT_BLOCK:
                    if (TextView.TextBuffer.CheckEditAccess())
                        return CommandStatus.Supported | CommandStatus.Enabled;
                    else
                        return CommandStatus.Supported;
                }
            }

            return base.QueryCommandStatus(ref group, id);
        }

        protected override bool HandlePreExec(ref Guid commandGroup, uint commandId, uint executionOptions, IntPtr pvaIn, IntPtr pvaOut)
        {
            if (commandGroup == typeof(VsCommands2K).GUID)
            {
                VsCommands2K cmd = (VsCommands2K)commandId;
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

            return base.HandlePreExec(ref commandGroup, commandId, executionOptions, pvaIn, pvaOut);
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
