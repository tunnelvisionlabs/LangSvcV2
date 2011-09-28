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

        private AntlrClassifierLexer Lexer
        {
            get
            {
                return _lexer;
            }
        }

        public override IToken NextToken()
        {
            IToken token = base.NextToken();

            switch (token.Type)
            {
            case CONTINUE_DOUBLE_ANGLE_STRING_LITERAL:
                Lexer.InDoubleAngleStringLiteral = true;
                token.Type = DOUBLE_ANGLE_STRING_LITERAL;
                break;

            case END_DOUBLE_ANGLE_STRING_LITERAL:
                Lexer.InDoubleAngleStringLiteral = false;
                token.Type = DOUBLE_ANGLE_STRING_LITERAL;
                break;

            case CONTINUE_COMMENT:
                Lexer.InComment = true;
                token.Type = ML_COMMENT;
                break;

            case END_COMMENT:
                Lexer.InComment = false;
                token.Type = ML_COMMENT;
                break;

            default:
                break;
            }

            return token;
        }

        protected override void ParseNextToken()
        {
            if (Lexer.InDoubleAngleStringLiteral)
                mCONTINUE_DOUBLE_ANGLE_STRING_LITERAL();
            else if (Lexer.InComment)
                mCONTINUE_COMMENT();
            else
                base.ParseNextToken();
        }
    }
}
