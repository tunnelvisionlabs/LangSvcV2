namespace JavaLanguageService.Language.Parsing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public interface IBackgroundParser
    {
        event EventHandler<ParseResultEventArgs> ParseComplete;
    }
}
