namespace JavaLanguageService.AntlrLanguage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Language.Intellisense;
    using System.Collections.ObjectModel;

    internal sealed class AntlrCompletionSource : ICompletionSource
    {
        public ReadOnlyCollection<CompletionSet> GetCompletionInformation(ICompletionSession session)
        {
            return null;
        }
    }
}
