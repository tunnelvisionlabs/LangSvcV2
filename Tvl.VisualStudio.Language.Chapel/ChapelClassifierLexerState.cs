namespace Tvl.VisualStudio.Language.Chapel
{
    using System;

    internal struct ChapelClassifierLexerState : IEquatable<ChapelClassifierLexerState>
    {
        public static readonly ChapelClassifierLexerState Initial =
            new ChapelClassifierLexerState(ChapelClassifierLexerMode.ChapelCode, false);

        public readonly ChapelClassifierLexerMode Mode;
        public readonly bool InComment;

        public ChapelClassifierLexerState(ChapelClassifierLexerMode mode, bool inComment)
        {
            Mode = mode;
            InComment = inComment;
        }

        public bool Equals(ChapelClassifierLexerState other)
        {
            return this.Mode == other.Mode
                && this.InComment == other.InComment;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ChapelClassifierLexerState))
                return false;

            return Equals((ChapelClassifierLexerState)obj);
        }

        public override int GetHashCode()
        {
            return (int)Mode ^ (InComment ? 1 : 0);
        }
    }
}
