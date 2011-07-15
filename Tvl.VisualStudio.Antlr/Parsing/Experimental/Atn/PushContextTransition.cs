namespace Tvl.VisualStudio.Language.Parsing.Experimental.Atn
{
    public class PushContextTransition : ContextTransition
    {
        public PushContextTransition(State targetState, int contextIdentifier)
            : base(targetState, contextIdentifier)
        {
        }

        public PopContextTransition PopTransition
        {
            get;
            set;
        }
    }
}
