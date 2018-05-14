namespace Tvl.VisualStudio.Language.Parsing.Experimental.Atn
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using JetBrains.Annotations;

    public class StateOptimizer
    {
        private readonly HashSet<State> _reachableStates;
        private readonly HashSet<Transition> _reachableTransitions;

        private readonly Dictionary<int, HashSet<PushContextTransition>> _pushContextTransitions = new Dictionary<int, HashSet<PushContextTransition>>();
        private readonly Dictionary<int, HashSet<PopContextTransition>> _popContextTransitions = new Dictionary<int, HashSet<PopContextTransition>>();

        private readonly HashSet<ulong> _nestedContexts = new HashSet<ulong>();
        private readonly HashSet<ulong> _contextStates = new HashSet<ulong>();

        public StateOptimizer([NotNull] IEnumerable<State> reachableStates)
        {
            Requires.NotNull(reachableStates, nameof(reachableStates));

            _reachableStates = new HashSet<State>(reachableStates, ObjectReferenceEqualityComparer<State>.Default);
            _reachableTransitions = new HashSet<Transition>(ObjectReferenceEqualityComparer<Transition>.Default);
            GetReachableTransitions(_reachableStates, _reachableTransitions);
            GetNestedContextsAndStates();
        }

        private void GetNestedContextsAndStates()
        {
            foreach (var outerTransition in _reachableTransitions.OfType<PushContextTransition>())
            {
                HashSet<State> visited = new HashSet<State>(ObjectReferenceEqualityComparer<State>.Default);
                foreach (var state in GetStatesInContext(outerTransition.TargetState, visited))
                {
                    ulong key = ((ulong)(ulong)outerTransition.ContextIdentifiers.Single() << 32) | (uint)state.Id;
                    _contextStates.Add(key);

                    foreach (var innerTransition in state.OutgoingTransitions.OfType<PushContextTransition>())
                    {
                        key = ((ulong)(ulong)outerTransition.ContextIdentifiers.Single() << 32) | (uint)innerTransition.ContextIdentifiers.Single();
                        _nestedContexts.Add(key);
                    }
                }
            }
        }

        private IEnumerable<State> GetStatesInContext([NotNull] State state, HashSet<State> visited)
        {
            Debug.Assert(state != null);

            if (!visited.Add(state))
                yield break;

            yield return state;

            foreach (var transition in state.OutgoingTransitions)
            {
                if (transition is PopContextTransition)
                    continue;

                if (transition is PushContextTransition)
                {
                    int context = ((ContextTransition)transition).ContextIdentifiers.Single();
                    foreach (var nextState in GetPopContextTransitions(context).SelectMany(i => GetStatesInContext(i.TargetState, visited)))
                        yield return nextState;

                    continue;
                }

                foreach (var nextState in GetStatesInContext(transition.TargetState, visited))
                    yield return nextState;
            }
        }

        public void Optimize([NotNull] IEnumerable<State> ruleStartStates)
        {
            Requires.NotNull(ruleStartStates, nameof(ruleStartStates));

            foreach (var state in _reachableStates)
                state.RemoveExtraEpsilonTransitions(this, ruleStartStates.Contains(state));

            foreach (var state in _reachableStates)
                state.AddRecursiveTransitions(this);

            Dictionary<int, Transition[]> optimizedTransitions = new Dictionary<int, Transition[]>();
            foreach (var state in _reachableStates)
            {
                Transition[] transitions = state.GetOptimizedTransitions().ToArray();
                optimizedTransitions.Add(state.Id, transitions);
            }

            foreach (var state in _reachableStates)
            {
                foreach (var transition in state.OutgoingTransitions.ToArray())
                    state.RemoveTransitionInternal(transition, this);

                foreach (var transition in optimizedTransitions[state.Id])
                    state.AddTransitionInternal(transition, this);

                state.IsOptimized = true;
            }
        }

        private void GetReachableTransitions([NotNull] IEnumerable<State> reachableStates, [NotNull] ISet<Transition> reachableTransitions)
        {
            Debug.Assert(reachableStates != null);
            Debug.Assert(reachableTransitions != null);

            foreach (var state in reachableStates)
            {
                foreach (var transition in state.OutgoingTransitions)
                    AddTransition(transition);
            }
        }

        private void AddReachableTransition([NotNull] Transition transition)
        {
            Debug.Assert(transition != null);

            if (!_reachableTransitions.Add(transition))
                throw new InvalidOperationException();
        }

        private void RemoveReachableTransition([NotNull] Transition transition)
        {
            Debug.Assert(transition != null);

            if (!_reachableTransitions.Remove(transition))
                throw new InvalidOperationException();
        }

        internal void AddTransition(Transition transition)
        {
            Requires.NotNull(transition, nameof(transition));

            AddReachableTransition(transition);

            PushContextTransition pushContextTransition = transition as PushContextTransition;
            if (pushContextTransition != null)
            {
                int context = pushContextTransition.ContextIdentifiers[0];
                HashSet<PushContextTransition> transitions;
                if (!_pushContextTransitions.TryGetValue(context, out transitions))
                {
                    transitions = new HashSet<PushContextTransition>(ObjectReferenceEqualityComparer<Transition>.Default);
                    _pushContextTransitions[context] = transitions;
                }

                transitions.Add(pushContextTransition);
            }

            PopContextTransition popContextTransition = transition as PopContextTransition;
            if (popContextTransition != null)
            {
                int context = popContextTransition.ContextIdentifiers.Last();
                HashSet<PopContextTransition> transitions;
                if (!_popContextTransitions.TryGetValue(context, out transitions))
                {
                    transitions = new HashSet<PopContextTransition>(ObjectReferenceEqualityComparer<Transition>.Default);
                    _popContextTransitions[context] = transitions;
                }

                transitions.Add(popContextTransition);
            }
        }

        internal void RemoveTransition([NotNull] Transition transition)
        {
            Requires.NotNull(transition, nameof(transition));

            RemoveReachableTransition(transition);

            PushContextTransition pushContextTransition = transition as PushContextTransition;
            if (pushContextTransition != null)
            {
                int context = pushContextTransition.ContextIdentifiers[0];
                HashSet<PushContextTransition> transitions = _pushContextTransitions[context];
                transitions.Remove(pushContextTransition);
            }

            PopContextTransition popContextTransition = transition as PopContextTransition;
            if (popContextTransition != null)
            {
                int context = popContextTransition.ContextIdentifiers.Last();
                HashSet<PopContextTransition> transitions = _popContextTransitions[context];
                transitions.Remove(popContextTransition);
            }
        }

        [NotNull]
        internal IEnumerable<PushContextTransition> GetPushContextTransitions([NotNull] PopContextTransition popContextTransition)
        {
            Requires.NotNull(popContextTransition, nameof(popContextTransition));
            return GetPushContextTransitions(popContextTransition.ContextIdentifiers.Last());
        }

        [NotNull]
        internal IEnumerable<PushContextTransition> GetPushContextTransitions(int context)
        {
            HashSet<PushContextTransition> transitions;
            if (!_pushContextTransitions.TryGetValue(context, out transitions))
                return Enumerable.Empty<PushContextTransition>();

            return transitions;
        }

        [NotNull]
        internal IEnumerable<PopContextTransition> GetPopContextTransitions([NotNull] PushContextTransition pushContextTransition)
        {
            Requires.NotNull(pushContextTransition, nameof(pushContextTransition));
            return GetPopContextTransitions(pushContextTransition.ContextIdentifiers[0]);
        }

        [NotNull]
        internal IEnumerable<PopContextTransition> GetPopContextTransitions(int context)
        {
            HashSet<PopContextTransition> transitions;
            if (!_popContextTransitions.TryGetValue(context, out transitions))
                return Enumerable.Empty<PopContextTransition>();

            return transitions;
        }

        public bool CanNestContexts(int outerContext, int innerContext)
        {
            ulong key = ((ulong)(ulong)outerContext << 32) | (uint)innerContext;
            return _nestedContexts.Contains(key);
        }

        public bool IsStateInContext(int context, int state)
        {
            ulong key = ((ulong)(ulong)context << 32) | (uint)state;
            return _contextStates.Contains(key);
        }
    }
}
