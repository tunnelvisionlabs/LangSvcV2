namespace Tvl.VisualStudio.Language.Parsing.Experimental.Interpreter
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using Tvl.VisualStudio.Language.Parsing.Experimental.Atn;

    [DebuggerDisplay("Transition Count: {Transitions.Count}")]
    public class InterpretTrace : IEquatable<InterpretTrace>
    {
        private static readonly LinkedList<InterpretTraceTransition> EmptyTransitions = new LinkedList<InterpretTraceTransition>();

        public readonly ContextFrame StartContext;
        public readonly LinkedList<InterpretTraceTransition> Transitions = new LinkedList<InterpretTraceTransition>();
        public readonly ContextFrame EndContext;
        public readonly bool BoundedStart;
        public readonly bool BoundedEnd;

        public InterpretTrace(ContextFrame startContext, ContextFrame endContext)
            : this(startContext, endContext, EmptyTransitions, false, false, false)
        {
        }

        private InterpretTrace(ContextFrame startContext, ContextFrame endContext, LinkedList<InterpretTraceTransition> transitions, bool boundedStart, bool boundedEnd, bool currentBounded)
        {
            StartContext = startContext;
            EndContext = endContext;
            if (currentBounded)
                Transitions = transitions;
            else
                Transitions = new LinkedList<InterpretTraceTransition>(transitions);
            BoundedStart = boundedStart;
            BoundedEnd = boundedEnd;
        }

        public Network Network
        {
            get
            {
                return StartContext.Network;
            }
        }

        public NetworkInterpreter Interpreter
        {
            get
            {
                return StartContext.Interpreter;
            }
        }

        public bool TryStepBackward(Transition transition, int symbol, int symbolPosition, out InterpretTrace result)
        {
            Contract.Requires<ArgumentNullException>(transition != null, "transition");

            Contract.Assert(Network.States.ContainsKey(transition.SourceState.Id), "Attempted to step outside the network.");
            //Contract.Assert(Network.States.ContainsKey(transition.TargetState.Id));

            bool boundedStart = BoundedStart;
            if (!boundedStart && Transitions.Count > 0)
            {
                bool stateBoundary = Interpreter.BoundaryStates.Contains(Transitions.First.Value.Transition.SourceState);
                bool ruleBoundary = false;
                PushContextTransition pushContextTransition = Transitions.First.Value.Transition as PushContextTransition;
                if (pushContextTransition != null)
                {
                    /* the rule boundary transfers from outside the rule to inside the rule (or
                     * inside a rule invoked by this rule)
                     */

                    // first, check if the transition goes to this rule
                    ruleBoundary = Interpreter.BoundaryRules.Contains(Network.StateRules[pushContextTransition.TargetState.Id]);

                    // next, check if the transition starts outside this rule and goes through this rule
                    if (!ruleBoundary && !Interpreter.BoundaryRules.Contains(Network.ContextRules[pushContextTransition.ContextIdentifiers[0]]))
                    {
                        ruleBoundary =
                            pushContextTransition.ContextIdentifiers
                            .Skip(1)
                            .Any(i => Interpreter.BoundaryRules.Contains(Network.ContextRules[i]));
                    }
                }

                if (stateBoundary || ruleBoundary)
                {
                    bool nested = false;
                    for (ContextFrame parent = StartContext.Parent; parent != null; parent = parent.Parent)
                    {
                        Contract.Assert(parent.Context != null);

                        RuleBinding contextRule = Network.ContextRules[parent.Context.Value];
                        if (Interpreter.BoundaryRules.Contains(contextRule))
                        {
                            nested = true;
                        }
                        else
                        {
                            if (Interpreter.BoundaryStates.Contains(contextRule.StartState))
                                nested = true;
                        }
                    }

                    boundedStart = !nested;
                }
            }

            result = null;

            if (transition.IsMatch)
            {
                if (!transition.MatchSet.Contains(symbol))
                    return false;

                ContextFrame startContext = new ContextFrame(transition.SourceState, this.StartContext.Context, this.StartContext.Parent, Interpreter);
                result = new InterpretTrace(startContext, this.EndContext, this.Transitions, boundedStart, this.BoundedEnd, boundedStart);
                if (!boundedStart)
                    result.Transitions.AddFirst(new InterpretTraceTransition(transition, symbol, symbolPosition, Interpreter));

                return true;
            }

            PreventContextType preventContextType = PreventContextType.None;
            if (transition.IsContext)
            {
                if (transition is PushContextTransition)
                    preventContextType = PreventContextType.Push;
                else if (transition is PopContextTransition)
                    preventContextType = PreventContextType.Pop;

                if (transition.IsRecursive)
                    preventContextType++; // only block non-recursive transitions
            }

            if (!transition.SourceState.GetSourceSet(preventContextType).Contains(symbol))
                return false;

            if (transition.IsContext)
            {
                PopContextTransition popContextTransition = transition as PopContextTransition;
                if (popContextTransition != null)
                {
                    ContextFrame subContext = this.StartContext;
                    foreach (var label in popContextTransition.ContextIdentifiers.Reverse())
                        subContext = new ContextFrame(popContextTransition.SourceState, null, new ContextFrame(subContext.State, label, subContext.Parent, Interpreter), Interpreter);

                    result = new InterpretTrace(subContext, this.EndContext, this.Transitions, boundedStart, this.BoundedEnd, boundedStart);
                    if (!boundedStart)
                        result.Transitions.AddFirst(new InterpretTraceTransition(transition, Interpreter));

                    return true;
                }

                PushContextTransition pushContextTransition = transition as PushContextTransition;
                if (pushContextTransition != null)
                {
                    ContextFrame startContext = this.StartContext;
                    ContextFrame endContext = this.EndContext;

                    foreach (var label in pushContextTransition.ContextIdentifiers.Reverse())
                    {
                        if (startContext.Parent != null)
                        {
                            Contract.Assert(startContext.Parent.Context.HasValue);

                            // if the start context has a state stack, pop an item off it
                            if (startContext.Parent.Context != label)
                                return false;

                            startContext = new ContextFrame(transition.SourceState, null, startContext.Parent.Parent, Interpreter);
                        }
                        else
                        {
                            // else we add a "predicate" to the end context
                            endContext = endContext.AddHeadContext(new ContextFrame(null, label, null, Interpreter));
                            startContext = new ContextFrame(transition.SourceState, startContext.Context, startContext.Parent, Interpreter);
                        }
                    }

                    result = new InterpretTrace(startContext, endContext, this.Transitions, boundedStart, this.BoundedEnd, boundedStart);
                    if (!boundedStart)
                        result.Transitions.AddFirst(new InterpretTraceTransition(transition, Interpreter));

                    return true;
                }

                throw new NotSupportedException("Unknown context transition.");
            }
            else if (transition.IsEpsilon)
            {
                ContextFrame startContext = new ContextFrame(transition.SourceState, this.StartContext.Context, this.StartContext.Parent, Interpreter);

                bool addTransition = !boundedStart && Interpreter.BoundaryStates.Contains(transition.SourceState);
                result = new InterpretTrace(startContext, this.EndContext, this.Transitions, boundedStart, this.BoundedEnd, !addTransition);

                if (addTransition)
                    result.Transitions.AddFirst(new InterpretTraceTransition(transition, Interpreter));

                return true;
            }

            throw new NotSupportedException("Unknown transition type.");
        }

        public bool TryStepForward(Transition transition, int symbol, int symbolPosition, out InterpretTrace result)
        {
            Contract.Requires<ArgumentNullException>(transition != null, "transition");

            Contract.Assert(Network.States.ContainsKey(transition.TargetState.Id), "Attempted to step outside the network.");
            //Contract.Assert(Network.States.ContainsKey(transition.TargetState.Id));

            bool boundedEnd = BoundedEnd;
            if (!boundedEnd && Transitions.Count > 0)
            {
                bool stateBoundary = Interpreter.BoundaryStates.Contains(Transitions.Last.Value.Transition.TargetState);
                bool ruleBoundary = false;
                PopContextTransition popContextTransition = Transitions.Last.Value.Transition as PopContextTransition;
                if (popContextTransition != null)
                {
                    /* the rule boundary transfers from outside the rule to inside the rule (or
                     * inside a rule invoked by this rule)
                     */

                    // first, check if the transition goes from this rule
                    ruleBoundary = Interpreter.BoundaryRules.Contains(Network.StateRules[popContextTransition.SourceState.Id]);

                    // next, check if the transition starts outside this rule and goes through this rule
                    if (!ruleBoundary && !Interpreter.BoundaryRules.Contains(Network.ContextRules[popContextTransition.ContextIdentifiers.Last()]))
                    {
                        ruleBoundary =
                            popContextTransition.ContextIdentifiers
                            .Take(popContextTransition.ContextIdentifiers.Count - 1)
                            .Any(i => Interpreter.BoundaryRules.Contains(Network.ContextRules[i]));
                    }
                }

                if (stateBoundary || ruleBoundary)
                {
                    bool nested = false;
                    for (ContextFrame parent = EndContext.Parent; parent != null; parent = parent.Parent)
                    {
                        Contract.Assert(parent.Context != null);

                        RuleBinding contextRule = Network.ContextRules[parent.Context.Value];
                        if (Interpreter.BoundaryRules.Contains(contextRule))
                        {
                            nested = true;
                        }
                        else
                        {
                            if (Interpreter.BoundaryStates.Contains(contextRule.StartState))
                                nested = true;
                        }
                    }

                    boundedEnd = !nested;
                }
            }

            result = null;

            if (transition.IsMatch)
            {
                if (!transition.MatchSet.Contains(symbol))
                    return false;

                ContextFrame endContext = new ContextFrame(transition.TargetState, this.EndContext.Context, this.EndContext.Parent, Interpreter);
                result = new InterpretTrace(this.StartContext, endContext, this.Transitions, this.BoundedStart, boundedEnd, boundedEnd);
                if (!boundedEnd)
                    result.Transitions.AddLast(new InterpretTraceTransition(transition, symbol, symbolPosition, Interpreter));

                return true;
            }

            PreventContextType preventContextType = PreventContextType.None;
            if (transition.IsContext)
            {
                if (transition is PushContextTransition)
                    preventContextType = PreventContextType.Push;
                else if (transition is PopContextTransition)
                    preventContextType = PreventContextType.Pop;

                if (transition.IsRecursive)
                    preventContextType++; // only block non-recursive transitions
            }

            if (!transition.TargetState.GetFollowSet(preventContextType).Contains(symbol))
                return false;

            if (transition.IsContext)
            {
                PushContextTransition pushContextTransition = transition as PushContextTransition;
                if (pushContextTransition != null)
                {
                    ContextFrame subContext = this.EndContext;
                    foreach (var label in pushContextTransition.ContextIdentifiers)
                        subContext = new ContextFrame(pushContextTransition.TargetState, null, new ContextFrame(subContext.State, label, subContext.Parent, Interpreter), Interpreter);

                    result = new InterpretTrace(this.StartContext, subContext, this.Transitions, this.BoundedStart, boundedEnd, boundedEnd);
                    if (!boundedEnd)
                        result.Transitions.AddLast(new InterpretTraceTransition(transition, Interpreter));

                    return true;
                }

                PopContextTransition popContextTransition = transition as PopContextTransition;
                if (popContextTransition != null)
                {
                    ContextFrame startContext = this.StartContext;
                    ContextFrame endContext = this.EndContext;

                    foreach (var label in popContextTransition.ContextIdentifiers)
                    {
                        if (endContext.Parent != null)
                        {
                            Contract.Assert(endContext.Parent.Context.HasValue);

                            // if the end context has a state stack, pop an item off it
                            if (endContext.Parent.Context != label)
                                return false;

                            endContext = new ContextFrame(transition.TargetState, null, endContext.Parent.Parent, Interpreter);
                        }
                        else
                        {
                            // else we add a "predicate" to the start context
                            startContext = startContext.AddHeadContext(new ContextFrame(null, label, null, Interpreter));
                            endContext = new ContextFrame(transition.TargetState, endContext.Context, endContext.Parent, Interpreter);
                        }
                    }

                    result = new InterpretTrace(startContext, endContext, this.Transitions, this.BoundedStart, boundedEnd, boundedEnd);
                    if (!boundedEnd)
                        result.Transitions.AddLast(new InterpretTraceTransition(transition, Interpreter));

                    return true;
                }

                throw new NotSupportedException("Unknown context transition.");
            }
            else if (transition.IsEpsilon)
            {
                ContextFrame endContext = new ContextFrame(transition.TargetState, this.EndContext.Context, this.EndContext.Parent, Interpreter);

                bool addTransition = !boundedEnd && Interpreter.BoundaryStates.Contains(transition.TargetState);
                result = new InterpretTrace(this.StartContext, endContext, this.Transitions, this.BoundedStart, boundedEnd, !addTransition);

                if (addTransition)
                    result.Transitions.AddLast(new InterpretTraceTransition(transition, Interpreter));

                return true;
            }

            throw new NotSupportedException("Unknown transition type.");
        }

        public virtual bool Equals(InterpretTrace other)
        {
            if (other == null)
                return false;

            return StartContext.Equals(other.StartContext)
                && EndContext.Equals(other.EndContext)
                && Transitions.SequenceEqual(other.Transitions);
        }

        public sealed override bool Equals(object obj)
        {
            return Equals(obj as InterpretTrace);
        }

        public override int GetHashCode()
        {
            return StartContext.GetHashCode() ^ EndContext.GetHashCode() /*^ Transitions.Aggregate(0, (i, j) => i ^ j.GetHashCode())*/;
        }
    }
}
