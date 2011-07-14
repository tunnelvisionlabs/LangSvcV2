namespace Tvl.VisualStudio.Language.Parsing.Experimental
{
    using IntervalSet = Tvl.VisualStudio.Language.Parsing.Collections.IntervalSet;

    public abstract class ContextTransition : Transition
    {
        private readonly int _contextIdentifier;

        public ContextTransition(State targetState, int contextIdentifier)
            : base(targetState)
        {
            _contextIdentifier = contextIdentifier;
        }

        public int ContextIdentifier
        {
            get
            {
                return _contextIdentifier;
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
    }
}
