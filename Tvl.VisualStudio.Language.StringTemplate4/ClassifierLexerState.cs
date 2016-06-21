namespace Tvl.VisualStudio.Language.StringTemplate4
{
    using System;

    internal struct ClassifierLexerState : IEquatable<ClassifierLexerState>
    {
        internal static readonly ClassifierLexerState Initial =
            new ClassifierLexerState(
                TemplateLexerMode.Group,
                OutermostTemplate.None,
                0,
                false,
                '<',
                '>');

        internal readonly TemplateLexerMode Mode;
        internal readonly OutermostTemplate Outermost;
        internal readonly int AnonymousTemplateLevel;
        internal readonly bool InComment;
        internal readonly char OpenDelimiter;
        internal readonly char CloseDelimiter;

        public ClassifierLexerState(TemplateLexerMode mode, OutermostTemplate outermost, int anonymousTemplateLevel, bool inComment, char openDelimiter, char closeDelimiter)
        {
            Mode = mode;
            Outermost = outermost;
            AnonymousTemplateLevel = anonymousTemplateLevel;
            InComment = inComment;
            OpenDelimiter = openDelimiter;
            CloseDelimiter = closeDelimiter;
        }

        public bool Equals(ClassifierLexerState other)
        {
            return AnonymousTemplateLevel == other.AnonymousTemplateLevel
                && Mode == other.Mode
                && Outermost == other.Outermost
                && InComment == other.InComment
                && OpenDelimiter == other.OpenDelimiter
                && CloseDelimiter == other.CloseDelimiter;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ClassifierLexerState))
                return false;

            return Equals((ClassifierLexerState)obj);
        }

        public override int GetHashCode()
        {
            return this.AnonymousTemplateLevel.GetHashCode()
                ^ this.Mode.GetHashCode()
                ^ this.Outermost.GetHashCode()
                ^ this.InComment.GetHashCode()
                ^ this.OpenDelimiter.GetHashCode()
                ^ this.CloseDelimiter.GetHashCode();
        }
    }
}
