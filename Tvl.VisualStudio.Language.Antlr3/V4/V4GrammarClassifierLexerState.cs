namespace Tvl.VisualStudio.Language.AntlrV4
{
    using System;
    using SimpleLexerState = Tvl.VisualStudio.Language.Parsing4.SimpleLexerState;
    using TokenConstants = Antlr4.Runtime.TokenConstants;

    public struct V4GrammarClassifierLexerState : IEquatable<V4GrammarClassifierLexerState>
    {
        private static readonly V4GrammarClassifierLexerState _initial = new V4GrammarClassifierLexerState(SimpleLexerState.Initial);

        private readonly SimpleLexerState _simpleLexerState;
        private readonly bool _inOptions;
        private readonly bool _inTokens;
        private readonly int _ruleType;

        public V4GrammarClassifierLexerState(GrammarHighlighterLexer lexer)
        {
            _simpleLexerState = new SimpleLexerState(lexer);
            _inOptions = lexer.IsInOptions;
            _inTokens = lexer.IsInTokens;
            _ruleType = lexer.RuleType;
        }

        private V4GrammarClassifierLexerState(SimpleLexerState state)
        {
            _simpleLexerState = state;
            _inOptions = false;
            _inTokens = false;
            _ruleType = TokenConstants.InvalidType;
        }

        public static V4GrammarClassifierLexerState Initial
        {
            get
            {
                return _initial;
            }
        }

        public void Apply(GrammarHighlighterLexer lexer)
        {
            _simpleLexerState.Apply(lexer);
            lexer.IsInOptions = _inOptions;
            lexer.IsInTokens = _inTokens;
            lexer.RuleType = _ruleType;
        }

        public bool Equals(V4GrammarClassifierLexerState other)
        {
            return _simpleLexerState.Equals(other._simpleLexerState)
                && _inOptions == other._inOptions
                && _inTokens == other._inTokens
                && _ruleType == other._ruleType;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is V4GrammarClassifierLexerState))
                return false;

            return Equals((V4GrammarClassifierLexerState)obj);
        }

        public override int GetHashCode()
        {
            int hashCode = 1;
            hashCode = hashCode * 31 + _simpleLexerState.GetHashCode();
            hashCode = hashCode * 31 + (_inOptions ? 1 : 0);
            hashCode = hashCode * 31 + (_inTokens ? 1 : 0);
            hashCode = hashCode * 31 + _ruleType;
            return hashCode;
        }
    }
}
