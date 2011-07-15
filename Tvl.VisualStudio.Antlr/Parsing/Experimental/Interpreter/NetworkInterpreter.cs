namespace Tvl.VisualStudio.Language.Parsing.Experimental.Interpreter
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using Antlr.Runtime;
    using Tvl.VisualStudio.Language.Parsing.Experimental.Atn;

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

        public ICollection<State> ForwardBoundaryStates
        {
            get
            {
                return _forwardBoundaryStates;
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
    }
}
