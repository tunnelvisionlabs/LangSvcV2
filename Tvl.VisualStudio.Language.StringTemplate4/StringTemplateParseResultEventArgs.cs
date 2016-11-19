namespace Tvl.VisualStudio.Language.StringTemplate4
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Antlr.Runtime;
    using Microsoft.VisualStudio.Text;
    using Tvl.VisualStudio.Language.Parsing;

    internal class StringTemplateParseResultEventArgs : AntlrParseResultEventArgs
    {
        public StringTemplateParseResultEventArgs(ITextSnapshot snapshot, IList<ParseErrorEventArgs> errors, TimeSpan elapsedTime, IList<IToken> tokens, IRuleReturnScope result, IList<Antlr4.Runtime.IToken> tokens4, Antlr4.Runtime.ParserRuleContext result4)
            : base(snapshot, errors, elapsedTime, tokens, result)
        {
            Tokens4 = tokens as ReadOnlyCollection<Antlr4.Runtime.IToken>;
            if (Tokens == null)
                Tokens4 = new ReadOnlyCollection<Antlr4.Runtime.IToken>(tokens4 ?? new Antlr4.Runtime.IToken[0]);

            Result4 = result4;
        }

        public ReadOnlyCollection<Antlr4.Runtime.IToken> Tokens4
        {
            get;
            private set;
        }

        public Antlr4.Runtime.ParserRuleContext Result4
        {
            get;
            private set;
        }
    }
}