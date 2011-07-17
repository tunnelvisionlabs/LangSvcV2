namespace Tvl.VisualStudio.Language.Parsing.Experimental.Interpreter
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using Antlr.Runtime;
    using Tvl.VisualStudio.Language.Parsing.Experimental.Atn;
    using Stopwatch = System.Diagnostics.Stopwatch;

    public class NetworkInterpreter
    {
        private readonly Network _network;
        private readonly ITokenStream _input;

        private readonly List<InterpretTrace> _contexts = new List<InterpretTrace>();
#if DFA
        private DeterministicTrace _deterministicTrace;
#endif

        private readonly HashSet<RuleBinding> _boundaryRules = new HashSet<RuleBinding>();
        private readonly HashSet<State> _boundaryStates = new HashSet<State>();
        private readonly HashSet<State> _forwardBoundaryStates = new HashSet<State>();
        private readonly HashSet<RuleBinding> _excludedStartRules = new HashSet<RuleBinding>();

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

        public ICollection<RuleBinding> BoundaryRules
        {
            get
            {
                return _boundaryRules;
            }
        }

        public ICollection<State> BoundaryStates
        {
            get
            {
                return _boundaryStates;
            }
        }

        public ICollection<State> ForwardBoundaryStates
        {
            get
            {
                return _forwardBoundaryStates;
            }
        }

        public ICollection<RuleBinding> ExcludedStartRules
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

            /*
             * Update the non-deterministic trace
             */

            Stopwatch updateTimer = Stopwatch.StartNew();
            if (_lookAheadPosition == 0 && _lookBehindPosition == 0 && _contexts.Count == 0)
            {
                /* create our initial set of states as the ones at the target end of a match transition
                 * that contains 'symbol' in the match set.
                 */
                List<Transition> transitions = new List<Transition>(_network.Transitions.Where(i => i.IsMatch && i.MatchSet.Contains(symbol)));
                foreach (var transition in transitions)
                {
                    if (ExcludedStartRules.Contains(Network.StateRules[transition.SourceState.Id]))
                        continue;

                    if (ExcludedStartRules.Contains(Network.StateRules[transition.TargetState.Id]))
                        continue;

                    ContextFrame startContext = new ContextFrame(transition.TargetState, null, null, this);
                    ContextFrame endContext = new ContextFrame(transition.TargetState, null, null, this);
                    _contexts.Add(new InterpretTrace(startContext, endContext));
                }

#if DFA
                DeterministicState deterministicState = new DeterministicState(_contexts.Select(i => i.StartContext));
                _deterministicTrace = new DeterministicTrace(deterministicState, deterministicState);
#endif
            }

            List<InterpretTrace> existing = new List<InterpretTrace>(_contexts);
            _contexts.Clear();
            SortedSet<int> states = new SortedSet<int>();
            HashSet<InterpretTrace> contexts = new HashSet<InterpretTrace>();
            HashSet<ContextFrame> existingUnique = new HashSet<ContextFrame>(existing.Select(i => i.StartContext));

            foreach (var context in existing)
            {
                states.Add(context.StartContext.State.Id);
                StepBackward(contexts, states, context, symbol, symbolPosition, PreventContextType.None);
                states.Clear();
            }

            _contexts.AddRange(contexts);
            long nfaUpdateTime = updateTimer.ElapsedMilliseconds;

#if DFA
            /*
             * Update the deterministic trace
             */

            updateTimer.Restart();

            DeterministicTransition deterministicTransition = _deterministicTrace.StartState.IncomingTransitions.SingleOrDefault(i => i.MatchSet.Contains(symbol));
            if (deterministicTransition == null)
            {
                DeterministicState sourceState = new DeterministicState(contexts.Select(i => i.StartContext));
                DeterministicState targetState = _deterministicTrace.StartState;
                deterministicTransition = targetState.IncomingTransitions.SingleOrDefault(i => i.SourceState.Equals(sourceState));
                if (deterministicTransition == null)
                {
                    deterministicTransition = new DeterministicTransition(targetState);
                    sourceState.AddTransition(deterministicTransition);
                }

                deterministicTransition.MatchSet.Add(symbol);
            }

            IEnumerable<DeterministicTraceTransition> deterministicTransitions = Enumerable.Repeat(new DeterministicTraceTransition(deterministicTransition, symbol, symbolPosition, this), 1);
            deterministicTransitions = deterministicTransitions.Concat(_deterministicTrace.Transitions);
            _deterministicTrace = new DeterministicTrace(deterministicTransition.SourceState, _deterministicTrace.EndState, deterministicTransitions);

            long dfaUpdateTime = updateTimer.ElapsedMilliseconds;
#endif

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

        private void StepBackward(ICollection<InterpretTrace> result, ICollection<int> states, InterpretTrace context, int symbol, int symbolPosition, PreventContextType preventContextType)
        {
            //if (context.StartContext.State != null && _boundaryStates.Contains(context.StartContext.State))
            //{
            //    result.Add(context);
            //    return;
            //}

            foreach (var transition in context.StartContext.State.IncomingTransitions)
            {
                switch (preventContextType)
                {
                case PreventContextType.Pop:
                    if (transition is PopContextTransition)
                        continue;

                    break;

                case PreventContextType.PopNonRecursive:
                    if ((!transition.IsRecursive) && (transition is PopContextTransition))
                        continue;

                    break;

                case PreventContextType.Push:
                    if (transition is PushContextTransition)
                        continue;

                    break;

                case PreventContextType.PushNonRecursive:
                    if ((!transition.IsRecursive) && (transition is PushContextTransition))
                        continue;

                    break;

                default:
                    break;
                }

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

                    PreventContextType nextPreventContextType = PreventContextType.None;
                    if (context.StartContext.State.IsOptimized)
                    {
                        if (transition is PushContextTransition)
                            nextPreventContextType = PreventContextType.Push;
                        else if (transition is PopContextTransition)
                            nextPreventContextType = PreventContextType.Pop;

                        if (transition.IsRecursive)
                            nextPreventContextType++; // only block non-recursive transitions
                    }

                    StepBackward(result, states, step, symbol, symbolPosition, nextPreventContextType);

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
    }
}
