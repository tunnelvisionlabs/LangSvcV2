namespace Tvl.VisualStudio.Language.Parsing.Experimental.Atn
{
    using System.Collections.Generic;

    public class PopContextTransition : ContextTransition
    {
        private readonly HashSet<PushContextTransition> _pushTransitions = new HashSet<PushContextTransition>();

        public PopContextTransition(State targetState, IEnumerable<int> contextIdentifiers)
            : base(targetState, contextIdentifiers)
        {
        }

        public PopContextTransition(State targetState, params int[] contextIdentifiers)
            : base(targetState, contextIdentifiers)
        {
        }

        public HashSet<PushContextTransition> PushTransitions
        {
            get
            {
                return _pushTransitions;
            }
        }
    }
}
