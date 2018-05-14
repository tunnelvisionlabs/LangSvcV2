namespace Tvl.VisualStudio.Text.Navigation
{
    using System.Diagnostics;
    using JetBrains.Annotations;
    using ImageSource = System.Windows.Media.ImageSource;
    using SnapshotSpan = Microsoft.VisualStudio.Text.SnapshotSpan;

    public class EditorNavigationTarget : IEditorNavigationTarget
    {
        private readonly string _name;
        private readonly IEditorNavigationType _editorNavigationType;
        private readonly NavigationTargetStyle _style;

        public EditorNavigationTarget([NotNull] string name, [NotNull] IEditorNavigationType editorNavigationType, SnapshotSpan span, ImageSource glyph = null, NavigationTargetStyle style = NavigationTargetStyle.None)
            : this(name, editorNavigationType, span, new SnapshotSpan(span.Start, span.End), glyph, style)
        {
            Debug.Assert(name != null);
            Debug.Assert(editorNavigationType != null);
        }

        public EditorNavigationTarget([NotNull] string name, [NotNull] IEditorNavigationType editorNavigationType, SnapshotSpan span, SnapshotSpan seek, ImageSource glyph = null, NavigationTargetStyle style = NavigationTargetStyle.None)
        {
            Requires.NotNull(name, nameof(name));
            Requires.NotNull(editorNavigationType, nameof(editorNavigationType));

            this._name = name;
            this._editorNavigationType = editorNavigationType;
            this.Span = span;
            this.Seek = seek;
            this._style = style;
            this.Glyph = glyph;
        }

        [NotNull]
        public string Name
        {
            get
            {
                return _name;
            }
        }

        [NotNull]
        public IEditorNavigationType EditorNavigationType
        {
            get
            {
                return _editorNavigationType;
            }
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
