namespace Tvl.VisualStudio.Language.Parsing
{
    using System.Collections.Generic;
    using Antlr.Runtime;
    using Microsoft.VisualStudio.Text;
    using System.Collections.ObjectModel;
    using System.Linq;

    public class AntlrParseResultEventArgs : ParseResultEventArgs
    {
        public AntlrParseResultEventArgs(ITextSnapshot snapshot, IList<ParseErrorEventArgs> errors, IList<IToken> tokens, ParserRuleReturnScope result)
            : base(snapshot, errors)
        {
            Tokens = tokens as ReadOnlyCollection<IToken>;
            if (Tokens == null)
                Tokens = new ReadOnlyCollection<IToken>(tokens);

            Result = result;
        }

        public ReadOnlyCollection<IToken> Tokens
        {
            get;
            private set;
        }

        public ParserRuleReturnScope Result
        {
            get;
            private set;
        }
    }
}
