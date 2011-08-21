namespace Tvl.Java.DebugInterface
{
    /// <summary>
    /// The type of all primitive boolean values accessed in the target VM. Calls
    /// to IValue.GetValueType() will return an implementor of this interface.
    /// </summary>
    public interface IBooleanType : IPrimitiveType
    {
    }
}
