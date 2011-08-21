namespace Tvl.Java.DebugInterface
{
    using System;

    /// <summary>
    /// Provides access to a primitive void value in the target VM.
    /// </summary>
    public interface IVoidValue : IMirror, IValue, IEquatable<IVoidValue>
    {
    }
}
