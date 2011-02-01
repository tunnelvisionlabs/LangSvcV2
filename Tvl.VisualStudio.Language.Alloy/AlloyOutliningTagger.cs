namespace Tvl.VisualStudio.Language.Alloy
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;
    using Tvl.VisualStudio.Language.Parsing;

    internal sealed class AlloyOutliningTagger : ITagger<IOutliningRegionTag>
    {
        private List<ITagSpan<IOutliningRegionTag>> _outliningRegions;
        private readonly AlloyOutliningTaggerProvider _provider;

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public AlloyOutliningTagger(ITextBuffer textBuffer, IBackgroundParser backgroundParser, AlloyOutliningTaggerProvider provider)
        {
            Contract.Requires<ArgumentNullException>(textBuffer != null, "textBuffer");
            Contract.Requires<ArgumentNullException>(backgroundParser != null, "backgroundParser");
            Contract.Requires<ArgumentNullException>(provider != null, "provider");

            this.TextBuffer = textBuffer;
            this.BackgroundParser = backgroundParser;
            this._provider = provider;

            this._outliningRegions = new List<ITagSpan<IOutliningRegionTag>>();

            this.BackgroundParser.ParseComplete += OnBackgroundParseComplete;
        }

        private ITextBuffer TextBuffer
        {
            get;
            set;
        }

        private IBackgroundParser BackgroundParser
        {
            get;
            set;
        }

        public IEnumerable<ITagSpan<IOutliningRegionTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            return _outliningRegions;
        }

        private void OnTagsChanged(SnapshotSpanEventArgs e)
        {
            var t = TagsChanged;
            if (t != null)
                t(this, e);
        }

        private void OnBackgroundParseComplete(object sender, ParseResultEventArgs e)
        {
            List<ITagSpan<IOutliningRegionTag>> outliningRegions = null;
            AntlrParseResultEventArgs antlrParseResultArgs = e as AntlrParseResultEventArgs;
            if (antlrParseResultArgs != null)
            {
                AlloyParser.compilationUnit_return parseResult = antlrParseResultArgs.Result as AlloyParser.compilationUnit_return;
                if (parseResult != null)
                    outliningRegions = AlloyOutliningTaggerWalker.ExtractOutliningRegions(parseResult, antlrParseResultArgs.Tokens, _provider, e.Snapshot);
            }

            this._outliningRegions = outliningRegions ?? new List<ITagSpan<IOutliningRegionTag>>();
            OnTagsChanged(new SnapshotSpanEventArgs(new SnapshotSpan(e.Snapshot, new Span(0, e.Snapshot.Length))));
        }
    }
}
