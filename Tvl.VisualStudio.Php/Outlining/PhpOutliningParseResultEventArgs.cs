namespace Tvl.VisualStudio.Language.Php.Outlining
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using Antlr.Runtime;
    using Antlr.Runtime.Tree;
    using Microsoft.VisualStudio.Text;
    using Tvl.VisualStudio.Language.Parsing;

    internal class PhpOutliningParseResultEventArgs : AntlrParseResultEventArgs
    {
        private readonly ReadOnlyCollection<CommonTree> _outliningTrees;

        public PhpOutliningParseResultEventArgs(ITextSnapshot snapshot, IList<ParseErrorEventArgs> errors, TimeSpan elapsedTime, IList<IToken> tokens, IRuleReturnScope result, ReadOnlyCollection<CommonTree> outliningTrees)
            : base(snapshot, errors, elapsedTime, tokens, result)
        {
            Contract.Requires<ArgumentNullException>(outliningTrees != null, "outliningTrees");

            _outliningTrees = outliningTrees;
        }

        public ReadOnlyCollection<CommonTree> OutliningTrees
        {
            get
            {
                return _outliningTrees;
            }
        }
    }
}
