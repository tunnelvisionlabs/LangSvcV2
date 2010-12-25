namespace Tvl.VisualStudio.Language.Alloy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Language.Intellisense;

    // see VBCompletionProvider
    internal class AlloyCompletionSource : ICompletionSource
    {
        private readonly ITextBuffer _textBuffer;
        private readonly AlloyCompletionSourceProvider _provider;

        public AlloyCompletionSource(ITextBuffer textBuffer, AlloyCompletionSourceProvider provider)
        {
            this._textBuffer = textBuffer;
            this._provider = provider;
        }

        public void AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }
    }
}
