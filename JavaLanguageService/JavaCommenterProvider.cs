namespace JavaLanguageService
{
    using System;
    using System.ComponentModel.Composition;
    using JavaLanguageService.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.UI.Undo;
    using Microsoft.VisualStudio.Utilities;
    using Tvl.VisualStudio.Text;

    [Export(typeof(ICommenterProvider))]
    [ContentType(Constants.JavaContentType)]
    public sealed class JavaCommenterProvider : ICommenterProvider
    {
        private static readonly CommentFormat JavaCommentFormat = new CommentFormat("//", "/*", "*/");

        [Import]
        private IUndoHistoryRegistry UndoHistoryRegistry
        {
            get;
            set;
        }

        public ICommenter GetCommenter(ITextView textView)
        {
            Func<Commenter> factory = () => new Commenter(textView, UndoHistoryRegistry, JavaCommentFormat);
            return textView.Properties.GetOrCreateSingletonProperty<Commenter>(factory);
        }
    }
}
