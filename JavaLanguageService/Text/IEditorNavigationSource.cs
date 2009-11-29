using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JavaLanguageService.Text
{
    public interface IEditorNavigationSource
    {
        event EventHandler NavigationTargetsChanged;

        IEnumerable<IEditorNavigationTarget> GetNavigationTargets();
    }
}
