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
    }
}
