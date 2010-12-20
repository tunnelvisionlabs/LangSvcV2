namespace Tvl.VisualStudio.Language.Antlr3
{
    using System;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;

    internal sealed class AntlrCompletionSource : ICompletionSource
    {
        public AntlrCompletionSource(ITextBuffer buffer)
        {
            this.TextBuffer = buffer;
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

        public void AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets)
        {
            throw new NotImplementedException();
        }

        private void Dispose(bool disposing)
        {
        }
    }
}
