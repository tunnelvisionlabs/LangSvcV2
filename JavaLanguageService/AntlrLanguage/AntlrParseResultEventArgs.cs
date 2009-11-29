namespace JavaLanguageService.AntlrLanguage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Antlr.Runtime;
    using Antlr3.Tool;
    using Microsoft.VisualStudio.Text;

    internal class AntlrParseResultEventArgs : ParseResultEventArgs
    {
        public AntlrParseResultEventArgs(ITextSnapshot snapshot, ParserRuleReturnScope result, IList<ParseErrorEventArgs> errors)
            : base(snapshot, result, errors)
        {
        }
    }
}
