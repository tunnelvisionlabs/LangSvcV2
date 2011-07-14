namespace Tvl.VisualStudio.Language.Alloy
{
    using System.Collections.Generic;
    using IQuickInfoSession = Microsoft.VisualStudio.Language.Intellisense.IQuickInfoSession;
    using IQuickInfoSource = Microsoft.VisualStudio.Language.Intellisense.IQuickInfoSource;
    using ITextBuffer = Microsoft.VisualStudio.Text.ITextBuffer;
    using ITextSnapshot = Microsoft.VisualStudio.Text.ITextSnapshot;
    using ITrackingSpan = Microsoft.VisualStudio.Text.ITrackingSpan;
    using SnapshotPoint = Microsoft.VisualStudio.Text.SnapshotPoint;
    using Microsoft.VisualStudio.Text;
    using Tvl.VisualStudio.Language.Alloy.IntellisenseModel;
    using System;
    using Microsoft.VisualStudio;
    using Tvl.VisualStudio.Language.Parsing;
    using Antlr.Runtime;
    using Tvl.VisualStudio.Language.Parsing.Experimental;
    using Tvl.VisualStudio.Language.Alloy.Experimental;
    using System.Linq;
    using Tvl.VisualStudio.Shell.OutputWindow;
    using System.Diagnostics;

    internal class AlloyQuickInfoSource : IQuickInfoSource
    {
        private readonly ITextBuffer _textBuffer;
        private readonly AlloyQuickInfoSourceProvider _provider;

        public AlloyQuickInfoSource(ITextBuffer textBuffer, AlloyQuickInfoSourceProvider provider)
        {
            if (textBuffer == null)
                throw new ArgumentNullException("textBuffer");
            if (provider == null)
                throw new ArgumentNullException("provider");

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
                    break;

                default:
                    return;
                }

                Network network = AlloySimplifiedAtnBuilder.BuildNetwork();

                RuleBinding memberSelectRule = network.GetRule("BinOpExpr18");
                HashSet<Transition> memberSelectTransitions = new HashSet<Transition>();
                GetReachableTransitions(memberSelectRule, memberSelectTransitions);

                NetworkInterpreter interpreter = new NetworkInterpreter(network, tokens);
                interpreter.BoundaryStates.Add(memberSelectRule.StartState);
                interpreter.BoundaryStates.Add(network.GetRule("LetDecl").StartState);
                interpreter.BoundaryStates.Add(network.GetRule("NameList").StartState);
                interpreter.BoundaryStates.Add(network.GetRule("Ref").StartState);
                interpreter.BoundaryStates.Add(network.GetRule("Module").StartState);
                interpreter.BoundaryStates.Add(network.GetRule("Open").StartState);
                interpreter.BoundaryStates.Add(network.GetRule("FactDecl").StartState);
                interpreter.BoundaryStates.Add(network.GetRule("AssertDecl").StartState);
                //interpreter.BoundaryStates.Add(network.GetRule("FunDecl").StartState);
                interpreter.BoundaryStates.Add(network.GetRule("FunctionName").StartState);
                interpreter.BoundaryStates.Add(network.GetRule("CmdDecl").StartState);
                interpreter.BoundaryStates.Add(network.GetRule("Typescope").StartState);
                interpreter.BoundaryStates.Add(network.GetRule("EnumDecl").StartState);
                interpreter.BoundaryStates.Add(network.GetRule("ElseClause").StartState);

                interpreter.ExcludedStartRules.Add("CallArguments");

                while (interpreter.TryStepBackward())
                {
                    if (interpreter.Contexts.Count == 0)
                        break;

                    /* we want all traces to start outside the binOpExpr18 rule, which means all
                     * traces with a transition reachable from binOpExpr18 should contain a push
                     * transition with binOpExpr18's start state as its target.
                     */
                    if (interpreter.Contexts.All(context => IsBounded(context, interpreter)))
                    {
                        break;
                    }
                }

                IOutputWindowPane pane = Provider.OutputWindowService.TryGetPane(PredefinedOutputWindowPanes.TvlIntellisense);
                if (pane != null)
                {
                    pane.WriteLine(string.Format("Located {0} QuickInfo expression(s) in {1}ms.", interpreter.Contexts.Count, stopwatch.ElapsedMilliseconds));
                }

                HashSet<string> finalResult = new HashSet<string>();
                SnapshotSpan? contextSpan = null;
                foreach (var context in interpreter.Contexts)
                {
                    Span? span = null;
                    //List<string> results = AnalyzeInterpreterTrace(context, memberSelectRule, out span);

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

                    if (span.HasValue && !span.Value.IsEmpty)
                        contextSpan = new SnapshotSpan(currentSnapshot, span.Value);

                    //if (results.Count > 0)
                    //{
                    //    finalResult.UnionWith(results);
                    //    applicableToSpan = currentSnapshot.CreateTrackingSpan(span, SpanTrackingMode.EdgeExclusive);
                    //}
                }

                foreach (var result in finalResult)
                {
                    quickInfoContent.Add(result);
                }

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
                VirtualSnapshotSpan selection = new VirtualSnapshotSpan();
                if (contextSpan.HasValue)
                    selection = new VirtualSnapshotSpan(contextSpan.Value);

                if (!selection.IsEmpty && selection.Contains(new VirtualSnapshotPoint(triggerPoint.Value)))
                {
                    applicableToSpan = selection.Snapshot.CreateTrackingSpan(selection.SnapshotSpan, SpanTrackingMode.EdgeExclusive);

                    try
                    {
                        Expression currentExpression = Provider.IntellisenseCache.ParseExpression(selection);
                        if (currentExpression != null)
                        {
                            SnapshotSpan? span = currentExpression.Span;
                            if (span.HasValue)
                                applicableToSpan = span.Value.Snapshot.CreateTrackingSpan(span.Value, SpanTrackingMode.EdgeExclusive);

                            quickInfoContent.Add(currentExpression.ToString());
                        }
                        else
                        {
                            quickInfoContent.Add("Could not parse expression.");
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ErrorHandler.IsCriticalException(ex))
                            throw;

                        quickInfoContent.Add(ex.Message);
                    }
                }
            }
        }

        private List<string> AnalyzeInterpreterTrace(NetworkInterpreter.InterpretTrace context, RuleBinding memberSelectRule, out Span span)
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

        private static bool IsBounded(NetworkInterpreter.InterpretTrace trace, NetworkInterpreter interpreter)
        {
            return trace.Transitions.Count > 0 && trace.Interpreter.BoundaryStates.Contains(trace.Transitions.First.Value.Transition.SourceState);
        }

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
                if (contextTransition != null && memberSelectTransitions.Add(contextTransition.PopTransition))
                    GetReachableTransitions(contextTransition.PopTransition.TargetState, memberSelectTransitions);
            }
        }

        public void Dispose()
        {
        }
    }
}
