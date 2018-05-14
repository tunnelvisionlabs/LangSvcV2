namespace Tvl.VisualStudio.Language.Parsing
{
    using JetBrains.Annotations;
    using Microsoft.VisualStudio.Text;

    public interface IBackgroundParserFactoryService
    {
        IBackgroundParser GetBackgroundParser([NotNull] ITextBuffer buffer);
    }
}
