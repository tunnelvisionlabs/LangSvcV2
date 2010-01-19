namespace Tvl.VisualStudio.Language.Antlr3
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Language.Intellisense;
    using System.Collections.ObjectModel;
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

        public ReadOnlyCollection<CompletionSet> GetCompletionInformation(ICompletionSession session)
        {
            return null;
        }
    }
}
