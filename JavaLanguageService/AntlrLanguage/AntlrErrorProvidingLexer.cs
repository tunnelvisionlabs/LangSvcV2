namespace JavaLanguageService.AntlrLanguage
{
    using Antlr.Runtime;
    using Antlr3.Grammars;

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
