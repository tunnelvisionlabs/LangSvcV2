namespace Tvl.VisualStudio.Language.AntlrV4
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Atn;

    public interface ICodeCompletionParser
    {
        CompletionParserATNSimulator Interpreter
        {
            get;
        }

        ParserRuleContext Context
        {
            get;
        }

        ITokenStream InputStream
        {
            get;
        }

        ATN Atn
        {
            get;
        }
    }
}
