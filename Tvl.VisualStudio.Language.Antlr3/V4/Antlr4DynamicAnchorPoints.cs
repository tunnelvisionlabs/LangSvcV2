namespace Tvl.VisualStudio.Language.AntlrV4
{
    using System;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.Text;

    internal class Antlr4DynamicAnchorPoints
    {
        private readonly Antlr4DynamicAnchorPointsProvider _provider;
        private readonly ITextBuffer _textBuffer;

        public Antlr4DynamicAnchorPoints(Antlr4DynamicAnchorPointsProvider provider, ITextBuffer textBuffer)
        {
            _provider = provider;
            _textBuffer = textBuffer;
        }

        public IList<IAnchor> GetValue(ITextSnapshot snapshot, ParserDataOptions options)
        {
            if (_textBuffer != snapshot.TextBuffer)
                throw new ArgumentException();

            Antlr4ReferenceAnchorPoints referenceAnchorPoints = _provider.ReferenceAnchorPointsProvider.GetReferenceAnchorPoints(_textBuffer);
            if (referenceAnchorPoints == null)
                return new IAnchor[0];

            IList<IAnchor> referenceAnchors = referenceAnchorPoints.GetValue(snapshot, ParserDataOptions.AllowStale);
            return referenceAnchors;
        }
    }
}
