namespace JavaLanguageService.Text
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Text;

    public interface IEditorNavigationSourceProvider
    {
        IEditorNavigationSource TryCreateEditorNavigationSource(ITextBuffer textBuffer);
    }
}
