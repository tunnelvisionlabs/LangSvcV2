namespace Tvl.VisualStudio.Language.Go.Experimental
{
    using Interval = Tvl.VisualStudio.Language.Parsing.Collections.Interval;
    using Nfa = Tvl.VisualStudio.Language.Parsing.Experimental.Atn.Nfa;

    /// <summary>
    /// This specialized ATN is efficient for locating the bounds of one of the outlined declarations.
    /// </summary>
    internal class GoOutliningAtnBuilder : GoReducedAtnBuilder
    {
        private static readonly Interval[] BracesAndKeywords =
            {
                new Interval(GoLexer.LBRACE, 1),
                new Interval(GoLexer.RBRACE, 1),
            };

        protected override void BindRulesImpl()
        {
            base.BindRulesImpl();

            Bindings.CompilationUnit.IsStartRule = false;

            Bindings.ImportDecl.IsStartRule = true;
            Bindings.TypeDecl.IsStartRule = true;
            Bindings.ConstDecl.IsStartRule = true;
            Bindings.FunctionDecl.IsStartRule = true;
            Bindings.MethodDecl.IsStartRule = true;
            Bindings.VarDecl.IsStartRule = true;
            Bindings.StructType.IsStartRule = true;
        }

        protected override Nfa BuildBlockRule()
        {
            /* For outlining, this matches braces and ensures the seek doesn't run
             * into a keyword that can't be contained in the block.
             */
            return Nfa.Sequence(
                Nfa.Match(GoLexer.LBRACE),
                Nfa.Closure(
                    Nfa.Choice(
                        Nfa.MatchComplement(BracesAndKeywords),
                        Nfa.Rule(Bindings.Block))),
                Nfa.Match(GoLexer.RBRACE));
        }

        protected override Nfa BuildParametersRule()
        {
            /* For outlining, this matches braces and ensures the seek doesn't run
             * into a keyword that can't be contained in the block.
             */
            return Nfa.Sequence(
                Nfa.Match(GoLexer.LPAREN),
                Nfa.Closure(
                    Nfa.Choice(
                        Nfa.MatchComplement(new Interval(GoLexer.LPAREN, 1), new Interval(GoLexer.RPAREN, 1)),
                        Nfa.Rule(Bindings.Parameters))),
                Nfa.Match(GoLexer.RPAREN));
        }

        protected override Nfa BuildReceiverRule()
        {
            /* For outlining, this matches braces and ensures the seek doesn't run
             * into a keyword that can't be contained in the block.
             */
            return Nfa.Sequence(
                Nfa.Match(GoLexer.LPAREN),
                Nfa.Closure(
                    Nfa.Choice(
                        Nfa.MatchComplement(new Interval(GoLexer.LPAREN, 1), new Interval(GoLexer.RPAREN, 1)),
                        Nfa.Rule(Bindings.Receiver))),
                Nfa.Match(GoLexer.RPAREN));
        }

        protected override Nfa BuildImportDeclRule()
        {
            return Nfa.Sequence(
                Nfa.Match(GoLexer.KW_IMPORT),
                Nfa.Rule(Bindings.Parameters));
        }

        protected override Nfa BuildConstDeclRule()
        {
            return Nfa.Sequence(
                Nfa.Match(GoLexer.KW_CONST),
                Nfa.Rule(Bindings.Parameters));
        }

        protected override Nfa BuildVarDeclRule()
        {
            return Nfa.Sequence(
                Nfa.Match(GoLexer.KW_VAR),
                Nfa.Rule(Bindings.Parameters));
        }

        protected override Nfa BuildStructTypeRule()
        {
            return Nfa.Sequence(
                Nfa.Match(GoLexer.KW_STRUCT),
                Nfa.Rule(Bindings.Block));
        }
    }
}
