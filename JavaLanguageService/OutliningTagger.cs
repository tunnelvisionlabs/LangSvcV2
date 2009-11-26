namespace JavaLanguageService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Text;

    internal sealed class OutliningTagger : ITagger<IOutliningRegionTag>
    {
        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public OutliningTagger(ITextBuffer sourceBuffer, JavaBackgroundParser backgroundParser)
        {
            this.SourceBuffer = sourceBuffer;
        }

        public ITextBuffer SourceBuffer
        {
            get;
            private set;
        }

        public JavaBackgroundParser BackgroundParser
        {
            get;
            private set;
        }

        public IEnumerable<ITagSpan<IOutliningRegionTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            yield break;
        }
    }
}
