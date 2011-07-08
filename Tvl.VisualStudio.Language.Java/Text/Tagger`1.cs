namespace JavaLanguageService.Text
{
    using System;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;

    public abstract class Tagger<T> : ITagger<T>
        where T : ITag
    {
        private TimeSpan _updateDelay;
        private CancellationPolicy _cancellationPolicy;
        private List<SnapshotSpan> _invalidSpans;

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public IEnumerable<ITagSpan<T>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            yield break;
        }

        protected abstract IEnumerable<ITagSpan<T>> GetTagsImpl(NormalizedSnapshotSpanCollection spans);

        protected virtual void OnTagsChanged(SnapshotSpanEventArgs e)
        {
            var t = TagsChanged;
            if (t != null)
                t(this, e);
        }
    }
}
