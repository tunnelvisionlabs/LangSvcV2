namespace JavaLanguageService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Antlr.Runtime;
    using System.Diagnostics.Contracts;

    partial class JavaColorizerLexer
    {
        private readonly JavaClassifierLexer _lexer;

        public JavaColorizerLexer(ICharStream input, JavaClassifierLexer lexer)
            : this(input)
        {
            Contract.Requires<ArgumentNullException>(lexer != null, "lexer");

            _lexer = lexer;
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
            IToken token = null;
            while (token == null)
            {
                state.token = null;
                state.channel = TokenChannels.Default;
                state.tokenStartCharIndex = input.Index;
                state.tokenStartCharPositionInLine = input.CharPositionInLine;
                state.tokenStartLine = input.Line;
                state.text = null;

                if (InComment)
                {
                    mCONTINUE_COMMENT();
                    if (state.token == null)
                        Emit();

                    token = state.token;
                }
                else
                {
                    token = base.NextToken();
                }
            }

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
