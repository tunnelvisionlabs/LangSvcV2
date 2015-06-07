namespace Tvl.VisualStudio.Language.AntlrV4
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Antlr4.Runtime;
    using Antlr4.Runtime.Atn;
    using Antlr4.Runtime.Misc;

    public abstract class ForestParser<T> : IForestParser<T>
        where T : ICodeCompletionParser
    {
        public virtual IDictionary<RuleContext, CaretReachedException> GetParseTrees(T parser)
        {
            List<MultipleDecisionData> potentialAlternatives = new List<MultipleDecisionData>();
            List<int> currentPath = new List<int>();
            Dictionary<RuleContext, CaretReachedException> results = new Dictionary<RuleContext, CaretReachedException>();

            // make sure the token stream is initialized before getting the index
            parser.InputStream.La(1);
            int initialToken = parser.InputStream.Index;
            while (true)
            {
                parser.InputStream.Seek(initialToken);
                TryParse(parser, potentialAlternatives, currentPath, results);
                if (!IncrementCurrentPath(potentialAlternatives, currentPath))
                    break;
            }

            Debug.WriteLine(string.Format("Forest parser constructed {0} parse trees.", results.Count));
            return results;
        }

        protected virtual bool IncrementCurrentPath(List<MultipleDecisionData> potentialAlternatives, List<int> currentPath)
        {
            for (int i = currentPath.Count - 1; i >= 0; i--)
            {
                if (currentPath[i] < potentialAlternatives[i].Alternatives.Length - 1)
                {
                    currentPath[i]++;
                    return true;
                }

                potentialAlternatives.RemoveAt(i);
                currentPath.RemoveAt(i);
            }

            return false;
        }

        protected virtual void TryParse(T parser, List<MultipleDecisionData> potentialAlternatives, List<int> currentPath, IDictionary<RuleContext, CaretReachedException> results)
        {
            RuleContext parseTree;
            try
            {
                parser.Interpreter.SetFixedDecisions(potentialAlternatives, currentPath);
                parseTree = ParseImpl(parser);
                results[parseTree] = null;
            }
            catch (CaretReachedException ex)
            {
                if (ex.Transitions == null)
                    return;

                if (ex.InnerException is FailedPredicateException)
                    return;

                for (parseTree = ex.FinalContext; parseTree.Parent != null; parseTree = parseTree.Parent)
                {
                    // intentionally blank
                }

                if (ex.InnerException != null)
                {
                    IntervalSet alts = new IntervalSet();
                    IntervalSet semanticAlts = new IntervalSet();
                    foreach (ATNConfig c in ex.Transitions.Keys)
                    {
                        if (semanticAlts.Contains(c.Alt))
                            continue;

                        alts.Add(c.Alt);

                        var recognizer = parser as Recognizer<IToken, ParserATNSimulator>;
                        if (recognizer == null || c.SemanticContext.Eval(recognizer, ex.FinalContext))
                            semanticAlts.Add(c.Alt);
                    }

                    if (alts.Count != semanticAlts.Count)
                    {
                        Console.WriteLine("Forest decision {0} reduced to {1} by predicate evaluation.", alts, semanticAlts);
                    }

                    int inputIndex = parser.InputStream.Index;
                    int decision = 0;

                    int stateNumber = ex.InnerException.OffendingState;
                    ATNState state = parser.Atn.states[stateNumber];
                    if (state is StarLoopbackState)
                    {
                        Debug.Assert(state.NumberOfTransitions == 1 && state.OnlyHasEpsilonTransitions);
                        Debug.Assert(state.Transition(0).target is StarLoopEntryState);
                        state = state.Transition(0).target;
                    }
                    else
                    {
                        PlusBlockStartState plusBlockStartState = state as PlusBlockStartState;
                        if (plusBlockStartState != null && plusBlockStartState.decision == -1)
                        {
                            state = plusBlockStartState.loopBackState;
                            Debug.Assert(state != null);
                        }
                    }

                    DecisionState decisionState = state as DecisionState;
                    if (decisionState != null)
                    {
                        decision = decisionState.decision;
                        if (decision < 0)
                            Debug.WriteLine(string.Format("No decision number found for state {0}.", state.stateNumber));
                    }
                    else
                    {
                        if (state != null)
                            Debug.WriteLine(string.Format("No decision number found for state {0}.", state.stateNumber));
                        else
                            Debug.WriteLine("No decision number found for state <null>.");

                        // continuing is likely to terminate
                        return;
                    }

                    Debug.Assert(semanticAlts.MinElement >= 1);
                    Debug.Assert(semanticAlts.MaxElement <= parser.Atn.decisionToState[decision].NumberOfTransitions);
                    int[] alternatives = semanticAlts.ToArray();

                    MultipleDecisionData decisionData = new MultipleDecisionData(inputIndex, decision, alternatives);
                    potentialAlternatives.Add(decisionData);
                    currentPath.Add(-1);
                }
                else
                {
                    results[parseTree] = ex;
                }
            }
            catch (RecognitionException ex)
            {
                // not a viable path
            }
        }

        protected abstract RuleContext ParseImpl(T parser);
    }
}
