namespace Tvl.VisualStudio.Language.Parsing.Experimental.Atn
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using IntervalSet = Tvl.VisualStudio.Language.Parsing.Collections.IntervalSet;
    using PreventContextType = Tvl.VisualStudio.Language.Parsing.Experimental.Interpreter.PreventContextType;

    public class State
    {
        private static int _nextState = 0;

        private readonly List<Transition> _outgoingTransitions = new List<Transition>();
        private readonly List<Transition> _incomingTransitions = new List<Transition>();
        private int _id = _nextState++;

        private IntervalSet[] _sourceSet;
        private IntervalSet[] _followSet;
        private bool? _isForwardRecursive;
        private bool? _isBackwardRecursive;
        private bool _isOptimized;

        public int Id
        {
            get
            {
                return _id;
            }

            set
            {
                _id = value;
            }
        }

        public List<Transition> OutgoingTransitions
        {
            get
            {
                return _outgoingTransitions;
            }
        }

        public List<Transition> IncomingTransitions
        {
            get
            {
                return _incomingTransitions;
            }
        }

        public bool IsOptimized
        {
            get
            {
                return _isOptimized;
            }
        }

        public bool IsBackwardRecursive
        {
            get
            {
                if (_isBackwardRecursive == null)
                {
                    _isBackwardRecursive = false;

                    HashSet<Transition> visited = new HashSet<Transition>();
                    Queue<Transition> queue = new Queue<Transition>(_incomingTransitions);

                    while (queue.Count > 0)
                    {
                        Transition transition = queue.Dequeue();
                        if (!visited.Add(transition))
                            continue;

                        if (transition.IsEpsilon || transition.IsContext)
                        {
                            if (transition.SourceState == this)
                            {
                                _isBackwardRecursive = true;
                                break;
                            }

                            foreach (var incoming in transition.SourceState.IncomingTransitions)
                                queue.Enqueue(incoming);
                        }
                    }
                }

                return _isBackwardRecursive.Value;
            }
        }

        public bool IsForwardRecursive
        {
            get
            {
                if (_isForwardRecursive == null)
                {
                    _isForwardRecursive = false;

                    HashSet<Transition> visited = new HashSet<Transition>(ObjectReferenceEqualityComparer<Transition>.Default);
                    Queue<Transition> queue = new Queue<Transition>(_outgoingTransitions);

                    while (queue.Count > 0)
                    {
                        Transition transition = queue.Dequeue();
                        if (!visited.Add(transition))
                            continue;

                        if (transition.IsEpsilon || transition.IsContext)
                        {
                            if (transition.TargetState == this)
                            {
                                _isForwardRecursive = true;
                                break;
                            }

                            foreach (var outgoing in transition.TargetState.OutgoingTransitions)
                                queue.Enqueue(outgoing);
                        }
                    }
                }

                return _isForwardRecursive.Value;
            }
        }

        public void Optimize()
        {
            if (IsOptimized)
                return;

            OptimizeOutgoingTransitions();
            _isOptimized = true;
        }

        private void OptimizeOutgoingTransitions()
        {
            List<Transition> oldTransitions = new List<Transition>(OutgoingTransitions);
            foreach (var transition in oldTransitions)
                RemoveTransition(transition);

            if (OutgoingTransitions.Count != 0)
                throw new InvalidOperationException();

            foreach (var transition in oldTransitions)
            {
                HashSet<State> visited = new HashSet<State>(ObjectReferenceEqualityComparer<State>.Default);
                visited.Add(this);

                AddOptimizedTransitions(visited, transition, PreventContextType.None);
            }

            Contract.Assert(oldTransitions.Count == 0 || OutgoingTransitions.Count > 0);
        }

        private void AddOptimizedTransitions(HashSet<State> visited, Transition transition, PreventContextType preventContextType)
        {
            Contract.Requires(visited != null);
            Contract.Requires(transition != null);
            Contract.Requires(transition.SourceState == null);

            try
            {
                if (transition.IsMatch || transition.TargetState.OutgoingTransitions.Count == 0 || !visited.Add(transition.TargetState))
                {
                    if (!transition.IsMatch && transition.TargetState.OutgoingTransitions.Count > 0)
                    {
                        // must be here because it's a recursive state
                        transition.IsRecursive = true;
                    }

                    AddTransitionInternal(transition);
                    return;
                }

                bool added = false;

                foreach (var nextTransition in transition.TargetState.OutgoingTransitions.ToArray())
                {
                    bool preventMerge = false;

                    switch (preventContextType)
                    {
                    case PreventContextType.Pop:
                        if (transition is PopContextTransition)
                            preventMerge = true;

                        break;

                    case PreventContextType.PopNonRecursive:
                        if ((!transition.IsRecursive) && (transition is PopContextTransition))
                            preventMerge = true;

                        break;

                    case PreventContextType.Push:
                        if (transition is PushContextTransition)
                            preventMerge = true;

                        break;

                    case PreventContextType.PushNonRecursive:
                        if ((!transition.IsRecursive) && (transition is PushContextTransition))
                            preventMerge = true;

                        break;

                    default:
                        break;
                    }

                    if (transition.IsEpsilon)
                    {
                        Contract.Assert(!preventMerge);
                        AddOptimizedTransitions(visited, MergeTransitions(transition, nextTransition), preventContextType);
                    }
                    else if (transition.IsContext)
                    {
                        PreventContextType nextPreventContextType = PreventContextType.None;
                        if (!preventMerge && transition.TargetState.IsOptimized)
                        {
                            if (nextTransition is PushContextTransition)
                                nextPreventContextType = PreventContextType.Push;
                            else if (nextTransition is PopContextTransition)
                                nextPreventContextType = PreventContextType.Pop;

                            if (nextTransition.IsRecursive)
                                nextPreventContextType++; // only block non-recursive transitions
                        }

                        bool canMerge = !preventMerge && !nextTransition.IsMatch;

                        if (canMerge && nextTransition.IsContext)
                        {
                            canMerge = nextTransition.GetType() == transition.GetType();
                        }

                        if (canMerge)
                        {
                            AddOptimizedTransitions(visited, MergeTransitions(transition, nextTransition), nextPreventContextType);
                        }
                        else if (!added)
                        {
                            AddTransitionInternal(transition);
                            added = true;
                        }
                    }
                }
            }
            finally
            {
                visited.Remove(transition.TargetState);
            }
        }

        private Transition MergeTransitions(Transition first, Transition second)
        {
            Contract.Requires(first != null);
            Contract.Requires(second != null);

            Contract.Ensures(Contract.Result<Transition>().SourceState == null);

            if (first.IsMatch)
                throw new InvalidOperationException();

            if (first.IsEpsilon)
            {
                if (second.IsEpsilon)
                    return new EpsilonTransition(second.TargetState);

                MatchRangeTransition matchRangeTransition = second as MatchRangeTransition;
                if (matchRangeTransition != null)
                    return new MatchRangeTransition(second.TargetState, matchRangeTransition.Range);

                PopContextTransition popContextTransition = second as PopContextTransition;
                if (popContextTransition != null)
                {
                    var transition = new PopContextTransition(second.TargetState, popContextTransition.ContextIdentifiers);
                    transition.PushTransitions.UnionWith(popContextTransition.PushTransitions);
                    Contract.Assert(Contract.ForAll(transition.PushTransitions, i => i.SourceState != null));
                    return transition;
                }

                PushContextTransition pushContextTransition = second as PushContextTransition;
                if (pushContextTransition != null)
                {
                    var transition = new PushContextTransition(second.TargetState, pushContextTransition.ContextIdentifiers);
                    transition.PopTransitions.UnionWith(pushContextTransition.PopTransitions);
                    Contract.Assert(Contract.ForAll(transition.PopTransitions, i => i.SourceState != null));
                    return transition;
                }

                throw new NotSupportedException();
            }

            PopContextTransition popFirst = first as PopContextTransition;
            if (popFirst != null)
            {
                if (second.IsEpsilon)
                {
                    var transition = new PopContextTransition(second.TargetState, popFirst.ContextIdentifiers);
                    transition.PushTransitions.UnionWith(popFirst.PushTransitions);
                    Contract.Assert(Contract.ForAll(transition.PushTransitions, i => i.SourceState != null));
                    return transition;
                }

                PopContextTransition popSecond = second as PopContextTransition;
                if (popSecond != null)
                {
                    var transition = new PopContextTransition(popSecond.TargetState, popFirst.ContextIdentifiers.Concat(popSecond.ContextIdentifiers));
                    //transition.PushTransitions.UnionWith(popFirst.PushTransitions);
                    transition.PushTransitions.UnionWith(popSecond.PushTransitions);
                    Contract.Assert(Contract.ForAll(transition.PushTransitions, i => i.SourceState != null));
                    return transition;
                }

                if (second is PushContextTransition)
                    throw new InvalidOperationException();

                if (second.IsMatch)
                    throw new NotSupportedException();

                throw new NotImplementedException();
            }

            PushContextTransition pushFirst = first as PushContextTransition;
            if (pushFirst != null)
            {
                if (second.IsEpsilon)
                {
                    var transition = new PushContextTransition(second.TargetState, pushFirst.ContextIdentifiers);
                    transition.PopTransitions.UnionWith(pushFirst.PopTransitions);
                    Contract.Assert(Contract.ForAll(transition.PopTransitions, i => i.SourceState != null));
                    return transition;
                }

                PushContextTransition pushSecond = second as PushContextTransition;
                if (pushSecond != null)
                {
                    var transition = new PushContextTransition(pushSecond.TargetState, pushFirst.ContextIdentifiers.Concat(pushSecond.ContextIdentifiers));
                    transition.PopTransitions.UnionWith(pushFirst.PopTransitions);
                    //transition.PopTransitions.UnionWith(pushSecond.PopTransitions);
                    Contract.Assert(Contract.ForAll(transition.PopTransitions, i => i.SourceState != null));
                    return transition;
                }

                if (second is PopContextTransition)
                    throw new InvalidOperationException();

                if (second.IsMatch)
                    throw new NotSupportedException();

                throw new NotImplementedException();
            }

            throw new NotImplementedException();
        }

        public void AddTransition(Transition transition)
        {
            Contract.Requires<ArgumentNullException>(transition != null, "transition");
            Contract.Requires<InvalidOperationException>(transition.SourceState == null);
            Contract.Requires<InvalidOperationException>(!OutgoingTransitions.Contains(transition));
            Contract.Requires<InvalidOperationException>(!transition.TargetState.IncomingTransitions.Contains(transition));

            AddTransitionInternal(transition);
        }

        public void RemoveTransition(Transition transition)
        {
            Contract.Requires<ArgumentNullException>(transition != null, "transition");
            Contract.Requires<ArgumentException>(transition.SourceState == this);
            Contract.Requires<InvalidOperationException>(OutgoingTransitions.Contains(transition));

            Contract.Ensures(transition.SourceState == null);
            Contract.Ensures(!Contract.OldValue(transition.SourceState).OutgoingTransitions.Contains(transition));
            Contract.Ensures(!transition.TargetState.IncomingTransitions.Contains(transition));

            RemoveTransitionInternal(transition);
        }

        public IntervalSet GetSourceSet()
        {
            return GetSourceSet(PreventContextType.None);
        }

        public IntervalSet GetSourceSet(PreventContextType preventContextType)
        {
            if (_sourceSet != null && _sourceSet[(int)preventContextType] != null)
                return _sourceSet[(int)preventContextType];

            IntervalSet[] sets = _sourceSet ?? new IntervalSet[Enum.GetValues(typeof(PreventContextType)).Cast<int>().Max() + 1];
            IntervalSet set = new IntervalSet();
            HashSet<Transition> visited = new HashSet<Transition>(ObjectReferenceEqualityComparer<Transition>.Default);
            var queue = new Queue<Tuple<Transition, PreventContextType>>(_incomingTransitions.Select(i => Tuple.Create(i, preventContextType)));

            while (queue.Count > 0)
            {
                var pair = queue.Dequeue();
                Transition transition = pair.Item1;
                PreventContextType nextPreventContextType = pair.Item2;
                if (!visited.Add(transition))
                    continue;

                if (transition.SourceState.IsOptimized)
                {
                    switch (nextPreventContextType)
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
                }

                if (transition.IsEpsilon || transition.IsContext)
                {
                    if (transition.IsContext)
                    {
                        nextPreventContextType = PreventContextType.None;
                        if (transition is PushContextTransition)
                            nextPreventContextType = PreventContextType.Push;
                        else if (transition is PopContextTransition)
                            nextPreventContextType = PreventContextType.Pop;

                        if (transition.IsRecursive)
                            nextPreventContextType++; // only block non-recursive transitions
                    }

                    if (transition.SourceState._sourceSet != null && transition.SourceState._sourceSet[(int)nextPreventContextType] != null)
                    {
                        set.UnionWith(transition.SourceState._sourceSet[(int)nextPreventContextType]);
                    }
                    else
                    {
                        foreach (var incoming in transition.SourceState.IncomingTransitions)
                            queue.Enqueue(Tuple.Create(incoming, nextPreventContextType));
                    }
                }
                else
                {
                    set.UnionWith(transition.MatchSet);
                }
            }

            _sourceSet = sets;
            _sourceSet[(int)preventContextType] = set;

            return set;
        }

        public IntervalSet GetFollowSet()
        {
            return GetFollowSet(PreventContextType.None);
        }

        public IntervalSet GetFollowSet(PreventContextType preventContextType)
        {
            if (_followSet != null && _followSet[(int)preventContextType] != null)
                return _followSet[(int)preventContextType];

            IntervalSet[] sets = _followSet ?? new IntervalSet[Enum.GetValues(typeof(PreventContextType)).Cast<int>().Max() + 1];
            IntervalSet set = new IntervalSet();
            HashSet<Transition> visited = new HashSet<Transition>(ObjectReferenceEqualityComparer<Transition>.Default);
            var queue = new Queue<Tuple<Transition, PreventContextType>>(_outgoingTransitions.Select(i => Tuple.Create(i, preventContextType)));

            while (queue.Count > 0)
            {
                var pair = queue.Dequeue();
                Transition transition = pair.Item1;
                PreventContextType nextPreventContextType = pair.Item2;
                if (!visited.Add(transition))
                    continue;

                if (transition.IsContext)
                {
                    switch (nextPreventContextType)
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
                }

                if (transition.IsEpsilon || transition.IsContext)
                {
                    if (transition.IsContext)
                    {
                        nextPreventContextType = PreventContextType.None;
                        if (transition.SourceState.IsOptimized)
                        {
                            if (transition is PushContextTransition)
                                nextPreventContextType = PreventContextType.Push;
                            else if (transition is PopContextTransition)
                                nextPreventContextType = PreventContextType.Pop;

                            if (transition.IsRecursive)
                                nextPreventContextType++; // only block non-recursive transitions
                        }
                    }

                    if (transition.TargetState._followSet != null && transition.TargetState._followSet[(int)nextPreventContextType] != null)
                    {
                        set.UnionWith(transition.TargetState._followSet[(int)nextPreventContextType]);
                    }
                    else
                    {
                        foreach (var outgoing in transition.TargetState.OutgoingTransitions)
                            queue.Enqueue(Tuple.Create(outgoing, nextPreventContextType));
                    }
                }
                else
                {
                    set.UnionWith(transition.MatchSet);
                }
            }

            _followSet = sets;
            _followSet[(int)preventContextType] = set;

            return set;
        }

        public override string ToString()
        {
            return string.Format("State {0}{1} {2}/{3}", Id, IsOptimized ? "!" : string.Empty, IncomingTransitions.Count, OutgoingTransitions.Count);
        }

        private void AddTransitionInternal(Transition transition)
        {
            Contract.Requires(transition != null);
            Contract.Requires(transition.SourceState == null);
            Contract.Requires(!OutgoingTransitions.Contains(transition));
            Contract.Requires(!transition.TargetState.IncomingTransitions.Contains(transition));

            _outgoingTransitions.Add(transition);
            transition.TargetState.IncomingTransitions.Add(transition);
            transition.SourceState = this;

            PopContextTransition popContextTransition = transition as PopContextTransition;
            PushContextTransition pushContextTransition = transition as PushContextTransition;
            if (popContextTransition != null)
            {
                foreach (var pushTransition in popContextTransition.PushTransitions)
                {
                    Contract.Assert(pushTransition.ContextIdentifiers.First() == popContextTransition.ContextIdentifiers.Last());
                    Contract.Assert(pushTransition.SourceState != null);
                    pushTransition.PopTransitions.Add(popContextTransition);
                }

                Contract.Assert(Contract.ForAll(OutgoingTransitions.OfType<PushContextTransition>(), i => i.ContextIdentifiers.Last() != popContextTransition.ContextIdentifiers.First() || popContextTransition.PushTransitions.Contains(i)));
            }
            else if (pushContextTransition != null)
            {
                foreach (var popTransition in pushContextTransition.PopTransitions)
                {
                    Contract.Assert(popTransition.ContextIdentifiers.Last() == pushContextTransition.ContextIdentifiers.First());
                    Contract.Assert(popTransition.SourceState != null);
                    popTransition.PushTransitions.Add(pushContextTransition);
                }

                Contract.Assert(Contract.ForAll(OutgoingTransitions.OfType<PopContextTransition>(), i => i.ContextIdentifiers.Last() != pushContextTransition.ContextIdentifiers.First() || pushContextTransition.PopTransitions.Contains(i)));
            }

            _followSet = null;
            _isForwardRecursive = null;
            transition.TargetState._sourceSet = null;
            transition.TargetState._isBackwardRecursive = null;
        }

        private void RemoveTransitionInternal(Transition transition)
        {
            Contract.Requires(transition != null, "transition");
            Contract.Requires(transition.SourceState == this);
            Contract.Requires(OutgoingTransitions.Contains(transition));

            Contract.Ensures(transition.SourceState == null);
            Contract.Ensures(!Contract.OldValue(transition.SourceState).OutgoingTransitions.Contains(transition));
            Contract.Ensures(!transition.TargetState.IncomingTransitions.Contains(transition));

            Contract.Assert(transition.TargetState.IncomingTransitions.Contains(transition));

            PopContextTransition popContextTransition = transition as PopContextTransition;
            if (popContextTransition != null)
            {
                foreach (var pushTransition in popContextTransition.PushTransitions)
                {
                    Contract.Assert(pushTransition.PopTransitions.Contains(transition));
                    pushTransition.PopTransitions.Remove(popContextTransition);
                }
            }

            PushContextTransition pushContextTransition = transition as PushContextTransition;
            if (pushContextTransition != null)
            {
                foreach (var popTransition in pushContextTransition.PopTransitions)
                {
                    Contract.Assert(popTransition.PushTransitions.Contains(transition));
                    popTransition.PushTransitions.Remove(pushContextTransition);
                }
            }

            OutgoingTransitions.Remove(transition);
            transition.TargetState.IncomingTransitions.Remove(transition);

            _followSet = null;
            _isForwardRecursive = null;
            transition.TargetState._sourceSet = null;
            transition.TargetState._isBackwardRecursive = null;
            transition.SourceState = null;
        }
    }
}
