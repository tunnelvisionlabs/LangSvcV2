namespace Tvl.VisualStudio.Text.Navigation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class EditorNavigationType : IEditorNavigationType
    {
        public EditorNavigationType(string type, IEnumerable<IEditorNavigationType> baseTypes)
        {
            this.EditorNavigationType = type;
            this.BaseTypes = baseTypes.ToArray();
        }

        public IEnumerable<IEditorNavigationType> BaseTypes
        {
            get;
            private set;
        }

        public string EditorNavigationType
        {
            get;
            private set;
        }

        public bool IsOfType(string type)
        {
            throw new NotImplementedException();
        }
    }
}
