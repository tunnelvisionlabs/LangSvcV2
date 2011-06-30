namespace Tvl.VisualStudio.Language.Antlr3
{
    using System;

    internal struct AntlrClassifierLexerState : IEquatable<AntlrClassifierLexerState>
    {
        public static readonly AntlrClassifierLexerState Initial =
            new AntlrClassifierLexerState(AntlrClassifierLexerMode.Grammar, 0, false);

        public readonly AntlrClassifierLexerMode Mode;
        public readonly int ActionLevel;
        public readonly bool InComment;

        public AntlrClassifierLexerState(AntlrClassifierLexerMode mode, int actionLevel, bool inComment)
        {
            Mode = mode;
            ActionLevel = actionLevel;
            InComment = inComment;
        }

        public bool Equals(AntlrClassifierLexerState other)
        {
            return this.Mode == other.Mode
                && this.ActionLevel == other.ActionLevel
                && this.InComment == other.InComment;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is AntlrClassifierLexerState))
                return false;

            return Equals((AntlrClassifierLexerState)obj);
        }

        public override int GetHashCode()
        {
            return (int)Mode ^ ActionLevel ^ (InComment ? 1 : 0);
        }
    }
}
