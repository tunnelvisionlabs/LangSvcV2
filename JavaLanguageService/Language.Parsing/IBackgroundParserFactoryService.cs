namespace JavaLanguageService.Language.Parsing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Text;

    public interface IBackgroundParserFactoryService
    {
        IBackgroundParser GetBackgroundParser(ITextBuffer buffer);
    }
}
