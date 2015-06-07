namespace Tvl.VisualStudio.Language.AntlrV4
{
    public interface IReferenceAnchors
    {
        IAnchor Previous
        {
            get;
        }

        IAnchor Enclosing
        {
            get;
        }
    }
}
