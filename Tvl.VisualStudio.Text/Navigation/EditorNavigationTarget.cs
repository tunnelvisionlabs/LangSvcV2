namespace Tvl.VisualStudio.Text.Navigation
{
    using System;
    using System.Windows.Media;
    using Microsoft.VisualStudio.Text;
    using System.Diagnostics.Contracts;

    public class EditorNavigationTarget : IEditorNavigationTarget
    {
        public EditorNavigationTarget(string name, IEditorNavigationType editorNavigationType, SnapshotSpan span)
            : this(name, editorNavigationType, span, new SnapshotSpan(span.Start, span.End), null)
        {
            Contract.Requires(name != null);
            Contract.Requires(editorNavigationType != null);
        }

        public EditorNavigationTarget(string name, IEditorNavigationType editorNavigationType, SnapshotSpan span, ImageSource glyph)
            : this(name, editorNavigationType, span, new SnapshotSpan(span.Start, span.End), glyph)
        {
            Contract.Requires(name != null);
            Contract.Requires(editorNavigationType != null);
        }

        public EditorNavigationTarget(string name, IEditorNavigationType editorNavigationType, SnapshotSpan span, SnapshotSpan seek)
            : this(name, editorNavigationType, span, seek, null)
        {
            Contract.Requires(name != null);
            Contract.Requires(editorNavigationType != null);
        }

        public EditorNavigationTarget(string name, IEditorNavigationType editorNavigationType, SnapshotSpan span, SnapshotSpan seek, ImageSource glyph)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (editorNavigationType == null)
                throw new ArgumentNullException("editorNavigationType");
            Contract.EndContractBlock();

            this.Name = name;
            this.EditorNavigationType = editorNavigationType;
            this.Span = span;
            this.Seek = seek;
            this.Glyph = glyph;
        }

        public IEditorNavigationType EditorNavigationType
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            private set;
        }

        public ImageSource Glyph
        {
            get;
            private set;
        }

        public SnapshotSpan Seek
        {
            get;
            private set;
        }

        public SnapshotSpan Span
        {
            get;
            private set;
        }
    }
}
