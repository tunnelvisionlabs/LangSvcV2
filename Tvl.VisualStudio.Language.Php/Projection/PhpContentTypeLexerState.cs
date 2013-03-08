namespace Tvl.VisualStudio.Language.Php.Projection
{
    using System;
    using SimpleLexerState = Tvl.VisualStudio.Language.Parsing4.SimpleLexerState;

    public struct PhpContentTypeLexerState : IEquatable<PhpContentTypeLexerState>
    {
        private static readonly PhpContentTypeLexerState _initial = new PhpContentTypeLexerState(SimpleLexerState.Initial);

        private readonly SimpleLexerState _simpleLexerState;
        private readonly string _heredocIdentifier;

        public PhpContentTypeLexerState(PhpContentTypeLexer lexer)
        {
            _simpleLexerState = new SimpleLexerState(lexer);
            _heredocIdentifier = lexer.HeredocIdentifier;
        }

        private PhpContentTypeLexerState(SimpleLexerState state)
        {
            _simpleLexerState = state;
            _heredocIdentifier = null;
        }

        public static PhpContentTypeLexerState Initial
        {
            get
            {
                return _initial;
            }
        }

        public void Apply(PhpContentTypeLexer lexer)
        {
            _simpleLexerState.Apply(lexer);
            lexer.HeredocIdentifier = _heredocIdentifier;
        }

        public bool Equals(PhpContentTypeLexerState other)
        {
            return _simpleLexerState.Equals(other._simpleLexerState)
                && _heredocIdentifier == other._heredocIdentifier;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is PhpContentTypeLexerState))
                return false;

            return Equals((PhpContentTypeLexerState)obj);
        }

        public override int GetHashCode()
        {
            int hashCode = 1;
            hashCode = hashCode * 31 + _simpleLexerState.GetHashCode();
            hashCode = hashCode * 31 + (_heredocIdentifier != null ? _heredocIdentifier.GetHashCode() : 0);
            return hashCode;
        }
    }
}
