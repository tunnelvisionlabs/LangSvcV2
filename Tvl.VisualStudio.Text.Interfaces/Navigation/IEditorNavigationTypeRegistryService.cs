namespace Tvl.VisualStudio.Text.Navigation
{
    using System.Collections.Generic;
    using JetBrains.Annotations;

    public interface IEditorNavigationTypeRegistryService
    {
        [NotNull]
        IEditorNavigationType CreateEditorNavigationType([NotNull] EditorNavigationTypeDefinition definition, [NotNull] string type, IEnumerable<IEditorNavigationType> baseTypes);

        [NotNull]
        IEditorNavigationType CreateTransientEditorNavigationType([NotNull] IEnumerable<IEditorNavigationType> baseTypes);

        [NotNull]
        IEditorNavigationType CreateTransientEditorNavigationType([NotNull] params IEditorNavigationType[] baseTypes);

        IEditorNavigationType GetEditorNavigationType([NotNull] string type);
    }
}
