namespace Tvl.VisualStudio.Language.Go
{
    using Antlr.Runtime;

    internal sealed class GoSemicolonInsertionTokenSource : ITokenSource
    {
        private ITokenSource _lexer;
        private bool _nextTokenIsSemicolon;
        private IToken _nextRealToken;

        public GoSemicolonInsertionTokenSource(GoLexer lexer)
        {
            _lexer = lexer;
        }

        public IToken NextToken()
        {
            IToken token = NextTokenImpl();
            while (token != null && token.Channel == TokenChannels.Hidden)
                token = NextTokenImpl();

            return token;
        }

        public string SourceName
        {
            get
            {
                return _lexer.SourceName;
            }
        }

        public string[] TokenNames
        {
            get
            {
                return _lexer.TokenNames;
            }
        }

        private IToken NextTokenImpl()
        {
            if (_nextTokenIsSemicolon)
            {
                _nextTokenIsSemicolon = false;
                return
                    new CommonToken(_nextRealToken.InputStream, GoLexer.SEMI, TokenChannels.Default, _nextRealToken.StartIndex, _nextRealToken.StopIndex)
                    {
                        Text = ";*"
                    };
            }

            // handle the first token in the stream
            if (_nextRealToken == null)
                _nextRealToken = _lexer.NextToken();

            IToken token = _nextRealToken;
            _nextRealToken = _lexer.NextToken();

            if (_nextRealToken.Type == GoLexer.NEW_LINE)
            {
                switch (token.Type)
                {
                case GoLexer.IDENTIFIER:
                case GoLexer.NUMBER:
                case GoLexer.CHAR_LITERAL:
                case GoLexer.STRING_LITERAL:
                case GoLexer.KW_BREAK:
                case GoLexer.KW_CONTINUE:
                case GoLexer.KW_FALLTHROUGH:
                case GoLexer.KW_RETURN:
                case GoLexer.INC:
                case GoLexer.DEC:
                case GoLexer.RPAREN:
                case GoLexer.RBRACK:
                case GoLexer.RBRACE:
                    _nextTokenIsSemicolon = true;
                    break;

                default:
                    break;
                }
            }

            return token;
        }
    }
}
