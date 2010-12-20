namespace Tvl.VisualStudio.Language.Php
{
    using System;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Operations;
    using Microsoft.VisualStudio.Utilities;
    using Tvl.VisualStudio.Text;

    [Export(typeof(ICommenterProvider))]
    [ContentType(PhpConstants.PhpContentType)]
    public sealed class PhpCommenterProvider : ICommenterProvider
    {
        private static readonly CommentFormat CommentFormat = new CommentFormat("//", "/*", "*/");

        [Import]
        private ITextUndoHistoryRegistry TextUndoHistoryRegistry
        {
            get;
            set;
        }

        public ICommenter GetCommenter(ITextView textView)
        {
            Func<Commenter> factory = () => new Commenter(textView, TextUndoHistoryRegistry, CommentFormat);
            return textView.Properties.GetOrCreateSingletonProperty<Commenter>(factory);
        }
    }
}
