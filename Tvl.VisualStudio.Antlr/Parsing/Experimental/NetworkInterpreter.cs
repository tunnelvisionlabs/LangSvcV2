namespace Tvl.VisualStudio.Language.Parsing.Experimental
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using Antlr.Runtime;
    using System.Collections.ObjectModel;
    using System.Diagnostics;

    public class NetworkInterpreter
    {
        private readonly Network _network;
        private readonly ITokenStream _input;

        private readonly List<InterpretTrace> _contexts = new List<InterpretTrace>();

        private readonly HashSet<State> _boundaryStates = new HashSet<State>();
        private readonly HashSet<State> _forwardBoundaryStates = new HashSet<State>();
        private readonly HashSet<string> _excludedStartRules = new HashSet<string>();

        private int _lookBehindPosition = 0;
        private int _lookAheadPosition = 0;

        public NetworkInterpreter(Network network, ITokenStream input)
        {
            Contract.Requires<ArgumentNullException>(network != null, "network");
            Contract.Requires<ArgumentNullException>(input != null, "input");

            _network = network;
            _input = input;
        }

        public Network Network
        {
            get
            {
                return _network;
            }
        }

        public ITokenStream Input
        {
            get
            {
                return _input;
            }
        }

        public ReadOnlyCollection<InterpretTrace> Contexts
        {
            get
            {
                return _contexts.AsReadOnly();
            }
        }

        public ICollection<State> BoundaryStates
        {
            get
            {
                return _boundaryStates;
            }
        }

        public ICollection<string> ExcludedStartRules
        {
            get
            {
                return _excludedStartRules;
            }
        }

        public bool TryStepBackward()
        {
            if (_input.Index - _lookBehindPosition <= 0)
                return false;

            int symbol = _input.LA(-1 - _lookBehindPosition);
            int symbolPosition = _input.Index - _lookBehindPosition - 1;

            if (_lookAheadPosition == 0 && _lookBehindPosition == 0 && _contexts.Count == 0)
            {
                /* create our initial set of states as the ones at the target end of a match transition
                 * that contains 'symbol' in the match set.
                 */
                List<Transition> transitions = new List<Transition>(_network.Transitions.Where(i => i.IsMatch && i.MatchSet.Contains(symbol)));
                foreach (var transition in transitions)
                {
                    if (_excludedStartRules.Contains(Network.StateRules[transition.SourceState.Id]))
                        continue;

                    if (_excludedStartRules.Contains(Network.StateRules[transition.TargetState.Id]))
                        continue;

                    ContextFrame startContext = new ContextFrame(transition.TargetState, null, null, this);
                    ContextFrame endContext = new ContextFrame(transition.TargetState, null, null, this);
                    _contexts.Add(new InterpretTrace(startContext, endContext));
                }
            }

            List<InterpretTrace> existing = new List<InterpretTrace>(_contexts);
            _contexts.Clear();
            SortedSet<int> states = new SortedSet<int>();
            HashSet<InterpretTrace> contexts = new HashSet<InterpretTrace>();

            foreach (var context in existing)
            {
                states.Add(context.StartContext.State.Id);
                StepBackward(contexts, states, context, symbol, symbolPosition);
                states.Clear();
            }

            _contexts.AddRange(contexts);
            _lookBehindPosition++;
            return true;
        }

        public bool TryStepForward()
        {
            if (_input.Index + _lookAheadPosition >= _input.Count)
                return false;

            int symbol = _input.LA(1 + _lookAheadPosition);
            int symbolPosition = _input.Index + _lookAheadPosition;

            if (_lookAheadPosition == 0 && _lookBehindPosition == 0 && _contexts.Count == 0)
            {
                /* create our initial set of states as the ones at the source end of a match transition
                 * that contains 'symbol' in the match set.
                 */
                List<Transition> transitions = new List<Transition>(_network.Transitions.Where(i => i.IsMatch && i.MatchSet.Contains(symbol)));
                foreach (var transition in transitions)
                {
                    if (_excludedStartRules.Contains(Network.StateRules[transition.SourceState.Id]))
                        continue;

                    if (_excludedStartRules.Contains(Network.StateRules[transition.TargetState.Id]))
                        continue;

                    ContextFrame startContext = new ContextFrame(transition.SourceState, null, null, this);
                    ContextFrame endContext = new ContextFrame(transition.SourceState, null, null, this);
                    _contexts.Add(new InterpretTrace(startContext, endContext));
                }
            }

            List<InterpretTrace> existing = new List<InterpretTrace>(_contexts);
            _contexts.Clear();
            SortedSet<int> states = new SortedSet<int>();
            HashSet<InterpretTrace> contexts = new HashSet<InterpretTrace>();

            foreach (var context in existing)
            {
                if (context.Transitions.Count > 0 && _forwardBoundaryStates.Contains(context.Transitions.Last.Value.Transition.TargetState))
                {
                    contexts.Add(context);
                    continue;
                }

                states.Add(context.EndContext.State.Id);
                StepForward(contexts, states, context, symbol, symbolPosition);
                states.Clear();
            }

            _contexts.AddRange(contexts);
            _lookAheadPosition++;
            return true;
        }

        private void StepBackward(ICollection<InterpretTrace> result, ICollection<int> states, InterpretTrace context, int symbol, int symbolPosition)
        {
            //if (context.StartContext.State != null && _boundaryStates.Contains(context.StartContext.State))
            //{
            //    result.Add(context);
            //    return;
            //}

            foreach (var transition in context.StartContext.State.IncomingTransitions)
            {
                InterpretTrace step;
                if (context.TryStepBackward(transition, symbol, symbolPosition, out step))
                {
                    if (transition.IsMatch)
                    {
                        result.Add(step);
                        continue;
                    }

                    bool recursive = transition.SourceState.IsBackwardRecursive;
                    if (recursive && states.Contains(transition.SourceState.Id))
                    {
                        // TODO: check postfix rule
                        continue;
                    }

                    if (recursive)
                        states.Add(transition.SourceState.Id);

                    StepBackward(result, states, step, symbol, symbolPosition);

                    if (recursive)
                        states.Remove(transition.SourceState.Id);
                }
            }
        }

        private void StepForward(ICollection<InterpretTrace> result, ICollection<int> states, InterpretTrace context, int symbol, int symbolPosition)
        {
            //if (context.EndContext.State != null && _forwardBoundaryStates.Contains(context.EndContext.State))
            //{
            //    result.Add(context);
            //    return;
            //}

            foreach (var transition in context.EndContext.State.OutgoingTransitions)
            {
                InterpretTrace step;
                if (context.TryStepForward(transition, symbol, symbolPosition, out step))
                {
                    if (transition.IsMatch)
                    {
                        result.Add(step);
                        continue;
                    }

                    bool recursive = transition.TargetState.IsForwardRecursive;

                    if (recursive && states.Contains(transition.TargetState.Id))
                    {
                        // TODO: check postfix rule
                        continue;
                    }

                    if (recursive)
                        states.Add(transition.TargetState.Id);

                    StepForward(result, states, step, symbol, symbolPosition);

                    if (recursive)
                        states.Remove(transition.TargetState.Id);
                }
            }
        }

        public class ContextFrame : IEquatable<ContextFrame>
        {
            public readonly State State;
            public readonly int? Context;
            public readonly ContextFrame Parent;
            public readonly NetworkInterpreter Interpreter;

            public ContextFrame(State state, int? context, ContextFrame parent, NetworkInterpreter interpreter)
            {
                State = state;
                Context = context;
                Parent = parent;
                Interpreter = interpreter;
            }

            public Network Network
            {
                get
                {
                    return Interpreter.Network;
                }
            }

            internal ContextFrame AddHeadContext(ContextFrame context)
            {
                ContextFrame parent = (Parent != null) ? Parent.AddHeadContext(context) : context;
                return new ContextFrame(State, Context, parent, context.Interpreter);
            }

            public virtual bool Equals(ContextFrame other)
            {
                if (other == null)
                    return false;

                return EqualityComparer<State>.Default.Equals(State, other.State)
                    && Context == other.Context
                    && EqualityComparer<ContextFrame>.Default.Equals(Parent, other.Parent);
            }

            public sealed override bool Equals(object obj)
            {
                return Equals(obj as ContextFrame);
            }

            public override int GetHashCode()
            {
                int state = (State != null) ? EqualityComparer<State>.Default.GetHashCode(State) : 0;
                long parent = (Parent != null) ? Parent.GetHashCode() * 104729 : 0;
                return state ^ (Context ?? 0) ^ parent.GetHashCode();
            }

            public override string ToString()
            {
                string current = "?";
                if (State != null)
                    current = string.Format("{0}({1})", State.Id, Network.StateRules[State.Id]);

                List<string> parentContexts = new List<string>();
                for (ContextFrame frame = Parent; frame != null; frame = frame.Parent)
                {
                    string contextName = frame.Context != null ? frame.Context.ToString() : "<null>";

                    string parentRule;
                    if (frame.Context != null && Network.StateRules.TryGetValue(frame.Context.Value, out parentRule))
                        contextName = string.Format("{0}({1})", contextName, parentRule);

                    parentContexts.Add(contextName);
                }

                return string.Format("{0} : [{1}]", current, string.Join(",", parentContexts));
            }
        }

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

                bool boundedStart = BoundedStart;
                if (!boundedStart && Transitions.Count > 0 && Interpreter._boundaryStates.Contains(Transitions.First.Value.Transition.SourceState))
                {
                    bool nested = false;
                    for (ContextFrame parent = StartContext.Parent; parent != null; parent = parent.Parent)
                    {
                        if (parent.Context != null)
                        {
                            string contextRule = Network.StateRules[parent.Context.Value];
                            RuleBinding ruleBinding = Network.GetRule(contextRule);
                            if (Interpreter._boundaryStates.Contains(ruleBinding.StartState))
                                nested = true;
                        }
                    }

                    boundedStart = !nested;
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

                if (!transition.SourceState.GetSourceSet().Contains(symbol))
                    return false;

                if (transition.IsContext)
                {
                    PopContextTransition popContextTransition = transition as PopContextTransition;
                    if (popContextTransition != null)
                    {
                        ContextFrame subContext = new ContextFrame(popContextTransition.SourceState, popContextTransition.ContextIdentifier, this.StartContext, Interpreter);
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

                        if (startContext.Context != null)
                        {
                            // if the start context has a state stack, pop an item off it
                            if (!string.Equals(startContext.Context, pushContextTransition.ContextIdentifier))
                                return false;

                            startContext = new ContextFrame(transition.SourceState, startContext.Parent.Context, startContext.Parent.Parent, Interpreter);
                        }
                        else
                        {
                            // else we add a "predicate" to the end context
                            endContext = endContext.AddHeadContext(new ContextFrame(Network.States[pushContextTransition.ContextIdentifier], pushContextTransition.ContextIdentifier, null, Interpreter));
                            startContext = new ContextFrame(transition.SourceState, startContext.Context, startContext.Parent, Interpreter);
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

                    bool addTransition = !boundedStart && Interpreter._boundaryStates.Contains(transition.SourceState);
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

                bool boundedEnd = BoundedEnd;
                if (!boundedEnd && Transitions.Count > 0 && Interpreter._forwardBoundaryStates.Contains(Transitions.Last.Value.Transition.TargetState))
                    boundedEnd = true;

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

                if (!transition.TargetState.GetFollowSet().Contains(symbol))
                    return false;

                if (transition.IsContext)
                {
                    PushContextTransition pushContextTransition = transition as PushContextTransition;
                    if (pushContextTransition != null)
                    {
                        ContextFrame subContext = new ContextFrame(pushContextTransition.TargetState, pushContextTransition.ContextIdentifier, this.EndContext, Interpreter);
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

                        if (endContext.Context != null)
                        {
                            // if the end context has a state stack, pop an item off it
                            if (!string.Equals(endContext.Context, popContextTransition.ContextIdentifier))
                                return false;

                            endContext = new ContextFrame(transition.TargetState, endContext.Parent.Context, endContext.Parent.Parent, Interpreter);
                        }
                        else
                        {
                            // else we add a "predicate" to the start context
                            startContext = startContext.AddHeadContext(new ContextFrame(Network.States[popContextTransition.ContextIdentifier], popContextTransition.ContextIdentifier, null, Interpreter));
                            endContext = new ContextFrame(transition.TargetState, endContext.Context, endContext.Parent, Interpreter);
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
                    result = new InterpretTrace(this.StartContext, endContext, this.Transitions, this.BoundedStart, boundedEnd, boundedEnd);
                    if (!boundedEnd && Interpreter._forwardBoundaryStates.Contains(transition.TargetState))
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

        public class InterpretTraceTransition : IEquatable<InterpretTraceTransition>
        {
            public readonly Transition Transition;
            public readonly NetworkInterpreter Interpreter;
            public readonly int? Symbol;
            public readonly int? TokenIndex;

            public InterpretTraceTransition(Transition transition, NetworkInterpreter interpreter)
            {
                Contract.Requires<ArgumentNullException>(transition != null, "transition");
                Contract.Requires<ArgumentNullException>(interpreter != null, "interpreter");

                Transition = transition;
                Interpreter = interpreter;
            }

            public InterpretTraceTransition(Transition transition, int symbol, int symbolPosition, NetworkInterpreter interpreter)
                : this(transition, interpreter)
            {
                Contract.Requires(transition != null);
                Contract.Requires(interpreter != null);

                Symbol = symbol;
                TokenIndex = symbolPosition;
            }

            public IToken Token
            {
                get
                {
                    if (!TokenIndex.HasValue)
                        return null;

                    return Interpreter.Input.Get(TokenIndex.Value);
                }
            }

            public virtual bool Equals(InterpretTraceTransition other)
            {
                if (other == null)
                    return false;

                return TokenIndex == other.TokenIndex
                    && Transition.Equals(other.Transition);
            }

            public sealed override bool Equals(object obj)
            {
                return Equals(obj as InterpretTraceTransition);
            }

            public override int GetHashCode()
            {
                return Transition.GetHashCode() ^ TokenIndex.GetHashCode();
            }

            public override string ToString()
            {
                string sourceState = string.Format("{0}({1})", Transition.SourceState.Id, Interpreter.Network.StateRules[Transition.SourceState.Id]);
                string targetState = string.Format("{0}({1})", Transition.TargetState.Id, Interpreter.Network.StateRules[Transition.TargetState.Id]);

                string transition = "->";
                if (Transition.IsMatch)
                {
                    transition = string.Format("-> {0} ->", Symbol);
                }
                else if (Transition.IsContext)
                {
                    transition = string.Format("-> {0} {1} ->", (Transition is PushContextTransition) ? "push" : "pop", ((ContextTransition)Transition).ContextIdentifier);
                }

                return string.Format("{0} {1} {2}", sourceState, transition, targetState);
            }
        }
    }
}
