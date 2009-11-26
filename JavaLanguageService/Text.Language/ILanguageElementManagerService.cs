namespace JavaLanguageService.Text.Language
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Text.Editor;

    public interface ILanguageElementManagerService
    {
        ILanguageElementManager GetLanguageElementManager(ITextView textView);
    }
}
