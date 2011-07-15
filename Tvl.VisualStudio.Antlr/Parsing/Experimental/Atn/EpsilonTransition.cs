namespace Tvl.VisualStudio.Language.Parsing.Experimental.Atn
{
    using IntervalSet = Tvl.VisualStudio.Language.Parsing.Collections.IntervalSet;

    public sealed class EpsilonTransition : Transition
    {
        public EpsilonTransition(State targetState)
            : base(targetState)
        {
        }

        public override sealed bool IsEpsilon
        {
            get
            {
                return true;
            }
        }

        public override bool IsContext
        {
            get
            {
                return false;
            }
        }

        public override bool IsMatch
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
