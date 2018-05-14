namespace Tvl.VisualStudio.Language.Go
{
    using System.Diagnostics;
    using Antlr.Runtime;
    using JetBrains.Annotations;
    using Tvl.VisualStudio.Language.Parsing;

    internal class GoClassifierLexer : ITokenSourceWithState<GoClassifierLexerState>
    {
        private readonly ICharStream _input;
        private readonly GoColorizerLexer _languageLexer;

        private GoClassifierLexerMode _mode;

        public GoClassifierLexer([NotNull] ICharStream input)
            : this(input, GoClassifierLexerState.Initial)
        {
            Debug.Assert(input != null);
        }

        public GoClassifierLexer([NotNull] ICharStream input, GoClassifierLexerState state)
        {
            Requires.NotNull(input, nameof(input));

            _input = input;
            _languageLexer = new GoColorizerLexer(input, this);

            _mode = state.Mode;
        }

        public ICharStream CharStream
        {
            get
            {
                return _input;
            }
        }

        public string SourceName
        {
            get
            {
                return "Go Classifier";
            }
        }

        public string[] TokenNames
        {
            get
            {
                return _languageLexer.TokenNames;
            }
        }

        internal GoClassifierLexerMode Mode
        {
            get
            {
                return _mode;
            }

            set
            {
                _mode = value;
            }
        }

        internal bool InComment
        {
            get
            {
                return Mode == GoClassifierLexerMode.GoCodeComment;
            }
        }

        internal bool InString
        {
            get
            {
                return Mode == GoClassifierLexerMode.GoCodeString;
            }
        }

        public GoClassifierLexerState GetCurrentState()
        {
            return new GoClassifierLexerState(_mode);
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
            } while (token == null || token.Type == GoColorizerLexer.NEWLINE);

            return token;
        }

        private IToken NextTokenCore()
        {
            IToken token = null;

            switch (Mode)
            {
            case GoClassifierLexerMode.GoCode:
            case GoClassifierLexerMode.GoCodeComment:
            case GoClassifierLexerMode.GoCodeString:
            default:
                token = _languageLexer.NextToken();
                break;
            }

            return token;
        }
    }
}
