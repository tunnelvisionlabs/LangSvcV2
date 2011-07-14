namespace Tvl.VisualStudio.Language.Parsing.Experimental
{
    using Interval = Tvl.VisualStudio.Language.Parsing.Collections.Interval;
    using IntervalSet = Tvl.VisualStudio.Language.Parsing.Collections.IntervalSet;

    public class MatchRangeTransition : Transition
    {
        private Interval _range;

        public MatchRangeTransition(State targetState, Interval range)
            : base(targetState)
        {
            _range = range;
        }

        public Interval Range
        {
            get
            {
                return _range;
            }
        }

        public override bool IsEpsilon
        {
            get
            {
                return false;
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
                return true;
            }
        }

        public override IntervalSet MatchSet
        {
            get
            {
                return IntervalSet.Of(_range);
            }
        }
    }
}
