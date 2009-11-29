namespace Tvl.VisualStudio.Language.Parsing
{
    using Microsoft.VisualStudio.Text;

    public interface IBackgroundParserFactoryService
    {
        IBackgroundParser GetBackgroundParser(ITextBuffer buffer);
    }
}
