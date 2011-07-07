namespace Tvl.VisualStudio.Language.Alloy
{
    using System;

    internal struct AlloyClassifierLexerState : IEquatable<AlloyClassifierLexerState>
    {
        public static readonly AlloyClassifierLexerState Initial =
            new AlloyClassifierLexerState(AlloyClassifierLexerMode.AlloyCode, false);

        public readonly AlloyClassifierLexerMode Mode;
        public readonly bool InComment;

        public AlloyClassifierLexerState(AlloyClassifierLexerMode mode, bool inComment)
        {
            Mode = mode;
            InComment = inComment;
        }

        public bool Equals(AlloyClassifierLexerState other)
        {
            return this.Mode == other.Mode
                && this.InComment == other.InComment;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is AlloyClassifierLexerState))
                return false;

            return Equals((AlloyClassifierLexerState)obj);
        }

        public override int GetHashCode()
        {
            return (int)Mode ^ (InComment ? 1 : 0);
        }
    }
}
