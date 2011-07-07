namespace Tvl.VisualStudio.Language.Go
{
    using System;

    internal struct GoClassifierLexerState : IEquatable<GoClassifierLexerState>
    {
        public static readonly GoClassifierLexerState Initial =
            new GoClassifierLexerState(GoClassifierLexerMode.GoCode);

        public readonly GoClassifierLexerMode Mode;

        public GoClassifierLexerState(GoClassifierLexerMode mode)
        {
            Mode = mode;
        }

        public bool Equals(GoClassifierLexerState other)
        {
            return this.Mode == other.Mode;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is GoClassifierLexerState))
                return false;

            return Equals((GoClassifierLexerState)obj);
        }

        public override int GetHashCode()
        {
            return Mode.GetHashCode();
        }
    }
}
