namespace Tvl.VisualStudio.Language.Alloy.Experimental
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.Text;
    using Tvl.VisualStudio.Language.Parsing;
    using Tvl.VisualStudio.Text.Navigation;
    using Antlr.Runtime;
    using Tvl.VisualStudio.Language.Parsing.Experimental.Interpreter;
    using Tvl.VisualStudio.Language.Parsing.Experimental.Atn;
    using Antlr.Runtime.Tree;

    internal sealed class AlloyAtnEditorNavigationSource : BackgroundParser, IEditorNavigationSource
    {
        private readonly AlloyAtnEditorNavigationSourceProvider _provider;
        private List<IEditorNavigationTarget> _navigationTargets;

        public AlloyAtnEditorNavigationSource(ITextBuffer textBuffer, AlloyAtnEditorNavigationSourceProvider provider)
            : base(textBuffer, provider.BackgroundIntelliSenseTaskScheduler, provider.TextDocumentFactoryService, provider.OutputWindowService)
        {
            _provider = provider;
            RequestParse(false);
        }

        public event EventHandler NavigationTargetsChanged;

        public override string Name
        {
            get
            {
                return "Editor Navigation Source";
            }
        }

        public IEnumerable<IEditorNavigationType> GetNavigationTypes()
        {
            yield return _provider.EditorNavigationTypeRegistryService.GetEditorNavigationType(PredefinedEditorNavigationTypes.Types);
            yield return _provider.EditorNavigationTypeRegistryService.GetEditorNavigationType(PredefinedEditorNavigationTypes.Members);
        }

        public IEnumerable<IEditorNavigationTarget> GetNavigationTargets()
        {
            return _navigationTargets ?? Enumerable.Empty<IEditorNavigationTarget>();
        }

        protected override void ReParseImpl()
        {
            // lex the entire document to get the set of identifiers we'll need to process
            ITextSnapshot snapshot = TextBuffer.CurrentSnapshot;
            var input = new SnapshotCharStream(snapshot, new Span(0, snapshot.Length));
            var lexer = new AlloyLexer(input);
            var tokens = new CommonTokenStream(lexer);
            tokens.Fill();

            /* Want to collect information from the following:
             *  - module (name)
             * Want to provide navigation info for the following types:
             *  - sig
             *  - enum
             * Want to provide navigation info for the following members:
             *  - decl (within a sigBody)
             *  - fun
             *  - pred
             *  - nameList (within an enumBody)
             * Eventually should consider the following:
             *  - cmdDecl
             *  - fact
             *  - assert
             */

            List<IToken> navigationKeywords = new List<IToken>();
            while (tokens.LA(1) != CharStreamConstants.EndOfFile)
            {
                switch (tokens.LA(1))
                {
                case AlloyLexer.KW_MODULE:
                case AlloyLexer.KW_SIG:
                case AlloyLexer.KW_ENUM:
                case AlloyLexer.KW_FUN:
                case AlloyLexer.KW_PRED:
                //case AlloyLexer.KW_ASSERT:
                //case AlloyLexer.KW_FACT:
                    navigationKeywords.Add(tokens.LT(1));
                    break;

                case CharStreamConstants.EndOfFile:
                    goto doneLexing;

                default:
                    break;
                }

                tokens.Consume();
            }

        doneLexing:

            List<IEditorNavigationTarget> navigationTargets = new List<IEditorNavigationTarget>();
            AstParserRuleReturnScope<CommonTree, IToken> moduleTree = null;
            CommonTreeAdaptor treeAdaptor = new CommonTreeAdaptor();

            foreach (var token in navigationKeywords)
            {
                tokens.Seek(token.TokenIndex);
                tokens.Consume();

                NetworkInterpreter interpreter = CreateNetworkInterpreter(tokens);
                while (interpreter.TryStepBackward())
                {
                    if (interpreter.Contexts.Count == 0)
                        break;

                    if (interpreter.Contexts.All(context => context.BoundedStart))
                        break;
                }

                interpreter.CombineBoundedStartContexts();

#if false // since we're using the AlloyParser, I don't think we need this.
                while (interpreter.TryStepForward())
                {
                    if (interpreter.Contexts.Count == 0)
                        break;

                    if (interpreter.Contexts.All(context => context.BoundedEnd))
                        break;
                }
#endif

                foreach (var context in interpreter.Contexts)
                {
                    switch (token.Type)
                    {
                    case AlloyLexer.KW_MODULE:
                        {
                            InterpretTraceTransition firstMatch = context.Transitions.FirstOrDefault(i => i.TokenIndex != null);
                            if (firstMatch == null)
                                continue;

                            tokens.Seek(firstMatch.TokenIndex.Value);
                            AlloyParser parser = new AlloyParser(tokens);
                            AstParserRuleReturnScope<CommonTree, IToken> result = parser.module();
                            if (result == null || parser.NumberOfSyntaxErrors > 0)
                                continue;

                            moduleTree = result;
                            break;
                        }

                    case AlloyLexer.KW_SIG:
                    case AlloyLexer.KW_ENUM:
                    case AlloyLexer.KW_FUN:
                    case AlloyLexer.KW_PRED:
                        {
                            InterpretTraceTransition firstMatch = context.Transitions.FirstOrDefault(i => i.TokenIndex != null);
                            if (firstMatch == null)
                                continue;

                            tokens.Seek(firstMatch.TokenIndex.Value);
                            AlloyParser parser = new AlloyParser(tokens);
                            AstParserRuleReturnScope<CommonTree, IToken> result = null;

                            switch (token.Type)
                            {
                            case AlloyLexer.KW_SIG:
                                result = parser.sigDeclNoBlock();
                                break;

                            case AlloyLexer.KW_ENUM:
                                result = parser.enumDecl();
                                break;

                            case AlloyLexer.KW_FUN:
                            case AlloyLexer.KW_PRED:
                                result = parser.funDeclGenericBody();
                                break;
                            }

                            if (result == null || parser.NumberOfSyntaxErrors > 0)
                                continue;

                            if (moduleTree != null)
                            {
                                object tree = treeAdaptor.Nil();
                                treeAdaptor.AddChild(tree, moduleTree.Tree);
                                treeAdaptor.AddChild(tree, result.Tree);
                                treeAdaptor.SetTokenBoundaries(tree, moduleTree.Start, result.Stop);

                                result.Start = moduleTree.Start;
                                result.Tree = (CommonTree)tree;
                            }

                            navigationTargets.AddRange(AlloyEditorNavigationSourceWalker.ExtractNavigationTargets(result, tokens.GetTokens().AsReadOnly(), _provider, snapshot));
                            break;
                        }

                    default:
                        continue;
                    }

                    break;
#if false
                    InterpretTraceTransition firstBraceTransition = context.Transitions.FirstOrDefault(i => i.Symbol == AlloyLexer.LBRACE);
                    InterpretTraceTransition lastBraceTransition = context.Transitions.LastOrDefault(i => i.Transition.IsMatch);
                    if (firstBraceTransition == null || lastBraceTransition == null)
                        continue;

                    if (token.Type == AlloyLexer.KW_SIG)
                    {
                        InterpretTraceTransition lastBodyBraceTransition = context.Transitions.LastOrDefault(i => i.Symbol == AlloyLexer.RBRACE && interpreter.Network.StateRules[i.Transition.SourceState.Id].Name == AlloyOutliningAtnBuilder.RuleNames.SigBody);
                        if (lastBodyBraceTransition != lastBraceTransition)
                        {
                            var bodySpan = OutlineBlock(firstBraceTransition.Token, lastBodyBraceTransition.Token, snapshot);
                            if (bodySpan != null)
                                navigationTargets.Add(bodySpan);

                            firstBraceTransition = context.Transitions.LastOrDefault(i => i.Symbol == AlloyLexer.LBRACE && i.TokenIndex > lastBodyBraceTransition.TokenIndex);
                        }
                    }

                    var blockSpan = OutlineBlock(firstBraceTransition.Token, lastBraceTransition.Token, snapshot);
                    if (blockSpan != null)
                        navigationTargets.Add(blockSpan);
#endif
                }
            }

            _navigationTargets = navigationTargets;
            OnNavigationTargetsChanged(new SnapshotSpanEventArgs(new SnapshotSpan(snapshot, new Span(0, snapshot.Length))));
        }

        private NetworkInterpreter CreateNetworkInterpreter(ITokenStream tokens)
        {
            Network network = NetworkBuilder<AlloyOutliningAtnBuilder>.GetOrBuildNetwork();

            NetworkInterpreter interpreter = new NetworkInterpreter(network, tokens);

            interpreter.BoundaryRules.Add(network.GetRule(AlloyOutliningAtnBuilder.RuleNames.AssertDecl));
            interpreter.BoundaryRules.Add(network.GetRule(AlloyOutliningAtnBuilder.RuleNames.EnumDecl));
            interpreter.BoundaryRules.Add(network.GetRule(AlloyOutliningAtnBuilder.RuleNames.FactDecl));
            interpreter.BoundaryRules.Add(network.GetRule(AlloyOutliningAtnBuilder.RuleNames.FunDecl));
            interpreter.BoundaryRules.Add(network.GetRule(AlloyOutliningAtnBuilder.RuleNames.SigDecl));

            interpreter.ExcludedStartRules.Add(network.GetRule(AlloyOutliningAtnBuilder.RuleNames.SigBody));
            interpreter.ExcludedStartRules.Add(network.GetRule(AlloyOutliningAtnBuilder.RuleNames.Block));

            return interpreter;
        }

        private void OnNavigationTargetsChanged(EventArgs e)
        {
            var t = NavigationTargetsChanged;
            if (t != null)
                t(this, e);
        }
    }
}
