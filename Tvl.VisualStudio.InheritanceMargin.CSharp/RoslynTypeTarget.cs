#if ROSLYN

namespace Tvl.VisualStudio.InheritanceMargin.CSharp
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Text;

    internal sealed class TypeTarget : IInheritanceTarget
    {
        private readonly SourceTextContainer _textContainer;
        private readonly ISymbol _typeIdentifier;
        private readonly Project _project;

        public TypeTarget(SourceTextContainer textContainer, ISymbol typeIdentifier, Project project)
        {
            _textContainer = textContainer;
            _typeIdentifier = typeIdentifier;
            _project = project;
        }

        public string DisplayName
        {
            get
            {
                return _typeIdentifier.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            }
        }

        public void NavigateTo()
        {
            CSharpInheritanceAnalyzer.NavigateToSymbol(_textContainer, _typeIdentifier, _project);
        }
    }
}

#endif
