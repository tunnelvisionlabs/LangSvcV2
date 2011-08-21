namespace Tvl.Java.DebugInterface
{
    /// <summary>
    /// A string object from the target VM. A StringReference is an <see cref="IObjectReference"/>
    /// with additional access to string-specific information from the target VM.
    /// </summary>
    public interface IStringReference : IObjectReference
    {
        /// <summary>
        /// Returns the IStringReference as a string.
        /// </summary>
        string GetValue();
    }
}
