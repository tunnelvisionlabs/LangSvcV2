namespace Tvl.VisualStudio.Language.Antlr3
{
    using Antlr.Runtime;
    using global::Antlr3.Grammars;

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
