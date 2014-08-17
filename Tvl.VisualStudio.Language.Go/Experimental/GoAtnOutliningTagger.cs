namespace Tvl.VisualStudio.Language.Go.Experimental
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Antlr.Runtime;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;
    using Tvl.VisualStudio.Language.Parsing;
    using Tvl.VisualStudio.Language.Parsing.Experimental.Atn;
    using Tvl.VisualStudio.Language.Parsing.Experimental.Interpreter;
    using Tvl.VisualStudio.OutputWindow.Interfaces;

    internal sealed class GoAtnOutliningTagger : BackgroundParser, ITagger<IOutliningRegionTag>
    {
        private List<ITagSpan<IOutliningRegionTag>> _outliningRegions;

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public GoAtnOutliningTagger(ITextBuffer textBuffer, TaskScheduler taskScheduler, ITextDocumentFactoryService textDocumentFactoryService, IOutputWindowService outputWindowService)
            : base(textBuffer, taskScheduler, textDocumentFactoryService, outputWindowService)
        {
            RequestParse(false);
        }

        public override string Name
        {
            get
            {
                return "Outlining Tagger";
            }
        }

        public IEnumerable<ITagSpan<IOutliningRegionTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            return _outliningRegions ?? Enumerable.Empty<ITagSpan<IOutliningRegionTag>>();
        }

        protected override void ReParseImpl()
        {
            // lex the entire document to get the set of identifiers we'll need to classify
            ITextSnapshot snapshot = TextBuffer.CurrentSnapshot;
            var input = new SnapshotCharStream(snapshot, new Span(0, snapshot.Length));
            var lexer = new GoLexer(input);
            var tokenSource = new GoSemicolonInsertionTokenSource(lexer);
            var tokens = new CommonTokenStream(tokenSource);
            tokens.Fill();

            /* Want to outline the following blocks:
             *  - import
             *  - type
             *  - const
             *  - func
             */

            List<IToken> outliningKeywords = new List<IToken>();
            while (tokens.LA(1) != CharStreamConstants.EndOfFile)
            {
                switch (tokens.LA(1))
                {
                case GoLexer.KW_IMPORT:
                ////case GoLexer.KW_TYPE:
                case GoLexer.KW_CONST:
                case GoLexer.KW_STRUCT:
                case GoLexer.KW_FUNC:
                case GoLexer.KW_VAR:
                    outliningKeywords.Add(tokens.LT(1));
                    break;

                case CharStreamConstants.EndOfFile:
                    goto doneLexing;

                default:
                    break;
                }

                tokens.Consume();
            }

        doneLexing:

            List<ITagSpan<IOutliningRegionTag>> outliningRegions = new List<ITagSpan<IOutliningRegionTag>>();
            foreach (var token in outliningKeywords)
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

                while (interpreter.TryStepForward())
                {
                    if (interpreter.Contexts.Count == 0)
                        break;

                    if (interpreter.Contexts.All(context => context.BoundedEnd))
                        break;
                }

                interpreter.CombineBoundedEndContexts();

                foreach (var context in interpreter.Contexts)
                {
                    switch (token.Type)
                    {
                    case GoLexer.KW_IMPORT:
                    case GoLexer.KW_VAR:
                    case GoLexer.KW_CONST:
                        {
                            InterpretTraceTransition firstTransition = context.Transitions.Where(i => i.Transition.IsMatch).ElementAtOrDefault(1);
                            InterpretTraceTransition lastTransition = context.Transitions.LastOrDefault(i => i.Transition.IsMatch);
                            if (firstTransition == null || lastTransition == null)
                                continue;

                            if (firstTransition.Symbol != GoLexer.LPAREN)
                                continue;

                            var blockSpan = OutlineBlock(firstTransition.Token, lastTransition.Token, snapshot);
                            if (blockSpan != null)
                            {
                                outliningRegions.Add(blockSpan);
                                break;
                            }

                            break;
                        }

                    case GoLexer.KW_STRUCT:
                    case GoLexer.KW_FUNC:
                        {
                            InterpretTraceTransition firstTransition = context.Transitions.FirstOrDefault(i => i.Symbol == GoLexer.LBRACE);
                            InterpretTraceTransition lastTransition = context.Transitions.LastOrDefault(i => i.Transition.IsMatch);
                            if (firstTransition == null || lastTransition == null)
                                continue;

                            var blockSpan = OutlineBlock(firstTransition.Token, lastTransition.Token, snapshot);
                            if (blockSpan != null)
                            {
                                outliningRegions.Add(blockSpan);
                                break;
                            }
                            break;
                        }
                    }
                }
            }

            _outliningRegions = outliningRegions;
            OnTagsChanged(new SnapshotSpanEventArgs(new SnapshotSpan(snapshot, new Span(0, snapshot.Length))));
        }

        private ITagSpan<IOutliningRegionTag> OutlineBlock(IToken firstBrace, IToken lastBrace, ITextSnapshot snapshot)
        {
            Span span = Span.FromBounds(firstBrace.StartIndex, lastBrace.StopIndex + 1);
            if (snapshot.GetLineNumberFromPosition(span.Start) == snapshot.GetLineNumberFromPosition(span.End))
                return null;

            SnapshotSpan snapshotSpan = new SnapshotSpan(snapshot, span);
            IOutliningRegionTag tag = new OutliningRegionTag("...", "...");
            TagSpan<IOutliningRegionTag> tagSpan = new TagSpan<IOutliningRegionTag>(snapshotSpan, tag);
            return tagSpan;
        }

        private NetworkInterpreter CreateNetworkInterpreter(ITokenStream tokens)
        {
            Network network = NetworkBuilder<GoOutliningAtnBuilder>.GetOrBuildNetwork();

            NetworkInterpreter interpreter = new NetworkInterpreter(network, tokens);

            interpreter.BoundaryRules.Add(network.GetRule(GoOutliningAtnBuilder.RuleNames.ImportDecl));
            interpreter.BoundaryRules.Add(network.GetRule(GoOutliningAtnBuilder.RuleNames.TypeDecl));
            interpreter.BoundaryRules.Add(network.GetRule(GoOutliningAtnBuilder.RuleNames.ConstDecl));
            interpreter.BoundaryRules.Add(network.GetRule(GoOutliningAtnBuilder.RuleNames.FunctionDecl));
            interpreter.BoundaryRules.Add(network.GetRule(GoOutliningAtnBuilder.RuleNames.MethodDecl));
            interpreter.BoundaryRules.Add(network.GetRule(GoOutliningAtnBuilder.RuleNames.VarDecl));
            interpreter.BoundaryRules.Add(network.GetRule(GoOutliningAtnBuilder.RuleNames.StructType));

            interpreter.ExcludedStartRules.Add(network.GetRule(GoOutliningAtnBuilder.RuleNames.FunctionType));
            interpreter.ExcludedStartRules.Add(network.GetRule(GoOutliningAtnBuilder.RuleNames.TypeSwitchGuard));
            interpreter.ExcludedStartRules.Add(network.GetRule(GoOutliningAtnBuilder.RuleNames.Block));
            interpreter.ExcludedStartRules.Add(network.GetRule(GoOutliningAtnBuilder.RuleNames.Parameters));
            interpreter.ExcludedStartRules.Add(network.GetRule(GoOutliningAtnBuilder.RuleNames.Receiver));

            return interpreter;
        }

        private void OnTagsChanged(SnapshotSpanEventArgs e)
        {
            var t = TagsChanged;
            if (t != null)
                t(this, e);
        }
    }
}
