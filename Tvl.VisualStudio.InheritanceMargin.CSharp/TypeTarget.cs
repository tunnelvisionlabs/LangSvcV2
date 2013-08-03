namespace Tvl.VisualStudio.InheritanceMargin.CSharp
{
    using Microsoft.RestrictedUsage.CSharp.Semantics;

    internal sealed class TypeTarget : IInheritanceTarget
    {
        private readonly string _displayName;
        private readonly CSharpTypeIdentifier _typeIdentifier;

        public TypeTarget(string displayName, CSharpTypeIdentifier typeIdentifier)
        {
            _displayName = displayName;
            _typeIdentifier = typeIdentifier;
        }

        public string DisplayName
        {
            get
            {
                return _displayName;
            }
        }

        public void NavigateTo()
        {
            CSharpInheritanceAnalyzer.NavigateToType(_typeIdentifier);
        }
    }
}
