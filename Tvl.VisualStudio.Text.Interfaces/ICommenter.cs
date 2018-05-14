namespace Tvl.VisualStudio.Text
{
    using JetBrains.Annotations;
    using Microsoft.VisualStudio.Text;

    public interface ICommenter
    {
        /// <summary>
        /// Comments out spans of code.
        /// </summary>
        /// <param name="spans">The collection of spans to comment out.</param>
        /// <returns>A collection of spans encompassing the resulting comments.</returns>
        [NotNull]
        NormalizedSnapshotSpanCollection CommentSpans([NotNull] NormalizedSnapshotSpanCollection spans);

        /// <summary>
        /// Uncomments spans of code.
        /// </summary>
        /// <param name="spans">The collection of spans to uncomment.</param>
        /// <returns>A collection of spans encompassing the resulting uncommented code.</returns>
        [NotNull]
        NormalizedSnapshotSpanCollection UncommentSpans([NotNull] NormalizedSnapshotSpanCollection spans);
    }
}
