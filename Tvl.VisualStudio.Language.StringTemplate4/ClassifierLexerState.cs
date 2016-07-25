namespace Tvl.VisualStudio.Language.StringTemplate4
{
    using Tvl.VisualStudio.Language.Parsing4;

    public struct ClassifierLexerState
    {
        private static readonly ClassifierLexerState _initial = new ClassifierLexerState(SimpleLexerState.Initial);

        private readonly SimpleLexerState _simpleLexerState;
        private readonly char _openDelimiter;
        private readonly char _closeDelimiter;

        public ClassifierLexerState(ClassifierLexer lexer)
        {
            _simpleLexerState = new SimpleLexerState(lexer);
            _openDelimiter = lexer.OpenDelimiter;
            _closeDelimiter = lexer.CloseDelimiter;
        }

        private ClassifierLexerState(SimpleLexerState state)
        {
            _simpleLexerState = state;
            _openDelimiter = ClassifierLexer.DefaultOpenDelimiter;
            _closeDelimiter = ClassifierLexer.DefaultCloseDelimiter;
        }

        public static ClassifierLexerState Initial
        {
            get
            {
                return _initial;
            }
        }

        public void Apply(ClassifierLexer lexer)
        {
            _simpleLexerState.Apply(lexer);
            lexer.SetDelimiters(_openDelimiter, _closeDelimiter);
        }

        public bool Equals(ClassifierLexerState other)
        {
            return _simpleLexerState.Equals(other._simpleLexerState)
                && _openDelimiter == other._openDelimiter
                && _closeDelimiter == other._closeDelimiter;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ClassifierLexerState))
                return false;

            return Equals((ClassifierLexerState)obj);
        }

        public override int GetHashCode()
        {
            int hashCode = 1;
            hashCode = hashCode * 31 + _simpleLexerState.GetHashCode();
            hashCode = hashCode * 31 + _openDelimiter.GetHashCode();
            hashCode = hashCode * 31 + _closeDelimiter.GetHashCode();
            return hashCode;
        }
    }
}
