namespace JavaLanguageService.AntlrLanguage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Language.Intellisense;
    using System.Collections.ObjectModel;
    using Microsoft.VisualStudio.Text;

    internal sealed class AntlrQuickInfoSource : IQuickInfoSource
    {
        public ReadOnlyCollection<object> GetToolTipContent(IQuickInfoSession session, out ITrackingSpan applicableToSpan)
        {
            applicableToSpan = null;
            return null;
        }
    }
}
