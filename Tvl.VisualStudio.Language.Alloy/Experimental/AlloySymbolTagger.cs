namespace Tvl.VisualStudio.Language.Alloy.Experimental
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

    internal sealed class AlloySymbolTagger : BackgroundParser, ITagger<IClassificationTag>
    {
        private readonly IClassificationTypeRegistryService _classificationTypeRegistryService;

        private List<ITagSpan<IClassificationTag>> _tags = new List<ITagSpan<IClassificationTag>>();

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public AlloySymbolTagger(ITextBuffer textBuffer, IClassificationTypeRegistryService classificationTypeRegistryService, TaskScheduler taskScheduler, ITextDocumentFactoryService textDocumentFactoryService, IOutputWindowService outputWindowService)
            : base(textBuffer, taskScheduler, textDocumentFactoryService, outputWindowService)
        {
            _classificationTypeRegistryService = classificationTypeRegistryService;
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
            // lex the entire document to get the set of identifiers we'll need to classify
            ITextSnapshot snapshot = TextBuffer.CurrentSnapshot;
            var input = new SnapshotCharStream(snapshot, new Span(0, snapshot.Length));
            var lexer = new AlloyLexer(input);
            var tokens = new CommonTokenStream(lexer);
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
                case AlloyLexer.IDENTIFIER:
                    identifiers.Add(tokens.LT(1));
                    break;

                case AlloyLexer.KW_MODULE:
                case AlloyLexer.KW_OPEN:
                case AlloyLexer.KW_AS:
                case AlloyLexer.KW_ENUM:
                case AlloyLexer.KW_FACT:
                case AlloyLexer.KW_ASSERT:
                case AlloyLexer.KW_RUN:
                case AlloyLexer.KW_CHECK:
                case AlloyLexer.KW_EXTENDS:

                case AlloyLexer.KW_FUN:
                case AlloyLexer.KW_PRED:

                case AlloyLexer.KW_SIG:
                    nameKeywords.Add(tokens.LT(1));
                    break;

                case AlloyLexer.COLON:
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
                NetworkInterpreter interpreter = CreateNetworkInterpreter(tokens);
                while (interpreter.TryStepForward())
                {
                    if (interpreter.Contexts.Count == 0)
                        break;

                    if (interpreter.Contexts.All(context => context.BoundedEnd))
                        break;
                }

                foreach (var context in interpreter.Contexts)
                {
                    foreach (var transition in context.Transitions)
                    {
                        if (!transition.Symbol.HasValue)
                            continue;

                        switch (transition.Symbol)
                        {
                        case AlloyLexer.IDENTIFIER:
                        case AlloyLexer.KW_THIS:
                            RuleBinding rule = interpreter.Network.StateRules[transition.Transition.TargetState.Id];
                            if (rule.Name != AlloySimplifiedAtnBuilder.RuleNames.NameDefinition)
                                references.Add(tokens.Get(transition.TokenIndex.Value));
                            if (rule.Name != AlloySimplifiedAtnBuilder.RuleNames.NameReference)
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

                NetworkInterpreter interpreter = CreateNetworkInterpreter(tokens);
                while (interpreter.TryStepBackward())
                {
                    if (interpreter.Contexts.Count == 0)
                        break;

                    if (interpreter.Contexts.All(context => context.BoundedStart))
                        break;
                }

                while (interpreter.TryStepForward())
                {
                    if (interpreter.Contexts.Count == 0)
                        break;

                    if (interpreter.Contexts.All(context => context.BoundedEnd))
                        break;
                }

                foreach (var context in interpreter.Contexts)
                {
                    foreach (var transition in context.Transitions)
                    {
                        if (!transition.Symbol.HasValue)
                            continue;

                        switch (transition.Symbol)
                        {
                        case AlloyLexer.IDENTIFIER:
                        case AlloyLexer.KW_THIS:
                            RuleBinding rule = interpreter.Network.StateRules[transition.Transition.TargetState.Id];
                            if (rule.Name != AlloySimplifiedAtnBuilder.RuleNames.NameDefinition)
                                references.Add(tokens.Get(transition.TokenIndex.Value));
                            if (rule.Name != AlloySimplifiedAtnBuilder.RuleNames.NameReference)
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

                NetworkInterpreter interpreter = CreateNetworkInterpreter(tokens);
                while (interpreter.TryStepBackward())
                {
                    if (interpreter.Contexts.Count == 0)
                        break;

                    if (interpreter.Contexts.All(context => context.BoundedStart))
                        break;
                }

                while (interpreter.TryStepForward())
                {
                    if (interpreter.Contexts.Count == 0)
                        break;

                    if (interpreter.Contexts.All(context => context.BoundedEnd))
                        break;
                }

                foreach (var context in interpreter.Contexts)
                {
                    foreach (var transition in context.Transitions)
                    {
                        if (!transition.Symbol.HasValue)
                            continue;

                        switch (transition.Symbol)
                        {
                        case AlloyLexer.IDENTIFIER:
                        case AlloyLexer.KW_THIS:
                            RuleBinding rule = interpreter.Network.StateRules[transition.Transition.TargetState.Id];
                            if (rule.Name != AlloySimplifiedAtnBuilder.RuleNames.NameDefinition)
                                references.Add(tokens.Get(transition.TokenIndex.Value));
                            if (rule.Name != AlloySimplifiedAtnBuilder.RuleNames.NameReference)
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

            IClassificationType definitionClassificationType = _classificationTypeRegistryService.GetClassificationType(AlloySymbolTaggerClassificationTypeNames.Definition);
            tags.AddRange(ClassifyTokens(snapshot, definitions, new ClassificationTag(definitionClassificationType)));

            IClassificationType referenceClassificationType = _classificationTypeRegistryService.GetClassificationType(AlloySymbolTaggerClassificationTypeNames.Reference);
            tags.AddRange(ClassifyTokens(snapshot, references, new ClassificationTag(referenceClassificationType)));

            IClassificationType unknownClassificationType = _classificationTypeRegistryService.GetClassificationType(AlloySymbolTaggerClassificationTypeNames.UnknownIdentifier);
            tags.AddRange(ClassifyTokens(snapshot, unknownIdentifiers, new ClassificationTag(unknownClassificationType)));

            _tags = tags;

            OnTagsChanged(new SnapshotSpanEventArgs(new SnapshotSpan(snapshot, new Span(0, snapshot.Length))));
        }

        private NetworkInterpreter CreateNetworkInterpreter(ITokenStream tokens)
        {
            Network network = AlloySimplifiedAtnBuilder.BuildNetwork();

            NetworkInterpreter interpreter = new NetworkInterpreter(network, tokens);

            RuleBinding memberSelectRule = network.GetRule(AlloySimplifiedAtnBuilder.RuleNames.BinOpExpr18);
            interpreter.BoundaryRules.Add(memberSelectRule);

            //interpreter.BoundaryRules.Add(network.GetRule(AlloySimplifiedAtnBuilder.RuleNames.LetDecl));
            //interpreter.BoundaryRules.Add(network.GetRule(AlloySimplifiedAtnBuilder.RuleNames.QuantDecls));
            //interpreter.BoundaryRules.Add(network.GetRule(AlloySimplifiedAtnBuilder.RuleNames.Decl));
            ////interpreter.BoundaryRules.Add(network.GetRule(AlloySimplifiedAtnBuilder.RuleNames.NameList));
            ////interpreter.BoundaryRules.Add(network.GetRule(AlloySimplifiedAtnBuilder.RuleNames.NameListName));
            //interpreter.BoundaryRules.Add(network.GetRule(AlloySimplifiedAtnBuilder.RuleNames.Ref));
            //interpreter.BoundaryRules.Add(network.GetRule(AlloySimplifiedAtnBuilder.RuleNames.Open));
            //interpreter.BoundaryRules.Add(network.GetRule(AlloySimplifiedAtnBuilder.RuleNames.FactDecl));
            //interpreter.BoundaryRules.Add(network.GetRule(AlloySimplifiedAtnBuilder.RuleNames.AssertDecl));
            ////interpreter.BoundaryRules.Add(network.GetRule(AlloySimplifiedAtnBuilder.RuleNames.FunDecl));
            ////interpreter.BoundaryRules.Add(network.GetRule(AlloySimplifiedAtnBuilder.RuleNames.FunctionName));
            //interpreter.BoundaryRules.Add(network.GetRule(AlloySimplifiedAtnBuilder.RuleNames.CmdDecl));
            //interpreter.BoundaryRules.Add(network.GetRule(AlloySimplifiedAtnBuilder.RuleNames.Typescope));
            //interpreter.BoundaryRules.Add(network.GetRule(AlloySimplifiedAtnBuilder.RuleNames.EnumDecl));
            //interpreter.BoundaryRules.Add(network.GetRule(AlloySimplifiedAtnBuilder.RuleNames.ElseClause));
            //interpreter.BoundaryRules.Add(network.GetRule(AlloySimplifiedAtnBuilder.RuleNames.Module));

            interpreter.BoundaryRules.Add(network.GetRule(AlloySimplifiedAtnBuilder.RuleNames.LetDecl));
            interpreter.BoundaryRules.Add(network.GetRule(AlloySimplifiedAtnBuilder.RuleNames.QuantDecls));
            interpreter.BoundaryRules.Add(network.GetRule(AlloySimplifiedAtnBuilder.RuleNames.Decl));

            /* adding this rule definitely didn't help! */
            //interpreter.BoundaryRules.Add(network.GetRule(AlloySimplifiedAtnBuilder.RuleNames.Expr));

            interpreter.BoundaryRules.Add(network.GetRule(AlloySimplifiedAtnBuilder.RuleNames.Module));
            interpreter.BoundaryRules.Add(network.GetRule(AlloySimplifiedAtnBuilder.RuleNames.FactDeclHeader));
            interpreter.BoundaryRules.Add(network.GetRule(AlloySimplifiedAtnBuilder.RuleNames.AssertDeclHeader));
            interpreter.BoundaryRules.Add(network.GetRule(AlloySimplifiedAtnBuilder.RuleNames.FunFunctionName));
            interpreter.BoundaryRules.Add(network.GetRule(AlloySimplifiedAtnBuilder.RuleNames.PredFunctionName));
            interpreter.BoundaryRules.Add(network.GetRule(AlloySimplifiedAtnBuilder.RuleNames.FunctionReturn));
            interpreter.BoundaryRules.Add(network.GetRule(AlloySimplifiedAtnBuilder.RuleNames.SigDeclHeader));
            interpreter.BoundaryRules.Add(network.GetRule(AlloySimplifiedAtnBuilder.RuleNames.SigExt));

            //interpreter.ExcludedStartRules.Add(network.GetRule(AlloySimplifiedAtnBuilder.RuleNames.CallArguments));

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
