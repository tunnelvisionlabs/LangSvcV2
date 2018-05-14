namespace Tvl.VisualStudio.Language.Alloy
{
    using System;
    using System.ComponentModel.Composition;
    using JetBrains.Annotations;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Operations;
    using Microsoft.VisualStudio.Utilities;
    using Tvl.VisualStudio.Text;

    [Export(typeof(ICommenterProvider))]
    [ContentType(AlloyConstants.AlloyContentType)]
    public sealed class AlloyCommenterProvider : ICommenterProvider
    {
        private static readonly LineCommentFormat LineCommentFormat = new LineCommentFormat("//");
        private static readonly LineCommentFormat LineCommentFormat2 = new LineCommentFormat("--");
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

            Func<Commenter> factory = () => new Commenter(textView, TextUndoHistoryRegistry, LineCommentFormat, LineCommentFormat2, BlockCommentFormat);
            return textView.Properties.GetOrCreateSingletonProperty<Commenter>(factory);
        }
    }
}
