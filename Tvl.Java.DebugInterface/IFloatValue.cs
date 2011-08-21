namespace Tvl.Java.DebugInterface
{
    using System;

    /// <summary>
    /// Provides access to a primitive float value in the target VM.
    /// </summary>
    public interface IFloatValue : IPrimitiveValue, IComparable<IFloatValue>, IEquatable<IFloatValue>
    {
        /// <summary>
        /// Returns this IFloatValue as a float.
        /// </summary>
        float GetValue();
    }
}
