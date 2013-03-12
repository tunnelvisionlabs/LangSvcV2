namespace Tvl.VisualStudio.Language.Parsing
{
    using Microsoft.VisualStudio.Text;

    public interface IBackgroundParserProvider
    {
        IBackgroundParser CreateParser(ITextBuffer buffer);
    }
}
