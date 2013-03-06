namespace Tvl.VisualStudio.Language.Php.Classification
{
    using Antlr4.Runtime;
    using Tvl.VisualStudio.Language.Parsing4;
    using StringComparison = System.StringComparison;

    partial class V4PhpClassifierLexer : ITokenSourceWithState<V4PhpClassifierLexerState>
    {
        private const string DocCommentStartSymbols = "$@&~<>#%\"\\";

        private int _stringBraceLevel;
        private string _heredocIdentifier;

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

        public ICharStream CharStream
        {
            get
            {
                return (ICharStream)InputStream;
            }
        }

        public override IToken NextToken()
        {
            IToken token = base.NextToken();
            while (token.Type == NEWLINE)
                token = base.NextToken();

            switch (token.Type)
            {
            case PHP_NOWDOC_START:
                // <<<'identifier'
                _heredocIdentifier = token.Text.Substring(3).Trim('\'');
                break;

            case PHP_HEREDOC_START:
                // <<<identifier
                _heredocIdentifier = token.Text.Substring(3);
                break;

            case PHP_HEREDOC_END:
            case PHP_NOWDOC_END:
                _heredocIdentifier = null;
                break;

            case LBRACE:
                if (_mode == PhpDoubleString || _mode == PhpHereDoc)
                    StringBraceLevel++;

                break;

            case RBRACE:
                if (_mode == PhpDoubleString || _mode == PhpHereDoc)
                    StringBraceLevel--;

                break;

            default:
                break;
            }

            return token;
        }

        public override int PopMode()
        {
            if (_mode == PhpDoubleString || _mode == PhpHereDoc)
                StringBraceLevel = 0;

            return base.PopMode();
        }

        private bool CheckHeredocEnd(int la1, string text)
        {
            // identifier
            //  - or -
            // identifier;
            bool semi = text[text.Length - 1] == ';';
            string identifier = semi ? text.Substring(0, text.Length - 1) : text;
            return string.Equals(identifier, HeredocIdentifier, StringComparison.Ordinal);
        }

        private static bool IsDocCommentStartCharacter(int c)
        {
            if (char.IsLetter((char)c))
                return true;

            return DocCommentStartSymbols.IndexOf((char)c) >= 0;
        }

        public V4PhpClassifierLexerState GetCurrentState()
        {
            return new V4PhpClassifierLexerState(this);
        }
    }
}
