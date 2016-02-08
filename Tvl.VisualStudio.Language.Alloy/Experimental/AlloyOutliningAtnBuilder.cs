namespace Tvl.VisualStudio.Language.Alloy.Experimental
{
    using Interval = Tvl.VisualStudio.Language.Parsing.Collections.Interval;
    using Nfa = Tvl.VisualStudio.Language.Parsing.Experimental.Atn.Nfa;

    /// <summary>
    /// This specialized ATN is efficient for locating the bounds of one of the outlined declarations
    /// (sig, fact, pred, assert, fun).
    /// </summary>
    internal class AlloyOutliningAtnBuilder : AlloySimplifiedAtnBuilder
    {
        private static readonly Interval[] BracesAndKeywords =
            {
                new Interval(AlloyLexer.LBRACE, 1),
                new Interval(AlloyLexer.RBRACE, 1),
                // only part of a enumDecl
                new Interval(AlloyLexer.KW_ENUM, 1),
                // only part of a factDecl
                new Interval(AlloyLexer.KW_FACT, 1),
                // only part of a assertDecl
                new Interval(AlloyLexer.KW_ASSERT, 1),
                // only part of a funDecl
                new Interval(AlloyLexer.KW_FUN, 1),
                new Interval(AlloyLexer.KW_PRED, 1),
                // only part of a sigDecl header:
                new Interval(AlloyLexer.KW_ABSTRACT, 1),
                new Interval(AlloyLexer.KW_SIG, 1),
                // only part of a cmdDecl:
                new Interval(AlloyLexer.KW_RUN, 1),
                new Interval(AlloyLexer.KW_CHECK, 1),
                new Interval(AlloyLexer.KW_EXPECT, 1),
                new Interval(AlloyLexer.KW_FOR, 1),
                new Interval(AlloyLexer.KW_BUT, 1),
                // only part of a cmdDecl or module
                new Interval(AlloyLexer.KW_EXACTLY, 1),
            };

        public AlloyOutliningAtnBuilder()
        {
        }

        protected override Nfa BuildBlockRule()
        {
            /* For outlining, this matches braces and ensures the seek doesn't run
             * into a keyword that can't be contained in the block.
             */
            return Nfa.Sequence(
                Nfa.Match(AlloyLexer.LBRACE),
                Nfa.Closure(
                    Nfa.Choice(
                        Nfa.MatchComplement(BracesAndKeywords),
                        Nfa.Rule(Bindings.Block))),
                Nfa.Match(AlloyLexer.RBRACE));
        }

        protected override Nfa BuildSigBodyRule()
        {
            /* For outlining, this matches braces and ensures the seek doesn't run
             * into a keyword that can't be contained in the block.
             */
            return Nfa.Sequence(
                Nfa.Match(AlloyLexer.LBRACE),
                Nfa.Closure(
                    Nfa.Choice(
                        Nfa.MatchComplement(BracesAndKeywords),
                        Nfa.Rule(Bindings.SigBody))),
                Nfa.Match(AlloyLexer.RBRACE));
        }
    }
}
