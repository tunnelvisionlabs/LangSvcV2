namespace Tvl.VisualStudio.Language.StringTemplate4
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Utilities;
    using Tvl.VisualStudio.Text.Navigation;

    public static class StringTemplateEditorNavigationTypes
    {
        public const string Templates = "Templates";

        [Export(typeof(EditorNavigationTypeDefinition))]
        [Name("Templates")]
        [Order(Before = PredefinedEditorNavigationTypes.Types)]
        internal sealed class TemplatesEditorNavigationTypeDefinition : EditorNavigationTypeDefinition
        {
            public TemplatesEditorNavigationTypeDefinition()
            {
                this.DisplayName = "Templates";
            }
        }
    }
}
