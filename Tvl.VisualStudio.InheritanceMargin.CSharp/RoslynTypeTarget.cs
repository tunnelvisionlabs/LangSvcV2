#if ROSLYN

namespace Tvl.VisualStudio.InheritanceMargin.CSharp
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Text;

    internal sealed class TypeTarget : IInheritanceTarget
    {
        private readonly SourceTextContainer _textContainer;
        private readonly ISymbol _typeIdentifier;
        private readonly Solution _solution;

        public TypeTarget(SourceTextContainer textContainer, ISymbol typeIdentifier, Solution solution)
        {
            _textContainer = textContainer;
            _typeIdentifier = typeIdentifier;
            _solution = solution;
        }

        public string DisplayName
        {
            get
            {
                return _typeIdentifier.ToString();
            }
        }

        public void NavigateTo()
        {
            CSharpInheritanceAnalyzer.NavigateToSymbol(_textContainer, _typeIdentifier, _solution.GetProject(_typeIdentifier.ContainingAssembly));
        }
    }
}

#endif
