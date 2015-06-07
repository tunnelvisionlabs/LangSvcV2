namespace Tvl.VisualStudio.Language.AntlrV4
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Atn;
    using Antlr4.Runtime.Misc;

    internal class Antlr4CompletionParserATNSimulator : CompletionParserATNSimulator
    {
        private static readonly IntervalSet WordlikeTokenTypes = new IntervalSet(
            // keywords
            GrammarLexer.OPTIONS,
            GrammarLexer.TOKENS,
            GrammarLexer.CHANNELS,
            GrammarLexer.IMPORT,
            GrammarLexer.FRAGMENT,
            GrammarLexer.LEXER,
            GrammarLexer.PARSER,
            GrammarLexer.GRAMMAR,
            GrammarLexer.PROTECTED,
            GrammarLexer.PUBLIC,
            GrammarLexer.PRIVATE,
            GrammarLexer.RETURNS,
            GrammarLexer.LOCALS,
            GrammarLexer.THROWS,
            GrammarLexer.CATCH,
            GrammarLexer.FINALLY,
            GrammarLexer.MODE,
            // atoms
            GrammarLexer.RULE_REF,
            GrammarLexer.TOKEN_REF,
            GrammarLexer.ID,
            // special
            GrammarLexer.ARG_ACTION_WORD,
            GrammarLexer.ACTION_REFERENCE,
            GrammarLexer.ACTION_WORD);

        public Antlr4CompletionParserATNSimulator(Parser parser, ATN atn)
            : base(parser, atn)
        {
            PredictionMode = PredictionMode.Sll;
        }

        protected override IntervalSet GetWordlikeTokenTypes()
        {
            return WordlikeTokenTypes;
        }
    }
}
