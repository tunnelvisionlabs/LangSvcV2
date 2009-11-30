namespace Tvl.VisualStudio.Text
{
    using System;
    using System.Collections.Generic;

    public interface IEditorNavigationSource
    {
        event EventHandler NavigationTargetsChanged;

        IEnumerable<IEditorNavigationTarget> GetNavigationTargets();
    }
}
