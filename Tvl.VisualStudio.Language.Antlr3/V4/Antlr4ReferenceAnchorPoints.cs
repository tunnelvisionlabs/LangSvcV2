namespace Tvl.VisualStudio.Language.AntlrV4
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using Antlr4.Runtime;
    using Antlr4.Runtime.Misc;
    using Antlr4.Runtime.Tree;
    using Microsoft.VisualStudio.Text;
    using Tvl.VisualStudio.Language.Parsing4;
    using ParseResultEventArgs = Tvl.VisualStudio.Language.Parsing.ParseResultEventArgs;

    internal class Antlr4ReferenceAnchorPoints
    {
        private static ConditionalWeakTable<ITextSnapshot, IList<IAnchor>> _referenceAnchors =
            new ConditionalWeakTable<ITextSnapshot, IList<IAnchor>>();

        private readonly Antlr4ReferenceAnchorPointsProvider _provider;
        private readonly ITextBuffer _textBuffer;

        private readonly Antlr4BackgroundParser _backgroundParser;

        private Tuple<ITextSnapshot, IList<IAnchor>> _newestResult;

        public Antlr4ReferenceAnchorPoints(Antlr4ReferenceAnchorPointsProvider provider, ITextBuffer textBuffer)
        {
            _provider = provider;
            _textBuffer = textBuffer;

            _backgroundParser = (Antlr4BackgroundParser)provider.BackgroundParserFactoryService.GetBackgroundParser(textBuffer);
            _backgroundParser.ParseComplete += HandleParseComplete;
        }

        internal IList<IAnchor> GetValue(ITextSnapshot snapshot, ParserDataOptions options)
        {
            if (_textBuffer != snapshot.TextBuffer)
                throw new ArgumentException();

            // reference anchors never change
            IList<IAnchor> result;
            if (_referenceAnchors.TryGetValue(snapshot, out result))
                return result;

            if ((options & ParserDataOptions.AllowStale) != ParserDataOptions.None)
            {
                var newestResult = _newestResult;
                if (newestResult != null)
                    return newestResult.Item2;
            }

            if ((options & ParserDataOptions.NoUpdate) != ParserDataOptions.None)
                return null;

            // calculate the results...
            result = _referenceAnchors.GetValue(snapshot, key => CreateReferenceAnchorPoints(key, null));
            return result;
        }

        [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_grammarSpec, 0, Dependents.Self)]
        private IList<IAnchor> CreateReferenceAnchorPoints(ITextSnapshot snapshot, GrammarParser.GrammarSpecContext parseResult)
        {
            if (parseResult == null)
            {
                AntlrParseResultEventArgs resultEventArgs = Antlr4BackgroundParser.ParseSnapshot(snapshot);
                parseResult = (GrammarParser.GrammarSpecContext)resultEventArgs.Result;
            }

            AnchorListener listener = new AnchorListener(snapshot);
            ParseTreeWalker.Default.Walk(listener, parseResult);
            return listener.Anchors;
        }

        private void HandleParseComplete(object sender, ParseResultEventArgs e)
        {
            AntlrParseResultEventArgs antlrEventArgs = (AntlrParseResultEventArgs)e;
            IList<IAnchor> result = _referenceAnchors.GetValue(e.Snapshot, snapshot => CreateReferenceAnchorPoints(snapshot, (GrammarParser.GrammarSpecContext)antlrEventArgs.Result));
            _newestResult = Tuple.Create(e.Snapshot, result);
        }

        private class AnchorListener : GrammarParserBaseListener
        {
            private readonly ITextSnapshot _snapshot;
            private readonly Stack<int> _anchorPositions = new Stack<int>();
            private readonly List<IAnchor> _anchors = new List<IAnchor>();

            public AnchorListener(ITextSnapshot snapshot)
            {
                _snapshot = snapshot;
            }

            public IList<IAnchor> Anchors
            {
                get
                {
                    return _anchors;
                }
            }

            [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_grammarType, 0, Dependents.Self | Dependents.Ancestors)]
            public override void EnterGrammarType(GrammarParser.GrammarTypeContext context)
            {
                EnterAnchor(context);
            }

            [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_grammarType, 0, Dependents.Self | Dependents.Ancestors)]
            public override void ExitGrammarType(GrammarParser.GrammarTypeContext context)
            {
                ExitAnchor(context, GrammarParser.RULE_grammarType);
            }

            [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_ruleSpec, 3, Dependents.Self | Dependents.Ancestors)]
            public override void EnterRuleSpec(GrammarParser.RuleSpecContext context)
            {
                EnterAnchor(context);
            }

            [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_ruleSpec, 3, Dependents.Self | Dependents.Ancestors)]
            public override void ExitRuleSpec(GrammarParser.RuleSpecContext context)
            {
                ExitAnchor(context, GrammarParser.RULE_ruleSpec);
            }

            private void EnterAnchor(ParserRuleContext context)
            {
                _anchorPositions.Push(context.Start.StartIndex);
            }

            private void ExitAnchor(ParserRuleContext context, int anchorId)
            {
                int start = _anchorPositions.Pop();
                Interval sourceInterval = ParseTrees.GetSourceInterval(context);
                int stop = sourceInterval.b + 1;
                SpanTrackingMode trackingMode = context.Stop != null ? SpanTrackingMode.EdgeExclusive : SpanTrackingMode.EdgePositive;
                if (stop >= start)
                {
                    _anchors.Add(CreateAnchor(context, start, stop, trackingMode, anchorId));
                }
            }

            [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_grammarType, 0, Dependents.Self)]
            private Anchor CreateAnchor(ParserRuleContext context, int start, int stop, SpanTrackingMode trackingMode, int rule)
            {
                ITrackingSpan trackingSpan = _snapshot.CreateTrackingSpan(start, stop - start, trackingMode);
                if (rule == GrammarParser.RULE_grammarType)
                {
                    return new GrammarTypeAnchor((GrammarParser.GrammarTypeContext)context, trackingSpan);
                }
                else
                {
                    return new GrammarAnchor(rule, trackingSpan);
                }
            }

            public class GrammarAnchor : Anchor
            {
                public GrammarAnchor(int ruleIndex, ITrackingSpan trackingSpan)
                    : base(ruleIndex, trackingSpan)
                {
                }

                protected IList<string> RuleNames
                {
                    get
                    {
                        return GrammarParser.ruleNames;
                    }
                }
            }

            public class GrammarTypeAnchor : GrammarAnchor
            {
                private readonly int grammarType;

                [RuleDependency(typeof(GrammarParser), GrammarParser.RULE_grammarType, 0, Dependents.Self)]
                public GrammarTypeAnchor(GrammarParser.GrammarTypeContext context, ITrackingSpan span)
                    : base(GrammarParser.RULE_grammarType, span)
                {
                    if (context.LEXER() != null)
                    {
                        grammarType = GrammarParser.LEXER;
                    }
                    else if (context.PARSER() != null)
                    {
                        grammarType = GrammarParser.PARSER;
                    }
                    else
                    {
                        grammarType = GrammarParser.COMBINED;
                    }
                }

                public int GrammarType
                {
                    get
                    {
                        return grammarType;
                    }
                }
            }
        }
    }
}
