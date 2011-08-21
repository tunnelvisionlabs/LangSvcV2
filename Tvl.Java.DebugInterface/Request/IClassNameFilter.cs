namespace Tvl.Java.DebugInterface.Request
{
    public interface IClassNameFilter
    {
        void AddClassExclusionFilter(string classPattern);

        void AddClassFilter(string classPattern);
    }
}
