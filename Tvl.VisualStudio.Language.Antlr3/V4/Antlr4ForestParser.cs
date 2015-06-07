namespace Tvl.VisualStudio.Language.AntlrV4
{
    using System;
    using Antlr4.Runtime;

    internal class Antlr4ForestParser : ForestParser<Antlr4CodeCompletionParser>
    {
        public static readonly Antlr4ForestParser Rules = new Antlr4ForestParser(GrammarParser.RULE_rules);
        public static readonly Antlr4ForestParser GrammarSpec = new Antlr4ForestParser(GrammarParser.RULE_grammarSpec);

        private readonly int _startRule;

        private Antlr4ForestParser(int startRule)
        {
            switch (startRule)
            {
            case GrammarParser.RULE_rules:
            case GrammarParser.RULE_grammarSpec:
                break;

            default:
                throw new ArgumentException();
            }

            _startRule = startRule;
        }

        [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_rules, 0, Dependents.Self)]
        [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_grammarSpec, 0, Dependents.Self)]
        protected override RuleContext ParseImpl(Antlr4CodeCompletionParser parser)
        {
            switch (_startRule)
            {
            case GrammarParser.RULE_rules:
                return parser.rules();

            case GrammarParser.RULE_grammarSpec:
                return parser.grammarSpec();

            default:
                throw new NotSupportedException();
            }
        }
    }
}
