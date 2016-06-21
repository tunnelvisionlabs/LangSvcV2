namespace Tvl.VisualStudio.Language.StringTemplate4
{
    using System;
    using System.Diagnostics.Contracts;
    using Antlr.Runtime;

    partial class InsideClassifierLexer
    {
        internal InsideClassifierLexer(ICharStream input, ClassifierLexer lexer)
            : this(input)
        {
            Contract.Requires<ArgumentNullException>(lexer != null, "lexer");
            AggregateLexer = lexer;
        }

        private ClassifierLexer AggregateLexer
        {
            get;
            set;
        }

        private char OpenDelimiter
        {
            get
            {
                return AggregateLexer.OpenDelimiter;
            }
        }

        private char CloseDelimiter
        {
            get
            {
                return AggregateLexer.CloseDelimiter;
            }
        }
    }
}
