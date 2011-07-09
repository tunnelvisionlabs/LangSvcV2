namespace Tvl.VisualStudio.Language.Php.Classification
{
    using System;
    using System.Diagnostics.Contracts;
    using Antlr.Runtime;

    partial class PhpHtmlTagClassifierLexer
    {
        private readonly PhpClassifierLexer _lexer;

        public PhpHtmlTagClassifierLexer(ICharStream input, PhpClassifierLexer lexer)
            : this(input)
        {
            Contract.Requires<ArgumentNullException>(lexer != null, "lexer");

            _lexer = lexer;
        }

        private HtmlTagState TagState
        {
            get
            {
                return (HtmlTagState)_lexer.HtmlTagState;
            }

            set
            {
                _lexer.HtmlTagState = (int)value;
            }
        }

        private bool FoundEntity
        {
            get
            {
                return (TagState & HtmlTagState.FoundEntity) != 0;
            }

            set
            {
                if (value)
                    TagState |= HtmlTagState.FoundEntity;
                else
                    TagState &= ~HtmlTagState.FoundEntity;
            }
        }

        private bool FollowingOperator
        {
            get
            {
                return (TagState & HtmlTagState.FollowingOperator) != 0;
            }

            set
            {
                if (value)
                    TagState |= HtmlTagState.FollowingOperator;
                else
                    TagState &= ~HtmlTagState.FollowingOperator;
            }
        }

        private bool InSingleQuoteString
        {
            get
            {
                return (TagState & HtmlTagState.InSingleQuoteString) != 0;
            }

            set
            {
                if (value)
                    TagState |= HtmlTagState.InSingleQuoteString;
                else
                    TagState &= ~HtmlTagState.InSingleQuoteString;
            }
        }

        private bool InDoubleQuoteString
        {
            get
            {
                return (TagState & HtmlTagState.InDoubleQuoteString) != 0;
            }

            set
            {
                if (value)
                    TagState |= HtmlTagState.InDoubleQuoteString;
                else
                    TagState &= ~HtmlTagState.InDoubleQuoteString;
            }
        }

        private bool InString
        {
            get
            {
                return InSingleQuoteString || InDoubleQuoteString;
            }
        }

        private static bool IsPhpTagStart(ICharStream input)
        {
            return input.LA(1) == '<'
                && input.LA(2) == '?'
                && input.LA(3) == 'p'
                && input.LA(4) == 'h'
                && input.LA(5) == 'p';
        }

        public override IToken NextToken()
        {
            IToken token = base.NextToken();

            switch (token.Type)
            {
            case HTML_OPERATOR:
                FollowingOperator = true;
                break;

            case NAME:
                if (FollowingOperator)
                {
                    token.Type = HTML_ATTRIBUTE_VALUE;
                    break;
                }
                else if (FoundEntity)
                {
                    token.Type = HTML_ATTRIBUTE_NAME;
                }
                else
                {
                    FoundEntity = true;
                    token.Type = HTML_ELEMENT_NAME;
                }

                break;

            case BEGIN_SINGLE_QUOTE_STRING:
                InSingleQuoteString = true;
                goto case CONTINUE_STRING;

            case BEGIN_DOUBLE_QUOTE_STRING:
                InDoubleQuoteString = true;
                goto case CONTINUE_STRING;

            case END_STRING:
                InSingleQuoteString = false;
                InDoubleQuoteString = false;
                goto case CONTINUE_STRING;

            case CONTINUE_STRING:
                token.Type = HTML_ATTRIBUTE_VALUE;
                break;

            case HTML_CLOSE_TAG:
                TagState = HtmlTagState.None;
                break;

            default:
                break;
            }

            if (token.Type != WS && token.Type != HTML_OPERATOR)
                FollowingOperator = false;

            return token;
        }

        protected override void ParseNextToken()
        {
            if (InSingleQuoteString)
                mCONTINUE_SINGLE_QUOTE_STRING();
            else if (InDoubleQuoteString)
                mCONTINUE_DOUBLE_QUOTE_STRING();
            else
                base.ParseNextToken();
        }

        [Flags]
        private enum HtmlTagState
        {
            None,
            FoundEntity = 0x0001,
            FollowingOperator = 0x0002,

            InSingleQuoteString = 0x0004,
            InDoubleQuoteString = 0x0008,
        }
    }
}
