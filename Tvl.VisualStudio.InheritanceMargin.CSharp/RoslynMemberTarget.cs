#if ROSLYN

namespace Tvl.VisualStudio.InheritanceMargin.CSharp
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Text;

    internal sealed class MemberTarget : IInheritanceTarget
    {
        private readonly SourceTextContainer _textContainer;
        private readonly ISymbol _memberIdentifier;
        private readonly Solution _solution;

        public MemberTarget(SourceTextContainer textContainer, ISymbol memberIdentifier, Solution solution)
        {
            _textContainer = textContainer;
            _memberIdentifier = memberIdentifier;
            _solution = solution;
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
            CSharpInheritanceAnalyzer.NavigateToSymbol(_textContainer, _memberIdentifier, _solution.GetProject(_memberIdentifier.ContainingAssembly));
        }
    }
}

#endif
