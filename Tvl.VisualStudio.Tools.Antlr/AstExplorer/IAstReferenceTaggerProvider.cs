namespace Tvl.VisualStudio.Tools.AstExplorer
{
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;

    internal interface IAstReferenceTaggerProvider
    {
        SimpleTagger<TextMarkerTag> GetAstReferenceTagger(ITextBuffer buffer);
    }
}
