namespace Tvl.VisualStudio.Language.Parsing.Experimental.Atn
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using IntervalSet = Tvl.VisualStudio.Language.Parsing.Collections.IntervalSet;

    public class State
    {
        private static int _nextState = 0;

        private readonly List<Transition> _outgoingTransitions = new List<Transition>();
        private readonly List<Transition> _incomingTransitions = new List<Transition>();
        private readonly int _id = _nextState++;

        private IntervalSet _sourceSet;
        private IntervalSet _followSet;
        private bool? _isForwardRecursive;
        private bool? _isBackwardRecursive;

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

        public void AddTransition(Transition transition)
        {
            Contract.Requires<ArgumentNullException>(transition != null, "transition");

            _outgoingTransitions.Add(transition);
            transition.TargetState.IncomingTransitions.Add(transition);
            transition.SourceState = this;

            _followSet = null;
            _isForwardRecursive = null;
            transition.TargetState._sourceSet = null;
            transition.TargetState._isBackwardRecursive = null;
        }

        public IntervalSet GetSourceSet()
        {
            if (_sourceSet != null)
                return _sourceSet;

            IntervalSet set = new IntervalSet();
            HashSet<Transition> visited = new HashSet<Transition>();
            Queue<Transition> queue = new Queue<Transition>(_incomingTransitions);

            while (queue.Count > 0)
            {
                Transition transition = queue.Dequeue();
                if (!visited.Add(transition))
                    continue;

                if (transition.IsEpsilon || transition.IsContext)
                {
                    foreach (var incoming in transition.SourceState.IncomingTransitions)
                        queue.Enqueue(incoming);
                }
                else
                {
                    set.UnionWith(transition.MatchSet);
                }
            }

            _sourceSet = set;

            return set;
        }

        public IntervalSet GetFollowSet()
        {
            if (_followSet != null)
                return _followSet;

            IntervalSet set = new IntervalSet();
            HashSet<Transition> visited = new HashSet<Transition>();
            Queue<Transition> queue = new Queue<Transition>(_outgoingTransitions);

            while (queue.Count > 0)
            {
                Transition transition = queue.Dequeue();
                if (!visited.Add(transition))
                    continue;

                if (transition.IsEpsilon || transition.IsContext)
                {
                    foreach (var outgoing in transition.TargetState.OutgoingTransitions)
                        queue.Enqueue(outgoing);
                }
                else
                {
                    set.UnionWith(transition.MatchSet);
                }
            }

            _followSet = set;

            return set;
        }
    }
}
