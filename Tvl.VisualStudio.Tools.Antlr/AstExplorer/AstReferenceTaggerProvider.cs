namespace Tvl.VisualStudio.Tools.AstExplorer
{
    using System;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;

    [Export(typeof(ITaggerProvider))]
    [Export(typeof(IAstReferenceTaggerProvider))]
    [ContentType("text")]
    [TagType(typeof(TextMarkerTag))]
    internal sealed class AstReferenceTaggerProvider : IAstReferenceTaggerProvider, ITaggerProvider
    {
        public ITagger<T> CreateTagger<T>(ITextBuffer buffer)
            where T : ITag
        {
            return (ITagger<T>)GetAstReferenceTagger(buffer);
        }

        public SimpleTagger<TextMarkerTag> GetAstReferenceTagger(ITextBuffer buffer)
        {
            Func<SimpleTagger<TextMarkerTag>> creator = () => new SimpleTagger<TextMarkerTag>(buffer);
            return buffer.Properties.GetOrCreateSingletonProperty(typeof(AstReferenceTaggerProvider), creator);
        }
    }
}
