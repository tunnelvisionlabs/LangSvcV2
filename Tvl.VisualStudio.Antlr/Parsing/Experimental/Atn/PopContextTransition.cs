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

        public override bool Equals(object obj)
        {
            return base.Equals(obj as PopContextTransition);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            string source = SourceState != null ? SourceState.Id.ToString() + (SourceState.IsOptimized ? "!" : string.Empty) : "?";
            string target = TargetState != null ? TargetState.Id.ToString() + (TargetState.IsOptimized ? "!" : string.Empty) : "?";
            string context = string.Join(" ", ContextIdentifiers);

            return string.Format("{0} -> pop {1} -> {2}", source, context, target);
        }
    }
}
