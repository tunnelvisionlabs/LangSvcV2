namespace Tvl.VisualStudio.Language.Parsing.Experimental.Atn
{
    using System;
    using System.Diagnostics;
    using JetBrains.Annotations;
    using RuntimeHelpers = System.Runtime.CompilerServices.RuntimeHelpers;

    public class RuleBinding : IEquatable<RuleBinding>
    {
        private readonly string _name;
        private readonly State _startState;
        private readonly State _endState;

        private bool _isStartRule;

        public RuleBinding([NotNull] string name)
            : this(name, new State(), new State())
        {
            Debug.Assert(!String.IsNullOrEmpty(name));
        }

        public RuleBinding([NotNull] string name, [NotNull] State startState, [NotNull] State endState)
        {
            Requires.NotNullOrEmpty(name, nameof(name));
            Requires.NotNull(startState, nameof(startState));
            Requires.NotNull(endState, nameof(endState));

            _name = name;
            _startState = startState;
            _endState = endState;
        }

        [NotNull]
        public string Name
        {
            get
            {
                return _name;
            }
        }

        [NotNull]
        public State StartState
        {
            get
            {
                return _startState;
            }
        }

        [NotNull]
        public State EndState
        {
            get
            {
                return _endState;
            }
        }

        public bool IsStartRule
        {
            get
            {
                return _isStartRule;
            }

            set
            {
                _isStartRule = value;
            }
        }

        public bool Equals(RuleBinding other)
        {
            return object.ReferenceEquals(this, other);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as RuleBinding);
        }

        public override int GetHashCode()
        {
            return RuntimeHelpers.GetHashCode(this);
        }

        public override string ToString()
        {
            return string.Format("Rule '{0}': {1}", Name, StartState);
        }
    }
}
