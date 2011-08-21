namespace Tvl.Java.DebugInterface.Request
{
    public interface IClassFilter : IClassNameFilter
    {
        void AddClassFilter(IReferenceType referenceType);
    }
}
