namespace Tvl.VisualStudio.Text.Navigation
{
    using System;
    using System.Diagnostics.Contracts;
    using SnapshotSpan = Microsoft.VisualStudio.Text.SnapshotSpan;
    using ImageSource = System.Windows.Media.ImageSource;

    public class EditorNavigationTarget : IEditorNavigationTarget
    {
        private readonly NavigationTargetStyle _style;

        public EditorNavigationTarget(string name, IEditorNavigationType editorNavigationType, SnapshotSpan span, ImageSource glyph = null, NavigationTargetStyle style = NavigationTargetStyle.None)
            : this(name, editorNavigationType, span, new SnapshotSpan(span.Start, span.End), glyph, style)
        {
            Contract.Requires(name != null);
            Contract.Requires(editorNavigationType != null);
        }

        public EditorNavigationTarget(string name, IEditorNavigationType editorNavigationType, SnapshotSpan span, SnapshotSpan seek, ImageSource glyph = null, NavigationTargetStyle style = NavigationTargetStyle.None)
        {
            Contract.Requires<ArgumentNullException>(name != null, "name");
            Contract.Requires<ArgumentNullException>(editorNavigationType != null, "editorNavigationType");

            this.Name = name;
            this.EditorNavigationType = editorNavigationType;
            this.Span = span;
            this.Seek = seek;
            this._style = style;
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

        public bool IsBold
        {
            get
            {
                return (_style & NavigationTargetStyle.Bold) != 0;
            }
        }

        public bool IsGray
        {
            get
            {
                return (_style & NavigationTargetStyle.Gray) != 0;
            }
        }

        public bool IsItalic
        {
            get
            {
                return (_style & NavigationTargetStyle.Italic) != 0;
            }
        }

        public bool IsUnderlined
        {
            get
            {
                return (_style & NavigationTargetStyle.Underlined) != 0;
            }
        }
    }
}
