namespace Tvl.VisualStudio.Language.Php
{
    using System;

    internal struct TemplateToken : IEquatable<TemplateToken>
    {
        internal readonly TemplateTokenKind Kind;
        internal readonly int Start;
        internal readonly int End;

        public TemplateToken(TemplateTokenKind kind, int start, int end)
        {
            Kind = kind;
            Start = start;
            End = end;
        }

        public override bool Equals(object obj)
        {
            if (obj is TemplateToken)
            {
                return Equals((TemplateToken)obj);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Kind.GetHashCode() ^ Start ^ End;
        }

        #region IEquatable<TemplateToken> Members

        public bool Equals(TemplateToken other)
        {
            return Kind == other.Kind &&
                Start == other.Start &&
                End == other.End;
        }

        #endregion
    }
}
