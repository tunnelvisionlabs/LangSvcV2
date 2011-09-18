namespace Tvl.VisualStudio.Language.Java.SourceData
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using Antlr.Runtime;
    using Antlr.Runtime.Tree;
    using Interval = Tvl.VisualStudio.Language.Parsing.Collections.Interval;

    partial class IntelliSenseCacheWalker
    {
        private int _errorCount;

        public override void DisplayRecognitionError(string[] tokenNames, RecognitionException e)
        {
            _errorCount++;
            if (_errorCount > 100)
                throw new OperationCanceledException();

            base.DisplayRecognitionError(tokenNames, e);
        }

        private static Interval GetSourceInterval(ITree tree, ITokenStream tokenStream)
        {
            Contract.Requires(tree != null);
            Contract.Requires(tokenStream != null);

            IToken firstToken = tokenStream.Get(tree.TokenStartIndex);
            IToken lastToken = tokenStream.Get(tree.TokenStopIndex);
            return Interval.FromBounds(firstToken.StartIndex, lastToken.StopIndex);
        }

        private static Interval GetSourceInterval(IToken token)
        {
            Contract.Requires(token != null);

            return Interval.FromBounds(token.StartIndex, token.StopIndex);
        }

        private Interval GetSourceInterval(IEnumerable<ITree> trees)
        {
            Contract.Requires(trees != null);

            IEnumerable<Interval> intervals = trees.Select(GetSourceInterval);
            int start = intervals.Min(i => i.Start);
            int endInclusive = intervals.Max(i => i.EndInclusive);
            return Interval.FromBounds(start, endInclusive);
        }

        private Interval GetSourceInterval(ITree tree)
        {
            Contract.Requires(tree != null);

            return GetSourceInterval(tree, this.input.TokenStream);
        }
    }
}
