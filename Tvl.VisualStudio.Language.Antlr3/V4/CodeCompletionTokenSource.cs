namespace Tvl.VisualStudio.Language.AntlrV4
{
    using System;
    using System.Text.RegularExpressions;
    using Antlr4.Runtime;
    using Antlr4.Runtime.Misc;

    internal class CodeCompletionTokenSource : ITokenSource
    {
        private readonly ITokenSource _source;
        private readonly int _caretOffset;
        private readonly Tuple<ITokenSource, ICharStream> _tokenFactorySourcePair;

        private ITokenFactory _tokenFactory = CommonTokenFactory.Default;

        private IToken _caretToken;

        public CodeCompletionTokenSource(ITokenSource source, int caretOffset)
        {
            _source = source;
            _caretOffset = caretOffset;
            _tokenFactorySourcePair = Tuple.Create(source, source.InputStream);
        }

        public int Line
        {
            get
            {
                return _source.Line;
            }
        }

        public int Column
        {
            get
            {
                return _source.Column;
            }
        }

        public ICharStream InputStream
        {
            get
            {
                return _source.InputStream;
            }
        }

        public string SourceName
        {
            get
            {
                return _source.SourceName;
            }
        }

        public ITokenFactory TokenFactory
        {
            get
            {
                return _tokenFactory;
            }

            set
            {
                _source.TokenFactory = value;
                _tokenFactory = value ?? CommonTokenFactory.Default;
            }
        }

        [return: NotNull]
        public IToken NextToken()
        {
            if (_caretToken == null)
            {
                IToken token = _source.NextToken();
                if (token.StopIndex + 1 < _caretOffset)
                {
                    // the caret is after this token; nothing special to do
                }
                else if (token.StartIndex > _caretOffset)
                {
                    // the token is after the caret; no need to include it
                    token = new CaretToken(_tokenFactorySourcePair, TokenConstants.DefaultChannel, _caretOffset, _caretOffset);
                    _caretToken = token;
                }
                else
                {
                    if (token.StopIndex + 1 == _caretOffset && token.StopIndex >= token.StartIndex)
                    {
                        if (!IsWordToken(token))
                        {
                            // the caret is at the end of this token, and this isn't a word token or a zero-length token
                            return token;
                        }
                    }

                    // the caret is in the middle of or at the end of this token
                    token = new CaretToken(token);
                    _caretToken = token;
                }

                return token;
            }

            throw new InvalidOperationException("Attempted to look past the caret.");
        }

        // ^[$A-Za-z_][A-Za-z0-9_]*$
        protected static readonly Regex WordPattern = new Regex("^[$A-Za-z_][A-Za-z0-9_]*$", RegexOptions.Compiled);

        protected virtual bool IsWordToken(IToken token)
        {
            return WordPattern.IsMatch(token.Text);
        }
    }
}
