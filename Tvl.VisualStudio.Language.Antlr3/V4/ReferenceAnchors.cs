namespace Tvl.VisualStudio.Language.AntlrV4
{
    public class ReferenceAnchors : IReferenceAnchors
    {
        private readonly IAnchor _previous;
        private readonly IAnchor _enclosing;

        public ReferenceAnchors(IAnchor previous, IAnchor enclosing)
        {
            _previous = previous;
            _enclosing = enclosing;
        }

        public IAnchor Previous
        {
            get
            {
                return _previous;
            }
        }

        public IAnchor Enclosing
        {
            get
            {
                return _enclosing;
            }
        }
    }
}
