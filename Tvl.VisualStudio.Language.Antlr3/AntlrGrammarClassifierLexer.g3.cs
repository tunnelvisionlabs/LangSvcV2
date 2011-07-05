namespace Tvl.VisualStudio.Language.Antlr3
{
    using System;
    using System.Diagnostics.Contracts;
    using Antlr.Runtime;

    partial class AntlrGrammarClassifierLexer
    {
        private readonly AntlrClassifierLexer _lexer;

        internal AntlrGrammarClassifierLexer(ICharStream input, AntlrClassifierLexer lexer)
            : this(input)
        {
            Contract.Requires<ArgumentNullException>(input != null, "input");
            Contract.Requires<ArgumentNullException>(lexer != null, "lexer");

            _lexer = lexer;
        }

        private int ActionLevel
        {
            get
            {
                return _lexer.ActionLevel;
            }

            set
            {
                _lexer.ActionLevel = value;
            }
        }

        private bool InComment
        {
            get
            {
                return _lexer.InComment;
            }

            set
            {
                _lexer.InComment = value;
            }
        }

        public override IToken NextToken()
        {
            IToken token = base.NextToken();

            switch (token.Type)
            {
            case CONTINUE_COMMENT:
                InComment = true;
                token.Type = ML_COMMENT;
                break;

            case END_COMMENT:
                InComment = false;
                token.Type = ML_COMMENT;
                break;

            default:
                break;
            }

            return token;
        }

        protected override void ParseNextToken()
        {
            if (InComment)
                mCONTINUE_COMMENT();
            else
                base.ParseNextToken();
        }
    }
}
