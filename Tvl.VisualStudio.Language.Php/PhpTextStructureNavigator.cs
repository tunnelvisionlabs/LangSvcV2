namespace Tvl.VisualStudio.Language.Php
{
    using System;
    using System.Diagnostics.Contracts;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Operations;
    using Microsoft.VisualStudio.Utilities;

    internal class PhpTextStructureNavigator : ITextStructureNavigator
    {
        private readonly ITextBuffer _textBuffer;
        private readonly ITextStructureNavigator _delegateNavigator;

        public PhpTextStructureNavigator(ITextBuffer textBuffer, ITextStructureNavigator delegateNavigator)
        {
            Contract.Requires<ArgumentNullException>(textBuffer != null, "textBuffer");
            Contract.Requires<ArgumentNullException>(delegateNavigator != null, "delegateNavigator");

            _textBuffer = textBuffer;
            _delegateNavigator = delegateNavigator;
        }

        public ITextBuffer TextBuffer
        {
            get
            {
                return _textBuffer;
            }
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
            TextExtent extent = _delegateNavigator.GetExtentOfWord(currentPosition);
            if (extent.IsSignificant)
            {
                var span = extent.Span;
                if (span.Start > 0 && IsIdentifierChar(span.Start.GetChar()) && IsIdentifierStartChar((span.Start - 1).GetChar()))
                    extent = new TextExtent(new SnapshotSpan(span.Start - 1, span.End), true);
            }

            return extent;
        }

        private bool IsIdentifierStartChar(char c)
        {
            if (char.IsLetter(c))
                return true;

            if (c == '_' || c == '$')
                return true;

            return false;
        }

        private bool IsIdentifierChar(char c)
        {
            return IsIdentifierStartChar(c) || char.IsDigit(c);
        }

        public SnapshotSpan GetSpanOfEnclosing(SnapshotSpan activeSpan)
        {
            return _delegateNavigator.GetSpanOfEnclosing(activeSpan);
        }

        public SnapshotSpan GetSpanOfFirstChild(SnapshotSpan activeSpan)
        {
            return _delegateNavigator.GetSpanOfFirstChild(activeSpan);
        }

        public SnapshotSpan GetSpanOfNextSibling(SnapshotSpan activeSpan)
        {
            return _delegateNavigator.GetSpanOfNextSibling(activeSpan);
        }

        public SnapshotSpan GetSpanOfPreviousSibling(SnapshotSpan activeSpan)
        {
            return _delegateNavigator.GetSpanOfPreviousSibling(activeSpan);
        }
    }
}
