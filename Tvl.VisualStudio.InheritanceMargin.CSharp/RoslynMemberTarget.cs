#if ROSLYN

namespace Tvl.VisualStudio.InheritanceMargin.CSharp
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Text;

    internal sealed class MemberTarget : IInheritanceTarget
    {
        private readonly SourceTextContainer _textContainer;
        private readonly ISymbol _memberIdentifier;
        private readonly Project _project;

        public MemberTarget(SourceTextContainer textContainer, ISymbol memberIdentifier, Project project)
        {
            _textContainer = textContainer;
            _memberIdentifier = memberIdentifier;
            _project = project;
        }

        public string DisplayName
        {
            get
            {
                return _memberIdentifier.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            }
        }

        public void NavigateTo()
        {
            CSharpInheritanceAnalyzer.NavigateToSymbol(_textContainer, _memberIdentifier, _project);
        }
    }
}

#endif
