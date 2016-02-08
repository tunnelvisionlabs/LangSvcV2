namespace Tvl.VisualStudio.Language.Alloy
{
    using System;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;

    internal class AlloySignatureHelpSource : ISignatureHelpSource
    {
        private readonly ITextBuffer _textBuffer;
        private readonly AlloySignatureHelpSourceProvider _provider;

        public AlloySignatureHelpSource(ITextBuffer textBuffer, AlloySignatureHelpSourceProvider provider)
        {
            _textBuffer = textBuffer;
            _provider = provider;
        }

        public void AugmentSignatureHelpSession(ISignatureHelpSession session, IList<ISignature> signatures)
        {
            throw new NotImplementedException();
        }

        public ISignature GetBestMatch(ISignatureHelpSession session)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }
    }
}
