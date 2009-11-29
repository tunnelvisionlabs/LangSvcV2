namespace JavaLanguageService.AntlrLanguage
{
    using System.Collections.Generic;
    using Antlr.Runtime;
    using Microsoft.VisualStudio.Text;
    using Tvl.VisualStudio.Language.Parsing;

    internal class AntlrParseResultEventArgs : ParseResultEventArgs
    {
        public AntlrParseResultEventArgs(ITextSnapshot snapshot, IList<ParseErrorEventArgs> errors, ParserRuleReturnScope result)
            : base(snapshot, errors)
        {
            Result = result;
        }

        public ParserRuleReturnScope Result
        {
            get;
            private set;
        }
    }
}
