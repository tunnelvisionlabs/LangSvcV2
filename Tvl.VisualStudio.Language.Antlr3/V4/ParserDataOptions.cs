namespace Tvl.VisualStudio.Language.AntlrV4
{
    using System;

    [Flags]
    internal enum ParserDataOptions
    {
        None = 0,

        /// <summary>
        /// If results from parsing a previous <see cref="ITextSnapshot"/> are cached, return those instead of updating
        /// the data.
        /// </summary>
        AllowStale = 0x0001,

        /// <summary>
        /// Returns <see langword="null"/> instead of updating the data if the data is not already cached for the
        /// requested snapshot. When used with <see cref="AllowStale"/>, stale data is returned if available, otherwise
        /// <see langword="null"/> is returned.
        /// </summary>
        NoUpdate = 0x0002,
    }
}
