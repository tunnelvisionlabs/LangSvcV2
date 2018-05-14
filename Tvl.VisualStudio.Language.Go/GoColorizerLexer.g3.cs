namespace Tvl.VisualStudio.Language.Go
{
    using Antlr.Runtime;
    using JetBrains.Annotations;

    partial class GoColorizerLexer
    {
        private readonly GoClassifierLexer _lexer;

        public GoColorizerLexer(ICharStream input, [NotNull] GoClassifierLexer lexer)
            : this(input)
        {
            Requires.NotNull(lexer, nameof(lexer));

            _lexer = lexer;
        }

        private GoClassifierLexerMode Mode
        {
            get
            {
                return _lexer.Mode;
            }

            set
            {
                _lexer.Mode = value;
            }
        }

        public override IToken NextToken()
        {
            IToken token = base.NextToken();

            switch (token.Type)
            {
            case CONTINUE_COMMENT:
                Mode = GoClassifierLexerMode.GoCodeComment;
                token.Type = ML_COMMENT;
                break;

            case END_COMMENT:
                Mode = GoClassifierLexerMode.GoCode;
                token.Type = ML_COMMENT;
                break;

            case CONTINUE_STRING:
                Mode = GoClassifierLexerMode.GoCodeString;
                token.Type = RAW_STRING_LITERAL;
                break;

            case END_STRING:
                Mode = GoClassifierLexerMode.GoCode;
                token.Type = RAW_STRING_LITERAL;
                break;

            default:
                break;
            }

            return token;
        }

        protected override void ParseNextToken()
        {
            if (Mode == GoClassifierLexerMode.GoCodeComment)
                mCONTINUE_COMMENT();
            else if (Mode == GoClassifierLexerMode.GoCodeString)
                mCONTINUE_STRING();
            else
                base.ParseNextToken();
        }

        public override void Recover(IIntStream input, RecognitionException re)
        {
            base.Recover(input, re);
        }

        public override void DisplayRecognitionError(string[] tokenNames, RecognitionException e)
        {
            base.DisplayRecognitionError(tokenNames, e);
        }
    }
}
