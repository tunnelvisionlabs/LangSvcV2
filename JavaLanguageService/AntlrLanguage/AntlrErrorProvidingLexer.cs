namespace JavaLanguageService.AntlrLanguage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Antlr3.Grammars;
    using Antlr.Runtime;

    internal class AntlrErrorProvidingLexer : ANTLRLexer
    {
        public AntlrErrorProvidingLexer(ICharStream input)
            : base(input)
        {
        }

        public AntlrErrorProvidingParser Parser
        {
            get;
            set;
        }
    }
}
