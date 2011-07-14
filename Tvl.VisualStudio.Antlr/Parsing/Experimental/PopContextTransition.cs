namespace Tvl.VisualStudio.Language.Parsing.Experimental
{
    public class PopContextTransition : ContextTransition
    {
        public PopContextTransition(State targetState, int contextIdentifier)
            : base(targetState, contextIdentifier)
        {
        }

        public PushContextTransition PushTransition
        {
            get;
            set;
        }
    }
}
