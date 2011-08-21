namespace Tvl.Java.DebugInterface
{
    /// <summary>
    /// The type associated with non-object values in a target VM. Instances of one of the sub-interfaces
    /// of this interface will be returned from <see cref="IValue.GetValueType()"/> for all
    /// <see cref="IPrimitiveValue"/> objects.
    /// </summary>
    public interface IPrimitiveType : IType
    {
    }
}
