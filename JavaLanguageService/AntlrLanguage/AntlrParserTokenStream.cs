namespace JavaLanguageService.AntlrLanguage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Antlr.Runtime;

    internal class AntlrParserTokenStream : CommonTokenStream
    {
        public AntlrParserTokenStream(ITokenSource tokenSource)
            : base(tokenSource)
        {
        }

        public AntlrErrorProvidingParser Parser
        {
            get;
            set;
        }
    }
}
