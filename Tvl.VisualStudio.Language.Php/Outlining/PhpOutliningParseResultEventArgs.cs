namespace Tvl.VisualStudio.Language.Php.Outlining
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Antlr4.Runtime;
    using JetBrains.Annotations;
    using Microsoft.VisualStudio.Text;

    using AntlrParseResultEventArgs = Tvl.VisualStudio.Language.Parsing4.AntlrParseResultEventArgs;
    using ParseErrorEventArgs = Tvl.VisualStudio.Language.Parsing.ParseErrorEventArgs;

    internal class PhpOutliningParseResultEventArgs : AntlrParseResultEventArgs
    {
        private readonly ReadOnlyCollection<ParserRuleContext> _outliningTrees;

        public PhpOutliningParseResultEventArgs(ITextSnapshot snapshot, IList<ParseErrorEventArgs> errors, TimeSpan elapsedTime, IList<IToken> tokens, ParserRuleContext result, [NotNull] ReadOnlyCollection<ParserRuleContext> outliningTrees)
            : base(snapshot, errors, elapsedTime, tokens, result)
        {
            Requires.NotNull(outliningTrees, nameof(outliningTrees));

            _outliningTrees = outliningTrees;
        }

        public ReadOnlyCollection<ParserRuleContext> OutliningTrees
        {
            get
            {
                return _outliningTrees;
            }
        }
    }
}
