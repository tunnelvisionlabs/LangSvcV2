namespace Tvl.VisualStudio.Language.Antlr3
{
    using System;

    internal struct AntlrClassifierLexerState : IEquatable<AntlrClassifierLexerState>
    {
        public static readonly AntlrClassifierLexerState Initial =
            new AntlrClassifierLexerState(AntlrClassifierLexerMode.Grammar, 0, false, false, false);

        public readonly AntlrClassifierLexerMode Mode;
        public readonly int ActionLevel;
        public readonly bool InComment;
        public readonly bool InOptions;
        public readonly bool InTokens;

        public AntlrClassifierLexerState(AntlrClassifierLexerMode mode, int actionLevel, bool inComment, bool inOptions, bool inTokens)
        {
            Mode = mode;
            ActionLevel = actionLevel;
            InComment = inComment;
            InOptions = inOptions;
            InTokens = inTokens;
        }

        public bool Equals(AntlrClassifierLexerState other)
        {
            return this.Mode == other.Mode
                && this.ActionLevel == other.ActionLevel
                && this.InComment == other.InComment
                && this.InOptions == other.InOptions
                && this.InTokens == other.InTokens;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is AntlrClassifierLexerState))
                return false;

            return Equals((AntlrClassifierLexerState)obj);
        }

        public override int GetHashCode()
        {
            return (int)Mode ^ ActionLevel ^ (InComment ? 1 : 0) ^ (InOptions ? 2 : 0) ^ (InTokens ? 4 : 0);
        }
    }
}
