#if !ROSLYN

namespace Tvl.VisualStudio.InheritanceMargin.CSharp
{
    using System.Collections.Generic;
    using Microsoft.RestrictedUsage.CSharp.Semantics;

    internal class CSharpMemberIdentifierEqualityComparer : IEqualityComparer<CSharpMemberIdentifier>
    {
        public bool Equals(CSharpMemberIdentifier left, CSharpMemberIdentifier right)
        {
            if (object.ReferenceEquals(left, right))
                return true;

            if (object.ReferenceEquals(left, null) || object.ReferenceEquals(right, null))
                return false;

            return left.Equals(right);
        }

        public int GetHashCode(CSharpMemberIdentifier identifier)
        {
            return identifier.GetHashCode();
        }
    }
}

#endif
