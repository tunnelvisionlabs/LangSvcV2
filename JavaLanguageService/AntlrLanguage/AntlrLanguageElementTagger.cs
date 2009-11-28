namespace JavaLanguageService.AntlrLanguage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using JavaLanguageService.Text.Language;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Text;

    internal sealed class AntlrLanguageElementTagger : ITagger<ILanguageElementTag>
    {
        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public AntlrLanguageElementTagger(ITextBuffer textBuffer)
        {
            this.TextBuffer = textBuffer;
        }

        public ITextBuffer TextBuffer
        {
            get;
            private set;
        }

        public IEnumerable<ITagSpan<ILanguageElementTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            yield break;
        }
    }
}
