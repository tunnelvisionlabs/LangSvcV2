namespace Tvl.VisualStudio.Language.Antlr3
{
    using System;
    using System.Diagnostics.Contracts;
    using Antlr.Runtime;

    partial class AntlrActionClassifierLexer
    {
        private readonly AntlrClassifierLexer _lexer;

        internal AntlrActionClassifierLexer(ICharStream input, AntlrClassifierLexer lexer)
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

        private static bool IsIdStartChar(int c)
        {
            return (c >= 'a' && c <= 'z')
                || (c >= 'A' && c <= 'Z')
                || c == '_';
        }

        public override IToken NextToken()
        {
            IToken token = base.NextToken();

            switch (token.Type)
            {
            case CONTINUE_CHAR_LITERAL:
                Lexer.InCharLiteral = true;
                token.Type = ACTION_CHAR_LITERAL;
                break;

            case END_CHAR_LITERAL:
                Lexer.InCharLiteral = false;
                token.Type = ACTION_CHAR_LITERAL;
                break;

            case CONTINUE_STRING_LITERAL:
                Lexer.InStringLiteral = true;
                token.Type = ACTION_STRING_LITERAL;
                break;

            case END_STRING_LITERAL:
                Lexer.InStringLiteral = false;
                token.Type = ACTION_STRING_LITERAL;
                break;

            case CONTINUE_COMMENT:
                Lexer.InComment = true;
                token.Type = ACTION_ML_COMMENT;
                break;

            case END_COMMENT:
                Lexer.InComment = false;
                token.Type = ACTION_ML_COMMENT;
                break;

            default:
                break;
            }

            return token;
        }

        protected override void ParseNextToken()
        {
            if (Lexer.InCharLiteral)
                mCONTINUE_CHAR_LITERAL();
            else if (Lexer.InStringLiteral)
                mCONTINUE_STRING_LITERAL();
            else if (Lexer.InComment)
                mCONTINUE_COMMENT();
            else
                base.ParseNextToken();
        }
    }
}
