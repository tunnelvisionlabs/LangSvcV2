namespace Tvl.Java.DebugInterface
{
    /// <summary>
    /// The mirror for a value in the target VM.
    /// </summary>
    public interface IValue : IMirror
    {
        /// <summary>
        /// Returns the run-time type of this value.
        /// </summary>
        IType GetValueType();
    }
}
