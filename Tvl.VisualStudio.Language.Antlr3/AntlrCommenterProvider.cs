namespace Tvl.VisualStudio.Language.Antlr3
{
    using System;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Operations;
    using Microsoft.VisualStudio.Utilities;
    using Tvl.VisualStudio.Text;

    [Export(typeof(ICommenterProvider))]
    [ContentType(AntlrConstants.AntlrContentType)]
    public sealed class AntlrCommenterProvider : ICommenterProvider
    {
        private static readonly CommentFormat AntlrCommentFormat = new CommentFormat("//", "/*", "*/");

        [Import]
        private ITextUndoHistoryRegistry TextUndoHistoryRegistry
        {
            get;
            set;
        }

        public ICommenter GetCommenter(ITextView textView)
        {
            Func<Commenter> factory = () => new Commenter(textView, TextUndoHistoryRegistry, AntlrCommentFormat);
            return textView.Properties.GetOrCreateSingletonProperty<Commenter>(factory);
        }
    }
}
