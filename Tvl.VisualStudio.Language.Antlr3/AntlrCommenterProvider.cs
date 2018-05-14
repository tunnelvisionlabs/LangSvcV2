namespace Tvl.VisualStudio.Language.Antlr3
{
    using System;
    using System.ComponentModel.Composition;
    using JetBrains.Annotations;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Operations;
    using Microsoft.VisualStudio.Utilities;
    using Tvl.VisualStudio.Text;

    [Export(typeof(ICommenterProvider))]
    [ContentType(AntlrConstants.AntlrContentType)]
    public sealed class AntlrCommenterProvider : ICommenterProvider
    {
        private static readonly LineCommentFormat LineCommentFormat = new LineCommentFormat("//");
        private static readonly BlockCommentFormat BlockCommentFormat = new BlockCommentFormat("/*", "*/");

        [Import]
        private ITextUndoHistoryRegistry TextUndoHistoryRegistry
        {
            get;
            set;
        }

        public ICommenter GetCommenter([NotNull] ITextView textView)
        {
            Requires.NotNull(textView, nameof(textView));

            Func<Commenter> factory = () => new Commenter(textView, TextUndoHistoryRegistry, LineCommentFormat, BlockCommentFormat);
            return textView.Properties.GetOrCreateSingletonProperty<Commenter>(factory);
        }
    }
}
