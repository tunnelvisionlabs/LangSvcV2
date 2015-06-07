namespace Tvl.VisualStudio.Language.AntlrV4
{
    using System;
    using Microsoft.VisualStudio.Text;

    public class Anchor : IAnchor
    {
        private readonly int _ruleIndex;
        private readonly ITrackingSpan _trackingSpan;

        public Anchor(int ruleIndex, ITrackingSpan trackingSpan)
        {
            if (trackingSpan == null)
                throw new ArgumentNullException("trackingSpan");

            _ruleIndex = ruleIndex;
            _trackingSpan = trackingSpan;
        }

        public int RuleIndex
        {
            get
            {
                return _ruleIndex;
            }
        }

        public ITrackingSpan TrackingSpan
        {
            get
            {
                return _trackingSpan;
            }
        }
    }
}
