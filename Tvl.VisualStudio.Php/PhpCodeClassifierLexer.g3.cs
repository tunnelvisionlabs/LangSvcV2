namespace Tvl.VisualStudio.Language.Php
{
    using System;
    using System.Diagnostics.Contracts;
    using Antlr.Runtime;

    partial class PhpCodeClassifierLexer
    {
        private readonly PhpClassifierLexer _lexer;

        public PhpCodeClassifierLexer(ICharStream input, PhpClassifierLexer lexer)
            : this(input)
        {
            Contract.Requires<ArgumentNullException>(lexer != null, "lexer");

            _lexer = lexer;
        }

        private bool InString
        {
            get
            {
                return _lexer.InString;
            }

            set
            {
                _lexer.InString = value;
            }
        }

        private string HeredocIdentifier
        {
            get
            {
                return _lexer.HeredocIdentifier;
            }

            set
            {
                _lexer.HeredocIdentifier = value;
            }
        }

        private int StringBraceLevel
        {
            get
            {
                return _lexer.StringBraceLevel;
            }

            set
            {
                _lexer.StringBraceLevel = value;
            }
        }

        private bool InStringExpression
        {
            get
            {
                return _lexer.InStringExpression;
            }

            set
            {
                _lexer.InStringExpression = value;
            }
        }

        private bool CheckHeredocEnd
        {
            get;
            set;
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

                if (InString && !string.IsNullOrEmpty(HeredocIdentifier))
                {
                    if (HeredocIdentifier == "'")
                        mCONTINUE_SINGLE_STRING();
                    else if (HeredocIdentifier == "\"")
                        mCONTINUE_DOUBLE_STRING();
                    else
                        mCONTINUE_HEREDOC();

                    if (state.token == null)
                        Emit();

                    token = state.token;
                }
                else
                {
                    token = base.NextToken();
                }
            }

            if (token.Type == CONTINUE_HEREDOC && CheckHeredocEnd)
            {
                string text = token.Text;
                if (text == HeredocIdentifier || text == HeredocIdentifier + ";")
                    token.Type = PHP_HEREDOC_END;
            }

            int textTokenType = HeredocIdentifier == "\"" ? PHP_DOUBLE_STRING_LITERAL : PHP_HEREDOC_TEXT;

            switch (token.Type)
            {
            case PHP_IDENTIFIER:
                if (InString)
                {
                    if (token.Text[0] == '$' && token.Text.Length > 1)
                        InStringExpression = true;
                    else if (!InStringExpression)
                        token.Type = textTokenType;
                }

                break;

            case PHP_ARROW:
            case PHP_LBRACK:
            case PHP_RBRACK:
                if (InString && !InStringExpression)
                    token.Type = textTokenType;

                break;

            case PHP_LBRACE:
                if (InString)
                    StringBraceLevel++;

                break;

            case PHP_RBRACE:
                if (InString)
                    StringBraceLevel--;

                break;

            case DOUBLE_STRING_ESCAPE:
                //token.Type = PHP_DOUBLE_STRING_LITERAL;
                break;

            case WS:
                if (InString && !InStringExpression)
                    token.Type = textTokenType;
                break;

            case CONTINUE_SINGLE_STRING:
                InString = true;
                InStringExpression = false;
                HeredocIdentifier = "'";
                token.Type = PHP_SINGLE_STRING_LITERAL;
                break;

            case CONTINUE_DOUBLE_STRING:
                InString = true;
                InStringExpression = false;
                HeredocIdentifier = "\"";
                token.Type = PHP_DOUBLE_STRING_LITERAL;
                break;

            case PHP_HEREDOC_START:
                InString = true;
                InStringExpression = false;
                HeredocIdentifier = token.Text.Substring(3);
                token.Type = PHP_HEREDOC_TEXT;
                break;

            case CONTINUE_HEREDOC:
                token.Type = PHP_HEREDOC_TEXT;
                break;

            case END_SINGLE_STRING:
                InString = false;
                HeredocIdentifier = null;
                token.Type = PHP_SINGLE_STRING_LITERAL;
                break;

            case END_DOUBLE_STRING:
                InString = false;
                HeredocIdentifier = null;
                token.Type = PHP_DOUBLE_STRING_LITERAL;
                break;

            case PHP_HEREDOC_END:
                InString = false;
                HeredocIdentifier = null;
                token.Type = PHP_HEREDOC_TEXT;
                break;

            default:
                break;
            }

            return token;
        }

        private static bool IsDoubleQuoteEscapeChar(int c)
        {
            switch (c)
            {
            case 'n':
            case 'r':
            case 't':
            case 'v':
            case 'f':
            case '\\':
            case '$':
            case '"':
            case '0':
            case '1':
            case '2':
            case '3':
            case '4':
            case '5':
            case '6':
            case '7':
                return true;

            case 'x':
                return true;

            default:
                return false;
            }
        }
    }
}
