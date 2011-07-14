namespace Tvl.VisualStudio.Language.Parsing.Experimental
{
    using System;
    using System.Diagnostics.Contracts;
    using IntervalSet = Tvl.VisualStudio.Language.Parsing.Collections.IntervalSet;

    public abstract class Transition
    {
        private State _sourceState;
        private State _targetState;

        public Transition(State targetState)
        {
            Contract.Requires<ArgumentNullException>(targetState != null, "targetState");

            _targetState = targetState;
        }

        public State SourceState
        {
            get
            {
                return _sourceState;
            }

            internal set
            {
                _sourceState = value;
            }
        }

        public State TargetState
        {
            get
            {
                return _targetState;
            }
        }

        public abstract bool IsEpsilon
        {
            get;
        }

        public abstract bool IsContext
        {
            get;
        }

        public abstract bool IsMatch
        {
            get;
        }

        public abstract IntervalSet MatchSet
        {
            get;
        }
    }
}
