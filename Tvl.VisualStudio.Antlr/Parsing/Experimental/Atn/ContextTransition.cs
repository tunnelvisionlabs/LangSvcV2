namespace Tvl.VisualStudio.Language.Parsing.Experimental.Atn
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using Tvl.Extensions;
    using IntervalSet = Tvl.VisualStudio.Language.Parsing.Collections.IntervalSet;

    public abstract class ContextTransition : Transition
    {
        private readonly IList<int> _contextIdentifiers;

        public ContextTransition(State targetState, IEnumerable<int> contextIdentifiers)
            : base(targetState)
        {
            Contract.Requires<ArgumentNullException>(contextIdentifiers != null, "contextIdentifiers");
            Contract.Requires<ArgumentException>(contextIdentifiers.Any());

            _contextIdentifiers = contextIdentifiers.ToArray();
        }

        public ContextTransition(State targetState, params int[] contextIdentifiers)
            : base(targetState)
        {
            Contract.Requires<ArgumentNullException>(contextIdentifiers != null, "contextIdentifiers");
            Contract.Requires<ArgumentException>(contextIdentifiers.Length > 0);

            _contextIdentifiers = contextIdentifiers.CloneArray();
        }

        public IList<int> ContextIdentifiers
        {
            get
            {
                return _contextIdentifiers;
            }
        }

        public override sealed bool IsEpsilon
        {
            get
            {
                return false;
            }
        }

        public override sealed bool IsContext
        {
            get
            {
                return true;
            }
        }

        public sealed override bool IsMatch
        {
            get
            {
                return false;
            }
        }

        public override IntervalSet MatchSet
        {
            get
            {
                return new IntervalSet();
            }
        }

        public override bool Equals(object obj)
        {
            ContextTransition other = obj as ContextTransition;
            if (other == null)
                return false;

            return ContextIdentifiers.SequenceEqual(other.ContextIdentifiers)
                && base.Equals(other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ ContextIdentifiers.Count.GetHashCode();
        }
    }
}
