#if !ROSLYN

namespace Tvl.VisualStudio.InheritanceMargin.CSharp
{
    using Microsoft.RestrictedUsage.CSharp.Semantics;

    internal sealed class MemberTarget : IInheritanceTarget
    {
        private readonly CSharpMemberIdentifier _memberIdentifier;

        public MemberTarget(CSharpMemberIdentifier memberIdentifier)
        {
            _memberIdentifier = memberIdentifier;
        }

        public string DisplayName
        {
            get
            {
                return _memberIdentifier.ToString();
            }
        }

        public void NavigateTo()
        {
            CSharpInheritanceAnalyzer.NavigateToMember(_memberIdentifier);
        }
    }
}

#endif
