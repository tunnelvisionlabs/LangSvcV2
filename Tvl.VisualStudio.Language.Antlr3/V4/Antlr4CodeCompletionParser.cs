namespace Tvl.VisualStudio.Language.AntlrV4
{
    using Antlr4.Runtime;

    internal class Antlr4CodeCompletionParser : GrammarParser, ICodeCompletionParser
    {
        public Antlr4CodeCompletionParser(ITokenStream input)
            : base(input)
        {
            Interpreter = new Antlr4CompletionParserATNSimulator(this, _ATN);
        }

        ITokenStream ICodeCompletionParser.InputStream
        {
            get
            {
                return (ITokenStream)base.InputStream;
            }
        }

        CompletionParserATNSimulator ICodeCompletionParser.Interpreter
        {
            get
            {
                return (CompletionParserATNSimulator)base.Interpreter;
            }
        }
    }
}
