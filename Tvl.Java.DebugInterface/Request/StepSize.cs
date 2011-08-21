namespace Tvl.Java.DebugInterface.Request
{
    public enum StepSize
    {
        /// <summary>
        /// Step to the next location on a different line.
        /// </summary>
        Line = -2,

        /// <summary>
        /// Step to the next available location.
        /// </summary>
        Min = -1,

        /// <summary>
        /// Invalid step size.
        /// </summary>
        Invalid = 0,

        /// <summary>
        /// Step into any newly pushed frames.
        /// </summary>
        Into = 1,

        /// <summary>
        /// Step over any newly pushed frames.
        /// </summary>
        Over = 2,

        /// <summary>
        /// Step out of the current frame.
        /// </summary>
        Out = 3,
    }
}
