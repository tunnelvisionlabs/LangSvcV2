namespace Tvl.VisualStudio.Language.Java
{
    using System;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;

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

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void AugmentQuickInfoSession(IQuickInfoSession session, IList<object> quickInfoContent, out ITrackingSpan applicableToSpan)
        {
            applicableToSpan = null;
        }

        private void Dispose(bool disposing)
        {
        }
    }
}
