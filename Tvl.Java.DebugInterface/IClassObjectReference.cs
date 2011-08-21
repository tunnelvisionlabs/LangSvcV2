namespace Tvl.Java.DebugInterface
{
    /// <summary>
    /// An instance of java.lang.Class from the target VM. Use this interface to access type
    /// information for the class, array, or interface that this object reflects.
    /// </summary>
    public interface IClassObjectReference : IObjectReference
    {
        /// <summary>
        /// Gets the <see cref="IReferenceType"/> corresponding to this class object.
        /// </summary>
        IReferenceType GetReflectedType();
    }
}
