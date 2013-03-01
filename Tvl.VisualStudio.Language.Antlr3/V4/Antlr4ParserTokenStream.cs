namespace Tvl.VisualStudio.Language.AntlrV4
{
    using Antlr4.Runtime;

    internal class Antlr4ParserTokenStream : CommonTokenStream
    {
        public Antlr4ParserTokenStream(ITokenSource tokenSource)
            : base(tokenSource)
        {
        }

        public Parser Parser
        {
            get;
            set;
        }
    }
}
