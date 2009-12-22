namespace JavaLanguageService.Php
{
    using System;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.UI.Undo;
    using Microsoft.VisualStudio.Utilities;
    using Tvl.VisualStudio.Text;

    [Export(typeof(ICommenterProvider))]
    [ContentType(PhpConstants.PhpContentType)]
    public sealed class PhpCommenterProvider : ICommenterProvider
    {
        private static readonly CommentFormat CommentFormat = new CommentFormat("//", "/*", "*/");

        [Import]
        private IUndoHistoryRegistry UndoHistoryRegistry
        {
            get;
            set;
        }

        public ICommenter GetCommenter(ITextView textView)
        {
            Func<Commenter> factory = () => new Commenter(textView, UndoHistoryRegistry, CommentFormat);
            return textView.Properties.GetOrCreateSingletonProperty<Commenter>(factory);
        }
    }
}
