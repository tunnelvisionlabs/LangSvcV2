namespace Tvl.VisualStudio.Language.Alloy
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Antlr.Runtime;
    using JetBrains.Annotations;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Text;
    using Tvl.VisualStudio.Language.Parsing;
    using Tvl.VisualStudio.Language.Parsing.Experimental.Atn;
    using Tvl.VisualStudio.Language.Parsing.Experimental.Interpreter;
    using Tvl.VisualStudio.OutputWindow.Interfaces;
    using AlloySimplifiedAtnBuilder = Tvl.VisualStudio.Language.Alloy.Experimental.AlloySimplifiedAtnBuilder;
    using Expression = Tvl.VisualStudio.Language.Alloy.IntellisenseModel.Expression;
    using IQuickInfoSession = Microsoft.VisualStudio.Language.Intellisense.IQuickInfoSession;
    using IQuickInfoSource = Microsoft.VisualStudio.Language.Intellisense.IQuickInfoSource;
    using ITextBuffer = Microsoft.VisualStudio.Text.ITextBuffer;
    using ITextSnapshot = Microsoft.VisualStudio.Text.ITextSnapshot;
    using ITrackingSpan = Microsoft.VisualStudio.Text.ITrackingSpan;
    using SnapshotPoint = Microsoft.VisualStudio.Text.SnapshotPoint;

    internal class AlloyQuickInfoSource : IQuickInfoSource
    {
        private readonly ITextBuffer _textBuffer;
        private readonly AlloyQuickInfoSourceProvider _provider;

        public AlloyQuickInfoSource([NotNull] ITextBuffer textBuffer, [NotNull] AlloyQuickInfoSourceProvider provider)
        {
            Requires.NotNull(textBuffer, nameof(textBuffer));
            Requires.NotNull(provider, nameof(provider));

            _textBuffer = textBuffer;
            _provider = provider;
        }

        public ITextBuffer TextBuffer
        {
            get
            {
                return _textBuffer;
            }
        }

        public AlloyQuickInfoSourceProvider Provider
        {
            get
            {
                return _provider;
            }
        }

        public void AugmentQuickInfoSession(IQuickInfoSession session, IList<object> quickInfoContent, out ITrackingSpan applicableToSpan)
        {
            applicableToSpan = null;
            if (session == null || quickInfoContent == null)
                return;

            if (session.TextView.TextBuffer == this.TextBuffer)
            {
                ITextSnapshot currentSnapshot = this.TextBuffer.CurrentSnapshot;
                SnapshotPoint? triggerPoint = session.GetTriggerPoint(currentSnapshot);
                if (!triggerPoint.HasValue)
                    return;

                #region experimental
                /* use the experimental model to locate and process the expression */
                Stopwatch stopwatch = Stopwatch.StartNew();

                // lex the entire document
                var input = new SnapshotCharStream(currentSnapshot, new Span(0, currentSnapshot.Length));
                var lexer = new AlloyLexer(input);
                var tokens = new CommonTokenStream(lexer);
                tokens.Fill();

                // locate the last token before the trigger point
                while (true)
                {
                    IToken nextToken = tokens.LT(1);
                    if (nextToken.Type == CharStreamConstants.EndOfFile)
                        break;

                    if (nextToken.StartIndex > triggerPoint.Value.Position)
                        break;

                    tokens.Consume();
                }

                switch (tokens.LA(-1))
                {
                case AlloyLexer.IDENTIFIER:
                case AlloyLexer.KW_THIS:
                case AlloyLexer.KW_UNIV:
                case AlloyLexer.KW_IDEN:
                case AlloyLexer.KW_INT2:
                case AlloyLexer.KW_SEQINT:
                case AlloyLexer.INTEGER:
                    break;

                default:
                    return;
                }

                Network network = NetworkBuilder<AlloySimplifiedAtnBuilder>.GetOrBuildNetwork();

                RuleBinding memberSelectRule = network.GetRule(AlloySimplifiedAtnBuilder.RuleNames.BinOpExpr18);
#if DEBUG && false
                HashSet<Transition> memberSelectTransitions = new HashSet<Transition>(ObjectReferenceEqualityComparer<Transition>.Default);
                GetReachableTransitions(memberSelectRule, memberSelectTransitions);
#endif

                NetworkInterpreter interpreter = new NetworkInterpreter(network, tokens);

                interpreter.BoundaryRules.Add(memberSelectRule);
                //interpreter.BoundaryRules.Add(network.GetRule(AlloySimplifiedAtnBuilder.RuleNames.UnaryExpression));
                interpreter.BoundaryRules.Add(network.GetRule(AlloySimplifiedAtnBuilder.RuleNames.LetDecl));
                interpreter.BoundaryRules.Add(network.GetRule(AlloySimplifiedAtnBuilder.RuleNames.NameListName));
                interpreter.BoundaryRules.Add(network.GetRule(AlloySimplifiedAtnBuilder.RuleNames.Ref));
                interpreter.BoundaryRules.Add(network.GetRule(AlloySimplifiedAtnBuilder.RuleNames.Module));
                interpreter.BoundaryRules.Add(network.GetRule(AlloySimplifiedAtnBuilder.RuleNames.Open));
                interpreter.BoundaryRules.Add(network.GetRule(AlloySimplifiedAtnBuilder.RuleNames.FactDecl));
                interpreter.BoundaryRules.Add(network.GetRule(AlloySimplifiedAtnBuilder.RuleNames.AssertDecl));
                interpreter.BoundaryRules.Add(network.GetRule(AlloySimplifiedAtnBuilder.RuleNames.FunctionName));
                interpreter.BoundaryRules.Add(network.GetRule(AlloySimplifiedAtnBuilder.RuleNames.CmdDecl));
                interpreter.BoundaryRules.Add(network.GetRule(AlloySimplifiedAtnBuilder.RuleNames.Typescope));
                interpreter.BoundaryRules.Add(network.GetRule(AlloySimplifiedAtnBuilder.RuleNames.EnumDecl));
                interpreter.BoundaryRules.Add(network.GetRule(AlloySimplifiedAtnBuilder.RuleNames.ElseClause));

                interpreter.ExcludedStartRules.Add(network.GetRule(AlloySimplifiedAtnBuilder.RuleNames.CallArguments));

                while (interpreter.TryStepBackward())
                {
                    if (interpreter.Contexts.Count == 0)
                        break;

                    /* we want all traces to start outside the binOpExpr18 rule, which means all
                     * traces with a transition reachable from binOpExpr18 should contain a push
                     * transition with binOpExpr18's start state as its target.
                     */
                    if (interpreter.Contexts.All(context => context.BoundedStart))
                    {
                        break;
                    }
                }

                HashSet<InterpretTrace> contexts = new HashSet<InterpretTrace>(BoundedStartInterpretTraceEqualityComparer.Default);
                if (interpreter.Contexts.Count > 0)
                {
                    contexts.UnionWith(interpreter.Contexts);
                }
                else
                {
                    contexts.UnionWith(interpreter.BoundedStartContexts);
                }

                IOutputWindowPane pane = Provider.OutputWindowService.TryGetPane(PredefinedOutputWindowPanes.TvlIntellisense);
                if (pane != null)
                {
                    pane.WriteLine(string.Format("Located {0} QuickInfo expression(s) in {1}ms.", contexts.Count, stopwatch.ElapsedMilliseconds));
                }

                HashSet<Span> spans = new HashSet<Span>();
                foreach (var context in contexts)
                {
                    Span? span = null;
                    foreach (var transition in context.Transitions)
                    {
                        if (!transition.Transition.IsMatch)
                            continue;

                        IToken token = transition.Token;
                        Span tokenSpan = new Span(token.StartIndex, token.StopIndex - token.StartIndex + 1);
                        if (span == null)
                            span = tokenSpan;
                        else
                            span = Span.FromBounds(Math.Min(span.Value.Start, tokenSpan.Start), Math.Max(span.Value.End, tokenSpan.End));
                    }

                    if (span.HasValue)
                        spans.Add(span.Value);
                }

                //List<Expression> expressions = new List<Expression>();
                //HashSet<string> finalResult = new HashSet<string>();
                //SnapshotSpan? contextSpan = null;

                bool foundInfo = false;
                foreach (var span in spans)
                {
                    if (!span.IsEmpty)
                    {
                        VirtualSnapshotSpan selection = new VirtualSnapshotSpan(new SnapshotSpan(currentSnapshot, span));
                        if (!selection.IsEmpty && selection.Contains(new VirtualSnapshotPoint(triggerPoint.Value)))
                        {
                            try
                            {
                                Expression currentExpression = Provider.IntellisenseCache.ParseExpression(selection);
                                if (currentExpression != null && currentExpression.Span.HasValue && currentExpression.Span.Value.Contains(triggerPoint.Value))
                                {
                                    applicableToSpan = currentExpression.Span.Value.Snapshot.CreateTrackingSpan(currentExpression.Span.Value, SpanTrackingMode.EdgeExclusive);
                                    quickInfoContent.Add(currentExpression.ToString());
                                    foundInfo = true;
                                }
                            }
                            catch (Exception ex) when (!ErrorHandler.IsCriticalException(ex))
                            {
                                quickInfoContent.Add(ex.Message);
                            }
                        }

                        //try
                        //{
                        //    SnapshotSpan contextSpan = new SnapshotSpan(currentSnapshot, span.Value);
                        //    Expression expression = Provider.IntellisenseCache.ParseExpression(contextSpan);
                        //    if (expression != null)
                        //        expressions.Add(expression);
                        //}
                        //catch (Exception e) when (!ErrorHandler.IsCriticalException(e))
                        //{
                        //}
                    }

                    //if (results.Count > 0)
                    //{
                    //    finalResult.UnionWith(results);
                    //    applicableToSpan = currentSnapshot.CreateTrackingSpan(span, SpanTrackingMode.EdgeExclusive);
                    //}
                }

                if (!foundInfo && spans.Count > 0)
                {
                    foreach (var span in spans)
                    {
                        if (!span.IsEmpty)
                        {
                            VirtualSnapshotSpan selection = new VirtualSnapshotSpan(new SnapshotSpan(currentSnapshot, span));
                            if (!selection.IsEmpty && selection.Contains(new VirtualSnapshotPoint(triggerPoint.Value)))
                            {
                                applicableToSpan = selection.Snapshot.CreateTrackingSpan(selection.SnapshotSpan, SpanTrackingMode.EdgeExclusive);
                                break;
                            }
                        }
                    }

                    quickInfoContent.Add("Could not parse expression.");
                }

                //foreach (var result in finalResult)
                //{
                //    quickInfoContent.Add(result);
                //}

                #endregion

#if false
                var selection = session.TextView.Selection.StreamSelectionSpan;
                if (selection.IsEmpty || !selection.Contains(new VirtualSnapshotPoint(triggerPoint.Value)))
                {
                    SnapshotSpan? expressionSpan = Provider.IntellisenseCache.GetExpressionSpan(triggerPoint.Value);
                    if (expressionSpan.HasValue)
                        selection = new VirtualSnapshotSpan(expressionSpan.Value);
                }
#endif
                //VirtualSnapshotSpan selection = new VirtualSnapshotSpan();
                //if (contextSpan.HasValue)
                //    selection = new VirtualSnapshotSpan(contextSpan.Value);

                //if (!selection.IsEmpty && selection.Contains(new VirtualSnapshotPoint(triggerPoint.Value)))
                //{
                //    applicableToSpan = selection.Snapshot.CreateTrackingSpan(selection.SnapshotSpan, SpanTrackingMode.EdgeExclusive);

                //    try
                //    {
                //        Expression currentExpression = Provider.IntellisenseCache.ParseExpression(selection);
                //        if (currentExpression != null)
                //        {
                //            SnapshotSpan? span = currentExpression.Span;
                //            if (span.HasValue)
                //                applicableToSpan = span.Value.Snapshot.CreateTrackingSpan(span.Value, SpanTrackingMode.EdgeExclusive);

                //            quickInfoContent.Add(currentExpression.ToString());
                //        }
                //        else
                //        {
                //            quickInfoContent.Add("Could not parse expression.");
                //        }
                //    }
                //    catch (Exception ex) when (!ErrorHandler.IsCriticalException(ex))
                //    {
                //        quickInfoContent.Add(ex.Message);
                //    }
                //}
            }
        }

#if false
        private List<string> AnalyzeInterpreterTrace(InterpretTrace context, RuleBinding memberSelectRule, out Span span)
        {
            var transitions = context.Transitions;
            int expressionLevel = 0;
            List<string> results = new List<string>();

            string identifier = null;
            span = new Span();

            foreach (var transition in transitions)
            {
                if (transition.Transition.IsMatch)
                {
                    switch (transition.Symbol.Value)
                    {
                    case AlloyLexer.IDENTIFIER:
                    case AlloyLexer.KW_THIS:
                    case AlloyLexer.KW_UNIV:
                    case AlloyLexer.KW_IDEN:
                    case AlloyLexer.KW_INT2:
                    case AlloyLexer.KW_SEQINT:
                    case AlloyLexer.INTEGER:
                        if (expressionLevel == 0)
                        {
                            IToken token = transition.Token;
                            identifier = token.Text;
                            span = new Span(token.StartIndex, token.StopIndex - token.StartIndex + 1);
                        }

                        break;

                    case AlloyLexer.DOT:
                        if (expressionLevel == 0)
                        {
                            for (int i = 0; i < results.Count; i++)
                            {
                                results[i] += identifier + ".";
                                identifier = null;
                            }
                        }

                        break;
                    }
                }
                else if (transition.Transition.IsContext)
                {
                    // only need to handle it if we're entering BinOpExpr18
                    PushContextTransition pushContextTransition = transition.Transition as PushContextTransition;
                    if (pushContextTransition != null && pushContextTransition.TargetState == memberSelectRule.StartState)
                    {
                        expressionLevel++;
                    }

                    PopContextTransition popContextTransition = transition.Transition as PopContextTransition;
                    if (popContextTransition != null && popContextTransition.SourceState == memberSelectRule.EndState)
                    {
                        if (expressionLevel > 0)
                            expressionLevel--;
                    }
                }
                else if (transition.Transition.IsEpsilon)
                {
                    if (expressionLevel == 0 && transition.Transition.SourceState == memberSelectRule.StartState)
                    {
                        results.Clear();
                        results.Add("<context>::");
                    }
                }
            }

            if (identifier != null)
            {
                for (int i = 0; i < results.Count; i++)
                {
                    results[i] += "[" + identifier + "]";
                }
            }

            return results;
        }
#endif

#if DEBUG && false
        private static void GetReachableTransitions(RuleBinding memberSelectRule, HashSet<Transition> memberSelectTransitions)
        {
            GetReachableTransitions(memberSelectRule.StartState, memberSelectTransitions);
        }

        private static void GetReachableTransitions(State state, HashSet<Transition> memberSelectTransitions)
        {
            foreach (var transition in state.OutgoingTransitions)
            {
                if (transition is PopContextTransition)
                    continue;

                if (memberSelectTransitions.Add(transition))
                    GetReachableTransitions(transition.TargetState, memberSelectTransitions);

                PushContextTransition contextTransition = transition as PushContextTransition;
                if (contextTransition != null)
                {
                    foreach (var popTransition in contextTransition.PopTransitions)
                    {
                        if (memberSelectTransitions.Add(popTransition))
                            GetReachableTransitions(popTransition.TargetState, memberSelectTransitions);
                    }
                }
            }
        }
#endif

        public void Dispose()
        {
        }
    }
}
