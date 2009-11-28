namespace JavaLanguageService.Text.Language
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Text;

    public interface ILanguageElementManager : IDisposable
    {
        event EventHandler<LanguageElementsChangedEventArgs> LanguageElementsChanged;

        IEnumerable<ILanguageElementTag> GetAllLanguageElements(SnapshotSpan span);
    }
}
