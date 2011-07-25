namespace Tvl.VisualStudio.Language.Go.Experimental
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Antlr.Runtime;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Text.Tagging;
    using Tvl.VisualStudio.Language.Parsing;
    using Tvl.VisualStudio.Language.Parsing.Experimental.Atn;
    using Tvl.VisualStudio.Language.Parsing.Experimental.Interpreter;
    using Tvl.VisualStudio.Shell.OutputWindow;
    using System.Diagnostics.Contracts;
    using System.Diagnostics;

    internal sealed class GoSymbolTagger : BackgroundParser, ITagger<IClassificationTag>
    {
        private readonly IClassificationTypeRegistryService _classificationTypeRegistryService;

        private List<ITagSpan<IClassificationTag>> _tags = new List<ITagSpan<IClassificationTag>>();

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public GoSymbolTagger(ITextBuffer textBuffer, IClassificationTypeRegistryService classificationTypeRegistryService, TaskScheduler taskScheduler, ITextDocumentFactoryService textDocumentFactoryService, IOutputWindowService outputWindowService)
            : base(textBuffer, taskScheduler, textDocumentFactoryService, outputWindowService)
        {
            _classificationTypeRegistryService = classificationTypeRegistryService;
            RequestParse(false);
        }

        public override string Name
        {
            get
            {
                return "Symbol Tagger";
            }
        }

        public IEnumerable<ITagSpan<IClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            return _tags;
        }

        private void OnTagsChanged(SnapshotSpanEventArgs e)
        {
            var t = TagsChanged;
            if (t != null)
                t(this, e);
        }

        protected override void ReParseImpl()
        {
            Stopwatch timer = Stopwatch.StartNew();

            // lex the entire document to get the set of identifiers we'll need to classify
            ITextSnapshot snapshot = TextBuffer.CurrentSnapshot;
            var input = new SnapshotCharStream(snapshot, new Span(0, snapshot.Length));
            var lexer = new GoLexer(input);
            var tokenSource = new GoSemicolonInsertionTokenSource(lexer);
            var tokens = new CommonTokenStream(tokenSource);
            tokens.Fill();

            /* easy to handle the following definitions:
             *  - module (name)
             *  - open (external symbol reference) ... as (name)
             *  - fact (name)?
             *  - assert (name)?
             *  - fun (ref.name | name)
             *  - pred (ref.name | name)
             *  - (name): run|check
             *  - sig (namelist)
             *  - enum (name)
             * moderate to handle the following definitions:
             *  - decl name(s)
             * harder to handle the following definitions:
             */

            /* A single name follows the following keywords:
             *  - KW_MODULE
             *  - KW_OPEN
             *  - KW_AS
             *  - KW_ENUM
             *  - KW_FACT (name is optional)
             *  - KW_ASSERT (name is optional)
             */
            List<IToken> nameKeywords = new List<IToken>();
            List<IToken> declColons = new List<IToken>();

            List<IToken> identifiers = new List<IToken>();
            while (tokens.LA(1) != CharStreamConstants.EndOfFile)
            {
                switch (tokens.LA(1))
                {
                case GoLexer.IDENTIFIER:
                    identifiers.Add(tokens.LT(1));
                    break;

                case GoLexer.KW_PACKAGE:
                case GoLexer.KW_IMPORT:
                case GoLexer.KW_TYPE:
                case GoLexer.KW_VAR:
                case GoLexer.KW_FUNC:
                //case GoLexer.KW_MODULE:
                //case GoLexer.KW_OPEN:
                //case GoLexer.KW_AS:
                //case GoLexer.KW_ENUM:
                //case GoLexer.KW_FACT:
                //case GoLexer.KW_ASSERT:
                //case GoLexer.KW_RUN:
                //case GoLexer.KW_CHECK:
                //case GoLexer.KW_EXTENDS:

                //case GoLexer.KW_FUN:
                //case GoLexer.KW_PRED:

                //case GoLexer.KW_SIG:
                    nameKeywords.Add(tokens.LT(1));
                    break;

                case GoLexer.DEFEQ:
                    declColons.Add(tokens.LT(1));
                    break;

                case CharStreamConstants.EndOfFile:
                    goto doneLexing;

                default:
                    break;
                }

                tokens.Consume();
            }

        doneLexing:

            HashSet<IToken> definitions = new HashSet<IToken>(TokenIndexEqualityComparer.Default);
            HashSet<IToken> references = new HashSet<IToken>(TokenIndexEqualityComparer.Default);

            foreach (var token in nameKeywords)
            {
                tokens.Seek(token.TokenIndex);
                NetworkInterpreter interpreter = CreateTopLevelNetworkInterpreter(tokens);
                while (interpreter.TryStepForward())
                {
                    if (interpreter.Contexts.Count == 0 || interpreter.Contexts.Count > 400)
                        break;

                    if (interpreter.Contexts.All(context => context.BoundedEnd))
                        break;
                }

                interpreter.CombineBoundedEndContexts();

                foreach (var context in interpreter.Contexts)
                {
                    foreach (var transition in context.Transitions)
                    {
                        if (!transition.Symbol.HasValue)
                            continue;

                        switch (transition.Symbol)
                        {
                        case GoLexer.IDENTIFIER:
                        //case GoLexer.KW_THIS:
                            RuleBinding rule = interpreter.Network.StateRules[transition.Transition.TargetState.Id];
                            if (rule.Name != GoSimplifiedAtnBuilder.RuleNames.SymbolDefinitionIdentifier)
                                references.Add(tokens.Get(transition.TokenIndex.Value));
                            if (rule.Name != GoSimplifiedAtnBuilder.RuleNames.SymbolReferenceIdentifier)
                                definitions.Add(tokens.Get(transition.TokenIndex.Value));
                            break;

                        default:
                            continue;
                        }
                    }
                }
            }

            foreach (var token in declColons)
            {
                tokens.Seek(token.TokenIndex);
                tokens.Consume();

                NetworkInterpreter interpreter = CreateFullNetworkInterpreter(tokens);
                while (interpreter.TryStepBackward())
                {
                    if (interpreter.Contexts.Count == 0 || interpreter.Contexts.Count > 400)
                        break;

                    if (interpreter.Contexts.All(context => context.BoundedStart))
                        break;

                    interpreter.Contexts.RemoveAll(i => !IsConsistentWithPreviousResult(i, true, definitions, references));
                }

                interpreter.CombineBoundedStartContexts();

                while (interpreter.TryStepForward())
                {
                    if (interpreter.Contexts.Count == 0 || interpreter.Contexts.Count > 400)
                        break;

                    if (interpreter.Contexts.All(context => context.BoundedEnd))
                        break;

                    interpreter.Contexts.RemoveAll(i => !IsConsistentWithPreviousResult(i, false, definitions, references));
                }

                interpreter.CombineBoundedEndContexts();

                foreach (var context in interpreter.Contexts)
                {
                    foreach (var transition in context.Transitions)
                    {
                        if (!transition.Symbol.HasValue)
                            continue;

                        switch (transition.Symbol)
                        {
                        case GoLexer.IDENTIFIER:
                        //case GoLexer.KW_THIS:
                            RuleBinding rule = interpreter.Network.StateRules[transition.Transition.TargetState.Id];
                            if (rule.Name != GoSimplifiedAtnBuilder.RuleNames.SymbolDefinitionIdentifier)
                                references.Add(tokens.Get(transition.TokenIndex.Value));
                            if (rule.Name != GoSimplifiedAtnBuilder.RuleNames.SymbolReferenceIdentifier)
                                definitions.Add(tokens.Get(transition.TokenIndex.Value));
                            break;

                        default:
                            continue;
                        }
                    }
                }
            }

            foreach (var token in identifiers)
            {
                if (definitions.Contains(token) || references.Contains(token))
                    continue;

                tokens.Seek(token.TokenIndex);
                tokens.Consume();

                NetworkInterpreter interpreter = CreateFullNetworkInterpreter(tokens);

                while (interpreter.TryStepBackward())
                {
                    if (interpreter.Contexts.Count == 0 || interpreter.Contexts.Count > 400)
                        break;

                    if (interpreter.Contexts.All(context => context.BoundedStart))
                        break;

                    interpreter.Contexts.RemoveAll(i => !IsConsistentWithPreviousResult(i, true, definitions, references));

                    if (AllAgree(interpreter.Contexts))
                        break;
                }

                interpreter.CombineBoundedStartContexts();

                while (interpreter.TryStepForward())
                {
                    if (interpreter.Contexts.Count == 0 || interpreter.Contexts.Count > 400)
                        break;

                    if (interpreter.Contexts.All(context => context.BoundedEnd))
                        break;

                    interpreter.Contexts.RemoveAll(i => !IsConsistentWithPreviousResult(i, false, definitions, references));

                    if (AllAgree(interpreter.Contexts))
                        break;
                }

                interpreter.CombineBoundedEndContexts();

                foreach (var context in interpreter.Contexts)
                {
                    foreach (var transition in context.Transitions)
                    {
                        if (!transition.Symbol.HasValue)
                            continue;

                        switch (transition.Symbol)
                        {
                        case GoLexer.IDENTIFIER:
                        //case GoLexer.KW_THIS:
                            RuleBinding rule = interpreter.Network.StateRules[transition.Transition.TargetState.Id];
                            if (rule.Name != GoSimplifiedAtnBuilder.RuleNames.SymbolDefinitionIdentifier)
                                references.Add(tokens.Get(transition.TokenIndex.Value));
                            if (rule.Name != GoSimplifiedAtnBuilder.RuleNames.SymbolReferenceIdentifier)
                                definitions.Add(tokens.Get(transition.TokenIndex.Value));
                            break;

                        default:
                            continue;
                        }
                    }
                }
            }

            // tokens which are in both the 'definitions' and 'references' sets are actually unknown.
            HashSet<IToken> unknownIdentifiers = new HashSet<IToken>(definitions, TokenIndexEqualityComparer.Default);
            unknownIdentifiers.IntersectWith(references);
            definitions.ExceptWith(unknownIdentifiers);
            references.ExceptWith(unknownIdentifiers);

            // the full set of unknown identifiers are any that aren't explicitly classified as a definition or a reference
            unknownIdentifiers = new HashSet<IToken>(identifiers, TokenIndexEqualityComparer.Default);
            unknownIdentifiers.ExceptWith(definitions);
            unknownIdentifiers.ExceptWith(references);

            List<ITagSpan<IClassificationTag>> tags = new List<ITagSpan<IClassificationTag>>();

            IClassificationType definitionClassificationType = _classificationTypeRegistryService.GetClassificationType(GoSymbolTaggerClassificationTypeNames.Definition);
            tags.AddRange(ClassifyTokens(snapshot, definitions, new ClassificationTag(definitionClassificationType)));

            IClassificationType referenceClassificationType = _classificationTypeRegistryService.GetClassificationType(GoSymbolTaggerClassificationTypeNames.Reference);
            tags.AddRange(ClassifyTokens(snapshot, references, new ClassificationTag(referenceClassificationType)));

            IClassificationType unknownClassificationType = _classificationTypeRegistryService.GetClassificationType(GoSymbolTaggerClassificationTypeNames.UnknownIdentifier);
            tags.AddRange(ClassifyTokens(snapshot, unknownIdentifiers, new ClassificationTag(unknownClassificationType)));

            _tags = tags;

            timer.Stop();

            IOutputWindowPane pane = OutputWindowService.TryGetPane(PredefinedOutputWindowPanes.TvlIntellisense);
            if (pane != null)
                pane.WriteLine(string.Format("Finished classifying {0} identifiers in {1}ms: {2} definitions, {3} references, {4} unknown", identifiers.Count, timer.ElapsedMilliseconds, definitions.Count, references.Count, unknownIdentifiers.Count));

            OnTagsChanged(new SnapshotSpanEventArgs(new SnapshotSpan(snapshot, new Span(0, snapshot.Length))));
        }

        private bool IsConsistentWithPreviousResult(InterpretTrace trace, bool checkStart, HashSet<IToken> definitions, HashSet<IToken> references)
        {
            Contract.Requires(trace != null);
            Contract.Requires(definitions != null);
            Contract.Requires(references != null);

            InterpretTraceTransition transition = checkStart ? trace.Transitions.First.Value : trace.Transitions.Last.Value;
            IToken token = transition.Token;
            if (definitions.Contains(token) && !references.Contains(token))
            {
                if (transition.Interpreter.Network.StateRules[transition.Transition.SourceState.Id].Name != GoSimplifiedAtnBuilder.RuleNames.SymbolDefinitionIdentifier)
                    return false;
            }
            else if (references.Contains(token) && !definitions.Contains(token))
            {
                if (transition.Interpreter.Network.StateRules[transition.Transition.SourceState.Id].Name != GoSimplifiedAtnBuilder.RuleNames.SymbolReferenceIdentifier)
                    return false;
            }

            return true;
        }

        private static bool AllAgree(IEnumerable<InterpretTrace> contexts)
        {
            var symbolTransitions = contexts.SelectMany(i => i.Transitions).Where(i => i.Symbol != null);
            var grouped = symbolTransitions.GroupBy(i => i.TokenIndex);
            foreach (var group in grouped)
            {
                if (group.First().Token.Type != GoLexer.IDENTIFIER)
                    continue;

                bool hasDefinition = false;
                bool hasReference = false;
                foreach (var item in group)
                {
                    string ruleName = item.Interpreter.Network.StateRules[item.Transition.SourceState.Id].Name;
                    if (ruleName == GoSimplifiedAtnBuilder.RuleNames.SymbolDefinitionIdentifier)
                        hasDefinition = true;
                    else if (ruleName == GoSimplifiedAtnBuilder.RuleNames.SymbolReferenceIdentifier)
                        hasReference = true;
                    else
                        return false;

                    if (hasDefinition && hasReference)
                        return false;
                }
            }

            return true;
        }

        private NetworkInterpreter CreateTopLevelNetworkInterpreter(ITokenStream tokens)
        {
            Network network = NetworkBuilder<GoTopLevelSymbolTaggerAtnBuilder>.GetOrBuildNetwork();

            NetworkInterpreter interpreter = new NetworkInterpreter(network, tokens);

            //RuleBinding memberSelectRule = network.GetRule(GoSimplifiedAtnBuilder.RuleNames.BinOpExpr18);
            //interpreter.BoundaryRules.Add(memberSelectRule);

            ////interpreter.BoundaryRules.Add(network.GetRule(GoSimplifiedAtnBuilder.RuleNames.LetDecl));
            ////interpreter.BoundaryRules.Add(network.GetRule(GoSimplifiedAtnBuilder.RuleNames.QuantDecls));
            ////interpreter.BoundaryRules.Add(network.GetRule(GoSimplifiedAtnBuilder.RuleNames.Decl));
            //////interpreter.BoundaryRules.Add(network.GetRule(GoSimplifiedAtnBuilder.RuleNames.NameList));
            //////interpreter.BoundaryRules.Add(network.GetRule(GoSimplifiedAtnBuilder.RuleNames.NameListName));
            ////interpreter.BoundaryRules.Add(network.GetRule(GoSimplifiedAtnBuilder.RuleNames.Ref));
            ////interpreter.BoundaryRules.Add(network.GetRule(GoSimplifiedAtnBuilder.RuleNames.Open));
            ////interpreter.BoundaryRules.Add(network.GetRule(GoSimplifiedAtnBuilder.RuleNames.FactDecl));
            ////interpreter.BoundaryRules.Add(network.GetRule(GoSimplifiedAtnBuilder.RuleNames.AssertDecl));
            //////interpreter.BoundaryRules.Add(network.GetRule(GoSimplifiedAtnBuilder.RuleNames.FunDecl));
            //////interpreter.BoundaryRules.Add(network.GetRule(GoSimplifiedAtnBuilder.RuleNames.FunctionName));
            ////interpreter.BoundaryRules.Add(network.GetRule(GoSimplifiedAtnBuilder.RuleNames.CmdDecl));
            ////interpreter.BoundaryRules.Add(network.GetRule(GoSimplifiedAtnBuilder.RuleNames.Typescope));
            ////interpreter.BoundaryRules.Add(network.GetRule(GoSimplifiedAtnBuilder.RuleNames.EnumDecl));
            ////interpreter.BoundaryRules.Add(network.GetRule(GoSimplifiedAtnBuilder.RuleNames.ElseClause));
            ////interpreter.BoundaryRules.Add(network.GetRule(GoSimplifiedAtnBuilder.RuleNames.Module));

            //interpreter.BoundaryRules.Add(network.GetRule(GoSimplifiedAtnBuilder.RuleNames.LetDecl));
            //interpreter.BoundaryRules.Add(network.GetRule(GoSimplifiedAtnBuilder.RuleNames.QuantDecls));
            //interpreter.BoundaryRules.Add(network.GetRule(GoSimplifiedAtnBuilder.RuleNames.Decl));

            ///* adding this rule definitely didn't help! */
            ////interpreter.BoundaryRules.Add(network.GetRule(GoSimplifiedAtnBuilder.RuleNames.Expr));

            //interpreter.BoundaryRules.Add(network.GetRule(GoSimplifiedAtnBuilder.RuleNames.Module));
            //interpreter.BoundaryRules.Add(network.GetRule(GoSimplifiedAtnBuilder.RuleNames.FactDeclHeader));
            //interpreter.BoundaryRules.Add(network.GetRule(GoSimplifiedAtnBuilder.RuleNames.AssertDeclHeader));
            //interpreter.BoundaryRules.Add(network.GetRule(GoSimplifiedAtnBuilder.RuleNames.FunFunctionName));
            //interpreter.BoundaryRules.Add(network.GetRule(GoSimplifiedAtnBuilder.RuleNames.PredFunctionName));
            //interpreter.BoundaryRules.Add(network.GetRule(GoSimplifiedAtnBuilder.RuleNames.FunctionReturn));
            //interpreter.BoundaryRules.Add(network.GetRule(GoSimplifiedAtnBuilder.RuleNames.SigDeclHeader));
            //interpreter.BoundaryRules.Add(network.GetRule(GoSimplifiedAtnBuilder.RuleNames.SigExt));

            // make sure we can handle forward walking from 'package'
            interpreter.BoundaryRules.Add(network.GetRule(GoSimplifiedAtnBuilder.RuleNames.PackageClause));
            // make sure we can handle forward walking from 'import'
            interpreter.BoundaryRules.Add(network.GetRule(GoSimplifiedAtnBuilder.RuleNames.ImportDecl));
            // make sure we can handle forward walking from 'type'
            interpreter.BoundaryRules.Add(network.GetRule(GoSimplifiedAtnBuilder.RuleNames.TypeDecl));
            interpreter.BoundaryRules.Add(network.GetRule(GoSimplifiedAtnBuilder.RuleNames.TypeSwitchGuard));
            // make sure we can handle forward walking from 'var'
            interpreter.BoundaryRules.Add(network.GetRule(GoSimplifiedAtnBuilder.RuleNames.VarDecl));
            // make sure we can handle forward walking from 'func'
            interpreter.BoundaryRules.Add(network.GetRule(GoSimplifiedAtnBuilder.RuleNames.FunctionType));
            interpreter.BoundaryRules.Add(network.GetRule(GoSimplifiedAtnBuilder.RuleNames.FunctionDeclHeader));
            interpreter.BoundaryRules.Add(network.GetRule(GoSimplifiedAtnBuilder.RuleNames.MethodDeclHeader));

            // make sure we can handle forward and backward walking from ':='
            interpreter.BoundaryRules.Add(network.GetRule(GoSimplifiedAtnBuilder.RuleNames.ShortVarDecl));
            interpreter.BoundaryRules.Add(network.GetRule(GoSimplifiedAtnBuilder.RuleNames.SimpleStmt));
            interpreter.BoundaryRules.Add(network.GetRule(GoSimplifiedAtnBuilder.RuleNames.RangeClause));
            interpreter.BoundaryRules.Add(network.GetRule(GoSimplifiedAtnBuilder.RuleNames.CommCase));

            interpreter.ExcludedStartRules.Add(network.GetRule(GoSimplifiedAtnBuilder.RuleNames.Block));

            return interpreter;
        }

        private NetworkInterpreter CreateFullNetworkInterpreter(ITokenStream tokens)
        {
            Network network = NetworkBuilder<GoReducedAtnBuilder>.GetOrBuildNetwork();

            NetworkInterpreter interpreter = new NetworkInterpreter(network, tokens);

            // make sure we can handle forward walking from 'package'
            interpreter.BoundaryRules.Add(network.GetRule(GoSimplifiedAtnBuilder.RuleNames.PackageClause));
            // make sure we can handle forward walking from 'import'
            interpreter.BoundaryRules.Add(network.GetRule(GoSimplifiedAtnBuilder.RuleNames.ImportDecl));
            // make sure we can handle forward walking from 'type'
            interpreter.BoundaryRules.Add(network.GetRule(GoSimplifiedAtnBuilder.RuleNames.TypeDecl));
            interpreter.BoundaryRules.Add(network.GetRule(GoSimplifiedAtnBuilder.RuleNames.TypeSwitchGuard));
            // make sure we can handle forward walking from 'var'
            interpreter.BoundaryRules.Add(network.GetRule(GoSimplifiedAtnBuilder.RuleNames.VarDecl));
            // make sure we can handle forward walking from 'func'
            interpreter.BoundaryRules.Add(network.GetRule(GoSimplifiedAtnBuilder.RuleNames.FunctionType));
            interpreter.BoundaryRules.Add(network.GetRule(GoSimplifiedAtnBuilder.RuleNames.FunctionDeclHeader));
            interpreter.BoundaryRules.Add(network.GetRule(GoSimplifiedAtnBuilder.RuleNames.MethodDeclHeader));

            // make sure we can handle forward and backward walking from ':='
            interpreter.BoundaryRules.Add(network.GetRule(GoSimplifiedAtnBuilder.RuleNames.ShortVarDecl));
            interpreter.BoundaryRules.Add(network.GetRule(GoSimplifiedAtnBuilder.RuleNames.SimpleStmt));
            interpreter.BoundaryRules.Add(network.GetRule(GoSimplifiedAtnBuilder.RuleNames.RangeClause));
            interpreter.BoundaryRules.Add(network.GetRule(GoSimplifiedAtnBuilder.RuleNames.CommCase));

            interpreter.BoundaryRules.Add(network.GetRule(GoSimplifiedAtnBuilder.RuleNames.Expression));
            interpreter.BoundaryRules.Add(network.GetRule(GoSimplifiedAtnBuilder.RuleNames.PrimaryExpr));

            interpreter.ExcludedStartRules.Add(network.GetRule(GoSimplifiedAtnBuilder.RuleNames.Block));

            return interpreter;
        }

        private static IEnumerable<ITagSpan<IClassificationTag>> ClassifyTokens(ITextSnapshot snapshot, IEnumerable<IToken> tokens, IClassificationTag classificationTag)
        {
            foreach (var token in tokens)
            {
                SnapshotSpan span = new SnapshotSpan(snapshot, Span.FromBounds(token.StartIndex, token.StopIndex + 1));
                yield return new TagSpan<IClassificationTag>(span, classificationTag);
            }
        }

        private class TokenIndexEqualityComparer : IEqualityComparer<IToken>
        {
            private static readonly TokenIndexEqualityComparer _default = new TokenIndexEqualityComparer();

            public static TokenIndexEqualityComparer Default
            {
                get
                {
                    return _default;
                }
            }

            public bool Equals(IToken x, IToken y)
            {
                if (x == null)
                    return y == null;

                if (y == null)
                    return false;

                return x.TokenIndex == y.TokenIndex;
            }

            public int GetHashCode(IToken obj)
            {
                return obj.TokenIndex.GetHashCode();
            }
        }
    }
}
