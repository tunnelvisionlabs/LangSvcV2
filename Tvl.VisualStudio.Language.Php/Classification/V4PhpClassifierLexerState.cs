namespace Tvl.VisualStudio.Language.Php.Classification
{
    using System;
    using SimpleLexerState = Tvl.VisualStudio.Language.Parsing4.SimpleLexerState;

    public struct V4PhpClassifierLexerState : IEquatable<V4PhpClassifierLexerState>
    {
        private static readonly V4PhpClassifierLexerState _initial = new V4PhpClassifierLexerState(new SimpleLexerState(V4PhpClassifierLexer.HtmlText));

        private readonly SimpleLexerState _simpleLexerState;
        private readonly int _stringBraceLevel;
        private readonly string _heredocIdentifier;
        private readonly bool _foundEntity;
        private readonly bool _foundOperator;

        public V4PhpClassifierLexerState(V4PhpClassifierLexer lexer)
        {
            _simpleLexerState = new SimpleLexerState(lexer);
            _stringBraceLevel = lexer.StringBraceLevel;
            _heredocIdentifier = lexer.HeredocIdentifier;
            _foundEntity = lexer.FoundEntity;
            _foundOperator = lexer.FoundOperator;
        }

        private V4PhpClassifierLexerState(SimpleLexerState state)
        {
            _simpleLexerState = state;
            _stringBraceLevel = 0;
            _heredocIdentifier = null;
            _foundEntity = false;
            _foundOperator = false;
        }

        public static V4PhpClassifierLexerState Initial
        {
            get
            {
                return _initial;
            }
        }

        public void Apply(V4PhpClassifierLexer lexer)
        {
            _simpleLexerState.Apply(lexer);
            lexer.StringBraceLevel = _stringBraceLevel;
            lexer.HeredocIdentifier = _heredocIdentifier;
            lexer.FoundEntity = _foundEntity;
            lexer.FoundOperator = _foundOperator;
        }

        public bool Equals(V4PhpClassifierLexerState other)
        {
            return _simpleLexerState.Equals(other._simpleLexerState)
                && _stringBraceLevel == other._stringBraceLevel
                && _heredocIdentifier == other._heredocIdentifier
                && _foundEntity == other._foundEntity
                && _foundOperator == other._foundOperator;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is V4PhpClassifierLexerState))
                return false;

            return Equals((V4PhpClassifierLexerState)obj);
        }

        public override int GetHashCode()
        {
            int hashCode = 1;
            hashCode = hashCode * 31 + _simpleLexerState.GetHashCode();
            hashCode = hashCode * 31 + _stringBraceLevel;
            hashCode = hashCode * 31 + (_heredocIdentifier != null ? _heredocIdentifier.GetHashCode() : 0);
            hashCode = hashCode * 31 + (_foundEntity ? 1 : 0);
            hashCode = hashCode * 31 + (_foundOperator ? 1 : 0);
            return hashCode;
        }
    }
}
