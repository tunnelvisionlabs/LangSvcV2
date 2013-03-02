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
            case PHP_HEREDOC_START:
                // <<<identifier
                _heredocIdentifier = token.Text.Substring(3);
                break;

            case PHP_HEREDOC_END:
                _heredocIdentifier = null;
                break;

            default:
                break;
            }

            return token;
        }

        private static bool IsTagStart(IIntStream input)
        {
            if (input.La(1) != '<')
                return false;

            int la2 = input.La(2);
            if (la2 < 0)
                return false;

            switch (la2)
            {
            case '?':
                return true;

            case '!':
                {
                    int la3 = input.La(3);
                    if (char.IsLetterOrDigit((char)la3))
                        return true;

                    if (la3 == '-' && input.La(4) == '-')
                        return true;

                    if (la3 == '[' && input.La(4) == 'C' && input.La(5) == 'D' && input.La(6) == 'A' && input.La(7) == 'T' && input.La(8) == 'A' && input.La(9) == '[')
                        return true;
                }

                return false;

            case '/':
                return char.IsLetterOrDigit((char)input.La(3));

            default:
                if (char.IsLetterOrDigit((char)la2))
                    return true;

                break;
            }

            return false;
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
