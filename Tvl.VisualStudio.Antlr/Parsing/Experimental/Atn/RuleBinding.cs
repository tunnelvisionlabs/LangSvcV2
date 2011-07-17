namespace Tvl.VisualStudio.Language.Parsing.Experimental.Atn
{
    using System;
    using Contract = System.Diagnostics.Contracts.Contract;
    using RuntimeHelpers = System.Runtime.CompilerServices.RuntimeHelpers;

    public class RuleBinding : IEquatable<RuleBinding>
    {
        public readonly string Name;
        public readonly State StartState;
        public readonly State EndState;

        public RuleBinding(string name)
            : this(name, new State(), new State())
        {
            Contract.Requires(!String.IsNullOrEmpty(name));

            Contract.Ensures(!string.IsNullOrEmpty(this.Name));
            Contract.Ensures(this.StartState != null);
            Contract.Ensures(this.EndState != null);
        }

        public RuleBinding(string name, State startState, State endState)
        {
            Contract.Requires<ArgumentNullException>(name != null, "name");
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(name));
            Contract.Requires<ArgumentNullException>(startState != null, "startState");
            Contract.Requires<ArgumentNullException>(endState != null, "endState");

            Contract.Ensures(!string.IsNullOrEmpty(this.Name));
            Contract.Ensures(this.StartState != null);
            Contract.Ensures(this.EndState != null);

            Name = name;
            StartState = startState;
            EndState = endState;
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
            return string.Format("Rule {0}: {1}", Name, StartState);
        }
    }
}
