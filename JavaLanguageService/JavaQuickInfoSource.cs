namespace JavaLanguageService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Language.Intellisense;
    using System.Collections.ObjectModel;

    internal sealed class JavaQuickInfoSource : IQuickInfoSource
    {
        public JavaQuickInfoSource(ITextBuffer textBuffer)
        {
            this.TextBuffer = textBuffer;
        }

        public ITextBuffer TextBuffer
        {
            get;
            private set;
        }

        public ReadOnlyCollection<object> GetToolTipContent(IQuickInfoSession session, out ITrackingSpan applicableToSpan)
        {
            applicableToSpan = null;
            return null;
        }
    }
}
