namespace Tvl.VisualStudio.Text.Navigation
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Utilities;

    public sealed class EditorNavigationTypeRegistryService : IEditorNavigationTypeRegistryService
    {
        [Export]
        [Name(PredefinedEditorNavigationTypes.Types)]
        [Order(Before = PredefinedEditorNavigationTypes.Members)]
        private static EditorNavigationTypeDefinition TypesStandardEditorNavigationTypeDefinition;

        [Export]
        [Name(PredefinedEditorNavigationTypes.Members)]
        private static readonly EditorNavigationTypeDefinition MembersStandardEditorNavigationTypeDefinition;

        private IEnumerable<Lazy<EditorNavigationTypeDefinition, IEditorNavigationTypeDefinitionMetadata>> _editorNavigationTypeDefinitions;

        private readonly Dictionary<string, EditorNavigationType> _navigationTypes =
            new Dictionary<string, EditorNavigationType>();

        public EditorNavigationTypeRegistryService()
        {
        }

        [ImportMany]
        private IEnumerable<Lazy<EditorNavigationTypeDefinition, IEditorNavigationTypeDefinitionMetadata>> EditorNavigationTypeDefinitions
        {
            get
            {
                return _editorNavigationTypeDefinitions;
            }
            set
            {
                if (_editorNavigationTypeDefinitions == value)
                    return;

                _editorNavigationTypeDefinitions = value;
                List<IEditorNavigationTypeDefinitionMetadata> navigationTypes = _editorNavigationTypeDefinitions.Select(definition => definition.Metadata).ToList();
                navigationTypes.RemoveAll(navigationType => _navigationTypes.ContainsKey(navigationType.Name));
                for (int i = navigationTypes.Count; i > 0; i--)
                {
                    int currentIndex = navigationTypes.FindIndex(navigationType => navigationType.BaseDefinition.All(_navigationTypes.ContainsKey));
                    if (currentIndex < 0)
                        throw new InvalidOperationException("Circular editor navigation type definition.");

                    var current = navigationTypes[currentIndex];
                    this._navigationTypes.Add(current.Name, new EditorNavigationType(current.Name, current.BaseDefinition.Select(GetEditorNavigationType)));
                    navigationTypes.RemoveAt(currentIndex);
                }
            }
        }

        public IEditorNavigationType CreateEditorNavigationType(string type, IEnumerable<IEditorNavigationType> baseTypes)
        {
            var navigationType = new EditorNavigationType(type, baseTypes);
            _navigationTypes.Add(type, navigationType);
            return navigationType;
        }

        public IEditorNavigationType CreateTransientEditorNavigationType(IEnumerable<IEditorNavigationType> baseTypes)
        {
            throw new NotImplementedException();
        }

        public IEditorNavigationType CreateTransientEditorNavigationType(params IEditorNavigationType[] baseTypes)
        {
            throw new NotImplementedException();
        }

        public IEditorNavigationType GetEditorNavigationType(string type)
        {
            EditorNavigationType navigationType;
            if (!this._navigationTypes.TryGetValue(type, out navigationType))
                return null;

            return navigationType;
        }
    }
}
