namespace Tvl.VisualStudio.Text.Navigation
{
    using System.Collections.Generic;

    public interface IEditorNavigationSourceMetadata
    {
        string ContentType
        {
            get;
        }

        IEnumerable<string> EditorNavigationTypes
        {
            get;
        }
    }
}
