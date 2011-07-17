namespace Tvl.VisualStudio.Language.Parsing.Experimental.Atn
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using IntervalSet = Tvl.VisualStudio.Language.Parsing.Collections.IntervalSet;
    using IList = System.Collections.IList;
    using PreventContextType = Tvl.VisualStudio.Language.Parsing.Experimental.Interpreter.PreventContextType;

    public class State
    {
        private static int _nextState = 0;

        private readonly List<Transition> _outgoingTransitions = new List<Transition>();
        private readonly List<Transition> _incomingTransitions = new List<Transition>();
        private readonly int _id = _nextState++;

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

                    HashSet<Transition> visited = new HashSet<Transition>();
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
            OptimizeIncomingTransitions();
            OptimizeOutgoingTransitions();
            _isOptimized = true;
        }

        private void OptimizeIncomingTransitions()
        {
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
                HashSet<State> visited = new HashSet<State>();
                visited.Add(this);

                AddOptimizedTransitions(visited, transition);
            }
        }

        private void AddOptimizedTransitions(HashSet<State> visited, Transition transition)
        {
            try
            {
                if (transition.IsMatch || !visited.Add(transition.TargetState))
                {
                    if (!transition.IsMatch)
                    {
                        // must be here because it's a recursive state
                        transition.IsRecursive = true;
                    }

                    AddTransition(transition);
                    return;
                }

                if (transition.IsEpsilon)
                {
                    foreach (var nextTransition in transition.TargetState.OutgoingTransitions)
                    {
                        AddOptimizedTransitions(visited, MergeTransitions(transition, nextTransition));
                    }

                    return;
                }

                if (transition.IsContext)
                {
                    bool added = false;

                    foreach (var nextTransition in transition.TargetState.OutgoingTransitions)
                    {
                        bool canMerge = !nextTransition.IsMatch;

                        if (nextTransition.IsContext)
                        {
                            canMerge = nextTransition.GetType() == transition.GetType();
                        }

                        if (canMerge)
                        {
                            AddOptimizedTransitions(visited, MergeTransitions(transition, nextTransition));
                        }
                        else if (!added)
                        {
                            AddTransition(transition);
                            added = true;
                        }
                    }

                    return;
                }
            }
            finally
            {
                visited.Remove(transition.TargetState);
            }

            throw new NotImplementedException();
        }

        private Transition MergeTransitions(Transition first, Transition second)
        {
            Contract.Requires<ArgumentNullException>(first != null, "first");
            Contract.Requires<ArgumentNullException>(second != null, "second");

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
                    return transition;
                }

                PushContextTransition pushContextTransition = second as PushContextTransition;
                if (pushContextTransition != null)
                {
                    var transition = new PushContextTransition(second.TargetState, pushContextTransition.ContextIdentifiers);
                    transition.PopTransitions.UnionWith(pushContextTransition.PopTransitions);
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
                    return transition;
                }

                PopContextTransition popSecond = second as PopContextTransition;
                if (popSecond != null)
                {
                    var transition = new PopContextTransition(popSecond.TargetState, popFirst.ContextIdentifiers.Concat(popSecond.ContextIdentifiers));
                    //transition.PushTransitions.UnionWith(popFirst.PushTransitions);
                    transition.PushTransitions.UnionWith(popSecond.PushTransitions);
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
                    return transition;
                }

                PushContextTransition pushSecond = second as PushContextTransition;
                if (pushSecond != null)
                {
                    var transition = new PushContextTransition(pushSecond.TargetState, pushFirst.ContextIdentifiers.Concat(pushSecond.ContextIdentifiers));
                    transition.PopTransitions.UnionWith(pushFirst.PopTransitions);
                    //transition.PopTransitions.UnionWith(pushSecond.PopTransitions);
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

            _outgoingTransitions.Add(transition);
            transition.TargetState.IncomingTransitions.Add(transition);
            transition.SourceState = this;

            PopContextTransition popContextTransition = transition as PopContextTransition;
            PushContextTransition pushContextTransition = transition as PushContextTransition;
            if (popContextTransition != null)
            {
                foreach (var pushTransition in popContextTransition.PushTransitions)
                {
                    if (pushTransition.ContextIdentifiers.First() != popContextTransition.ContextIdentifiers.Last())
                        throw new InvalidOperationException();

                    pushTransition.PopTransitions.Add(popContextTransition);
                }
            }
            else if (pushContextTransition != null)
            {
                foreach (var popTransition in pushContextTransition.PopTransitions)
                {
                    if (popTransition.ContextIdentifiers.Last() != pushContextTransition.ContextIdentifiers.First())
                        throw new InvalidOperationException();

                    popTransition.PushTransitions.Add(pushContextTransition);
                }
            }

            _followSet = null;
            _isForwardRecursive = null;
            transition.TargetState._sourceSet = null;
            transition.TargetState._isBackwardRecursive = null;
        }

        public void RemoveTransition(Transition transition)
        {
            Contract.Requires<ArgumentNullException>(transition != null, "transition");
            Contract.Requires<ArgumentException>(transition.SourceState == this);
            Contract.Requires<InvalidOperationException>(OutgoingTransitions.Contains(transition));

            Contract.Ensures(transition.SourceState == null);
            Contract.Ensures(!Contract.OldValue(transition.SourceState).OutgoingTransitions.Contains(transition));
            Contract.Ensures(!transition.TargetState.IncomingTransitions.Contains(transition));

            Contract.Assert(transition.TargetState.IncomingTransitions.Contains(transition));

            PopContextTransition popContextTransition = transition as PopContextTransition;
            if (popContextTransition != null)
            {
                foreach (var pushTransition in popContextTransition.PushTransitions)
                    pushTransition.PopTransitions.Remove(popContextTransition);
            }

            PushContextTransition pushContextTransition = transition as PushContextTransition;
            if (pushContextTransition != null)
            {
                foreach (var popTransition in pushContextTransition.PopTransitions)
                    popTransition.PushTransitions.Remove(pushContextTransition);
            }

            OutgoingTransitions.Remove(transition);
            transition.TargetState.IncomingTransitions.Remove(transition);

            _followSet = null;
            _isForwardRecursive = null;
            transition.TargetState._sourceSet = null;
            transition.TargetState._isBackwardRecursive = null;
            transition.SourceState = null;
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
            HashSet<Transition> visited = new HashSet<Transition>();
            Queue<Transition> queue = new Queue<Transition>(_incomingTransitions);
            PreventContextType nextPreventContextType = preventContextType;

            while (queue.Count > 0)
            {
                Transition transition = queue.Dequeue();
                if (!visited.Add(transition))
                    continue;

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

                if (transition.IsEpsilon || transition.IsContext)
                {
                    if (transition.IsContext)
                    {
                        nextPreventContextType = PreventContextType.None;
                        if (transition.TargetState.IsOptimized)
                        {
                            if (transition is PushContextTransition)
                                nextPreventContextType = PreventContextType.Push;
                            else if (transition is PopContextTransition)
                                nextPreventContextType = PreventContextType.Pop;

                            if (transition.IsRecursive)
                                nextPreventContextType++; // only block non-recursive transitions
                        }
                    }

                    if (transition.SourceState._sourceSet != null && transition.SourceState._sourceSet[(int)nextPreventContextType] != null)
                    {
                        set.UnionWith(transition.SourceState._sourceSet[(int)nextPreventContextType]);
                    }
                    else
                    {
                        foreach (var incoming in transition.SourceState.IncomingTransitions)
                            queue.Enqueue(incoming);
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
            HashSet<Transition> visited = new HashSet<Transition>();
            Queue<Transition> queue = new Queue<Transition>(_outgoingTransitions);
            PreventContextType nextPreventContextType = preventContextType;

            while (queue.Count > 0)
            {
                Transition transition = queue.Dequeue();
                if (!visited.Add(transition))
                    continue;

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
                            queue.Enqueue(outgoing);
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
    }
}
