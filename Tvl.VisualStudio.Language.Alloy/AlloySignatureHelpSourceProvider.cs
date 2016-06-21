namespace Tvl.VisualStudio.Language.Alloy
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Utilities;

    [Export(typeof(ISignatureHelpSourceProvider))]
    [Name("AlloySignatureHelpSourceProvider")]
    [ContentType(AlloyConstants.AlloyContentType)]
    [Order(Before = "default", After = "default")]
    // see VBSigHelpProviderFactory
    internal class AlloySignatureHelpSourceProvider : ISignatureHelpSourceProvider
    {
        public ISignatureHelpSource TryCreateSignatureHelpSource(ITextBuffer textBuffer)
        {
            return new AlloySignatureHelpSource(textBuffer, this);
        }
    }
}
