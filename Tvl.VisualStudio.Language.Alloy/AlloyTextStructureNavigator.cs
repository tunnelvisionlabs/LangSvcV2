namespace Tvl.VisualStudio.Language.Alloy
{
    using System;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Operations;
    using Microsoft.VisualStudio.Utilities;

    internal class AlloyTextStructureNavigator : ITextStructureNavigator
    {
        //private readonly AlloyTextStructureNavigatorProvider _provider;
        private readonly ITextBuffer _textBuffer;
        private readonly ITextStructureNavigator _delegateTextStructureNavigator;

        public AlloyTextStructureNavigator(ITextBuffer textBuffer, ITextStructureNavigator delegateTextStructureNavigator)
        {
            if (textBuffer == null)
                throw new ArgumentNullException("textBuffer");
            if (delegateTextStructureNavigator == null)
                throw new ArgumentNullException("delegateTextStructureNavigator");

            this._textBuffer = textBuffer;
            this._delegateTextStructureNavigator = delegateTextStructureNavigator;
        }

        public IContentType ContentType
        {
            get
            {
                return _textBuffer.ContentType;
            }
        }

        public TextExtent GetExtentOfWord(SnapshotPoint currentPosition)
        {
            return _delegateTextStructureNavigator.GetExtentOfWord(currentPosition);
        }

        public SnapshotSpan GetSpanOfEnclosing(SnapshotSpan activeSpan)
        {
            return _delegateTextStructureNavigator.GetSpanOfEnclosing(activeSpan);
        }

        public SnapshotSpan GetSpanOfFirstChild(SnapshotSpan activeSpan)
        {
            return _delegateTextStructureNavigator.GetSpanOfFirstChild(activeSpan);
        }

        public SnapshotSpan GetSpanOfNextSibling(SnapshotSpan activeSpan)
        {
            return _delegateTextStructureNavigator.GetSpanOfNextSibling(activeSpan);
        }

        public SnapshotSpan GetSpanOfPreviousSibling(SnapshotSpan activeSpan)
        {
            return _delegateTextStructureNavigator.GetSpanOfPreviousSibling(activeSpan);
        }
    }
}
