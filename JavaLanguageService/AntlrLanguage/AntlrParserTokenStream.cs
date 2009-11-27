namespace JavaLanguageService.AntlrLanguage
{
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
