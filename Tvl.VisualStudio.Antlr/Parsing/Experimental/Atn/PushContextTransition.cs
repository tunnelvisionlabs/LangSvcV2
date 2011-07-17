namespace Tvl.VisualStudio.Language.Parsing.Experimental.Atn
{
    using System.Collections.Generic;

    public class PushContextTransition : ContextTransition
    {
        private readonly HashSet<PopContextTransition> _popTransitions = new HashSet<PopContextTransition>();

        public PushContextTransition(State targetState, IEnumerable<int> contextIdentifiers)
            : base(targetState, contextIdentifiers)
        {
        }

        public PushContextTransition(State targetState, params int[] contextIdentifiers)
            : base(targetState, contextIdentifiers)
        {
        }

        public HashSet<PopContextTransition> PopTransitions
        {
            get
            {
                return _popTransitions;
            }
        }
    }
}
