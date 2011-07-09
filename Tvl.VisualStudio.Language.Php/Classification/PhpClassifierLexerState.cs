namespace Tvl.VisualStudio.Language.Php.Classification
{
    internal struct PhpClassifierLexerState : System.IEquatable<PhpClassifierLexerState>
    {
        public static readonly PhpClassifierLexerState Initial = new PhpClassifierLexerState(PhpClassifierLexerMode.HtmlText, false, null, 0, false, 0);

        public readonly PhpClassifierLexerMode Mode;
        public readonly bool InString;
        public readonly string HeredocIdentifier;
        public readonly int StringBraceLevel;
        public readonly bool InStringExpression;
        public readonly int HtmlTagState;

        public PhpClassifierLexerState(PhpClassifierLexerMode mode, bool inString, string heredocIdentifier, int stringBraceLevel, bool inStringExpression, int htmlTagState)
        {
            this.Mode = mode;
            this.InString = inString;
            this.HeredocIdentifier = heredocIdentifier;
            this.StringBraceLevel = stringBraceLevel;
            this.InStringExpression = inStringExpression;
            this.HtmlTagState = htmlTagState;
        }

        public bool Equals(PhpClassifierLexerState other)
        {
            return Mode == other.Mode
                && InString == other.InString
                && HeredocIdentifier == other.HeredocIdentifier
                && StringBraceLevel == other.StringBraceLevel
                && InStringExpression == other.InStringExpression
                && HtmlTagState == other.HtmlTagState;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is PhpClassifierLexerState))
                return false;

            return Equals((PhpClassifierLexerState)obj);
        }

        public override int GetHashCode()
        {
            return this.Mode.GetHashCode()
                ^ this.InString.GetHashCode()
                ^ this.HeredocIdentifier.GetHashCode()
                ^ this.StringBraceLevel
                ^ this.InStringExpression.GetHashCode()
                ^ this.HtmlTagState;
        }
    }
}
