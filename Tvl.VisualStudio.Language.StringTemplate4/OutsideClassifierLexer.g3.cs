namespace Tvl.VisualStudio.Language.StringTemplate4
{
    using System;
    using System.Diagnostics.Contracts;
    using Antlr.Runtime;

    partial class OutsideClassifierLexer
    {
        internal OutsideClassifierLexer(ICharStream input, ClassifierLexer lexer)
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

        private ClassifierLexer.TemplateLexerMode Mode
        {
            get
            {
                return AggregateLexer.Mode;
            }
        }

        private ClassifierLexer.OutermostTemplate Outermost
        {
            get
            {
                return AggregateLexer.Outermost;
            }
        }

        private int AnonymousTemplateLevel
        {
            get
            {
                return AggregateLexer.AnonymousTemplateLevel;
            }
        }

        private bool InComment
        {
            get
            {
                return AggregateLexer.InComment;
            }

            set
            {
                AggregateLexer.InComment = value;
            }
        }

        public override IToken NextToken()
        {
            IToken token = base.NextToken();

            switch (token.Type)
            {
            case CONTINUE_COMMENT:
                InComment = true;
                token.Type = COMMENT;
                break;

            case END_COMMENT:
                InComment = false;
                token.Type = COMMENT;
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
