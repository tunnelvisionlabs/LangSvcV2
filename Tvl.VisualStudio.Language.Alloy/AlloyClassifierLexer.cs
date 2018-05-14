namespace Tvl.VisualStudio.Language.Alloy
{
    using System.Diagnostics;
    using Antlr.Runtime;
    using JetBrains.Annotations;
    using Tvl.VisualStudio.Language.Parsing;

    internal class AlloyClassifierLexer : ITokenSourceWithState<AlloyClassifierLexerState>
    {
        private readonly ICharStream _input;
        private readonly AlloyColorizerLexer _languageLexer;

        private AlloyClassifierLexerMode _mode;
        private bool _inComment;

        public AlloyClassifierLexer([NotNull] ICharStream input)
            : this(input, AlloyClassifierLexerState.Initial)
        {
            Debug.Assert(input != null);
        }

        public AlloyClassifierLexer([NotNull] ICharStream input, AlloyClassifierLexerState state)
        {
            Requires.NotNull(input, nameof(input));

            _input = input;
            _languageLexer = new AlloyColorizerLexer(input, this);

            _mode = state.Mode;
            _inComment = state.InComment;
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
                return "Alloy Classifier";
            }
        }

        public string[] TokenNames
        {
            get
            {
                return _languageLexer.TokenNames;
            }
        }

        internal AlloyClassifierLexerMode Mode
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
                return _inComment;
            }

            set
            {
                _inComment = value;
            }
        }

        public AlloyClassifierLexerState GetCurrentState()
        {
            return new AlloyClassifierLexerState(_mode, _inComment);
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
            } while (token == null || token.Type == AlloyColorizerLexer.NEWLINE);

            return token;
        }

        private IToken NextTokenCore()
        {
            IToken token = null;

            switch (Mode)
            {
            case AlloyClassifierLexerMode.AlloyCode:
            default:
                token = _languageLexer.NextToken();
                break;
            }

            return token;
        }
    }
}
