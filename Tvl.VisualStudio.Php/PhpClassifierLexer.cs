namespace Tvl.VisualStudio.Language.Php
{
    using Tvl.VisualStudio.Language.Parsing;
    using Antlr.Runtime;
    using System;
    using System.Diagnostics.Contracts;

    internal class PhpClassifierLexer : ITokenSourceWithState<PhpClassifierLexerState>
    {
        private readonly ICharStream _input;
        private readonly PhpHtmlTextClassifierLexer _htmlTextLexer;
        private readonly PhpHtmlTagClassifierLexer _htmlTagLexer;
        private readonly PhpCodeClassifierLexer _codeLexer;
        private readonly PhpDocCommentClassifierLexer _commentLexer;

        private PhpClassifierLexerMode _mode;
        private bool _inString;
        private string _heredocIdentifier;
        private int _stringBraceLevel;
        private bool _inStringExpression;
        private int _htmlTagState;

        public PhpClassifierLexer(ICharStream input)
            : this(input, PhpClassifierLexerState.Initial)
        {
            Contract.Requires(input != null);
        }

        public PhpClassifierLexer(ICharStream input, PhpClassifierLexerState state)
        {
            Contract.Requires<ArgumentNullException>(input != null, "input");

            _input = input;
            _htmlTextLexer = new PhpHtmlTextClassifierLexer(input, this);
            _htmlTagLexer = new PhpHtmlTagClassifierLexer(input, this);
            _codeLexer = new PhpCodeClassifierLexer(input, this);
            _commentLexer = new PhpDocCommentClassifierLexer(input, this);

            _mode = state.Mode;
            _inString = state.InString;
            _heredocIdentifier = state.HeredocIdentifier;
            _stringBraceLevel = state.StringBraceLevel;
            _inStringExpression = state.InStringExpression;
            _htmlTagState = state.HtmlTagState;
        }

        public PhpClassifierLexerMode Mode
        {
            get
            {
                return _mode;
            }

            private set
            {
                _mode = value;
            }
        }

        public bool InString
        {
            get
            {
                return _inString;
            }

            set
            {
                _inString = value;
            }
        }

        public string HeredocIdentifier
        {
            get
            {
                return _heredocIdentifier;
            }

            set
            {
                _heredocIdentifier = value;
            }
        }

        public int StringBraceLevel
        {
            get
            {
                return _stringBraceLevel;
            }

            set
            {
                _stringBraceLevel = value;
            }
        }

        public bool InStringExpression
        {
            get
            {
                return _inStringExpression;
            }

            set
            {
                _inStringExpression = value;
            }
        }

        public int HtmlTagState
        {
            get
            {
                return _htmlTagState;
            }

            set
            {
                _htmlTagState = value;
            }
        }

        public string SourceName
        {
            get
            {
                return _input.SourceName;
            }
        }

        public string[] TokenNames
        {
            get
            {
                return _commentLexer.TokenNames;
            }
        }

        public PhpClassifierLexerState GetCurrentState()
        {
            return new PhpClassifierLexerState(_mode, _inString, _heredocIdentifier, _stringBraceLevel, _inStringExpression, _htmlTagState);
        }

        public IToken NextToken()
        {
            IToken token = null;
            do
            {
                int position = _input.Index;
                token = NextTokenCore();
                // ensure progress
                if (position == _input.Index)
                    _input.Consume();
            } while (token == null || token.Type == PhpDocCommentClassifierLexer.NEWLINE);

            return token;
        }

        private IToken NextTokenCore()
        {
            IToken token = null;

            switch (_mode)
            {
            case PhpClassifierLexerMode.PhpDocComment:
                token = _commentLexer.NextToken();

                switch (token.Type)
                {
                case PhpDocCommentClassifierLexer.END_COMMENT:
                    _mode = PhpClassifierLexerMode.PhpCode;
                    token.Type = PhpDocCommentClassifierLexer.DOC_COMMENT_TEXT;
                    break;

                default:
                    break;
                }

                break;

            case PhpClassifierLexerMode.PhpCode:
            case PhpClassifierLexerMode.PhpCodeWithinTag:
                token = _codeLexer.NextToken();

                switch (token.Type)
                {
                case PhpDocCommentClassifierLexer.DOC_COMMENT_START:
                    _mode = PhpClassifierLexerMode.PhpDocComment;
                    token.Type = PhpDocCommentClassifierLexer.DOC_COMMENT_TEXT;
                    break;

                case PhpDocCommentClassifierLexer.CLOSE_PHP_TAG:
                    _mode = (_mode == PhpClassifierLexerMode.PhpCode) ? PhpClassifierLexerMode.HtmlText : PhpClassifierLexerMode.HtmlTag;
                    break;

                default:
                    break;
                }

                break;

            case PhpClassifierLexerMode.HtmlTag:
                token = _htmlTagLexer.NextToken();

                switch (token.Type)
                {
                case PhpHtmlTagClassifierLexer.HTML_CLOSE_TAG:
                    _mode = PhpClassifierLexerMode.HtmlText;
                    break;

                case PhpHtmlTagClassifierLexer.HTML_START_CODE:
                    _mode = PhpClassifierLexerMode.PhpCodeWithinTag;
                    break;

                default:
                    break;
                }

                break;

            case PhpClassifierLexerMode.HtmlText:
            default:
                token = _htmlTextLexer.NextToken();

                switch (token.Type)
                {
                case PhpHtmlTextClassifierLexer.HTML_START_CODE:
                    _mode = PhpClassifierLexerMode.PhpCode;
                    break;

                case PhpHtmlTextClassifierLexer.HTML_START_TAG:
                    _mode = PhpClassifierLexerMode.HtmlTag;
                    break;

                default:
                    break;
                }

                break;
            }

            return token;
        }
    }
}
