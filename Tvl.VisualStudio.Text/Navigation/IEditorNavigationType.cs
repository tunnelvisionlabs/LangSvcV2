namespace Tvl.VisualStudio.Text.Navigation
{
    using System.Collections.Generic;

    public interface IEditorNavigationType
    {
        IEnumerable<IEditorNavigationType> BaseTypes
        {
            get;
        }

        string EditorNavigationType
        {
            get;
        }

        bool IsOfType(string type);
    }
}
