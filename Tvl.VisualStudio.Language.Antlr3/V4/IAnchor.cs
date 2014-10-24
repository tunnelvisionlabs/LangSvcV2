namespace Tvl.VisualStudio.Language.AntlrV4
{
    using Microsoft.VisualStudio.Text;

    public interface IAnchor
    {
        int RuleIndex
        {
            get;
        }

        ITrackingSpan TrackingSpan
        {
            get;
        }
    }
}
