namespace Tvl.VisualStudio.Language.Alloy
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using Antlr.Runtime.Tree;
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
            if (textBuffer == null)
                throw new ArgumentNullException("textBuffer");
            if (backgroundParser == null)
                throw new ArgumentNullException("backgroundParser");
            if (provider == null)
                throw new ArgumentNullException("provider");
            Contract.EndContractBlock();

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
            AntlrParseResultEventArgs antlrParseResultArgs = e as AntlrParseResultEventArgs;
            List<ITagSpan<IOutliningRegionTag>> outliningRegions = new List<ITagSpan<IOutliningRegionTag>>();
            if (antlrParseResultArgs != null)
            {
                AlloyParser.compilationUnit_return resultArgs = antlrParseResultArgs.Result as AlloyParser.compilationUnit_return;
                var result = resultArgs.Tree;
                if (result != null && result.Children != null)
                {
                    foreach (var child in result.Children.OfType<CommonTree>())
                    {
                        if (child.Children == null)
                            continue;

                        switch (child.Type)
                        {
                        case AlloyParser.KW_SIG:
                        case AlloyParser.KW_FUN:
                        case AlloyParser.KW_PRED:
                        case AlloyParser.KW_ASSERT:
                        case AlloyParser.KW_FACT:
                        case AlloyParser.KW_ENUM:
                        case AlloyParser.KW_RUN:
                        case AlloyParser.KW_CHECK:
                            break;

                        default:
                            continue;
                        }

                        foreach (var subchild in child.Children.OfType<CommonTree>())
                        {
                            switch (subchild.Type)
                            {
                            case AlloyParser.LBRACE:
                                var startToken = antlrParseResultArgs.Tokens[subchild.TokenStartIndex];
                                var stopToken = antlrParseResultArgs.Tokens[subchild.TokenStopIndex];
                                Span span = new Span(startToken.StartIndex, stopToken.StopIndex - startToken.StartIndex + 1);
                                SnapshotSpan snapshotSpan = new SnapshotSpan(e.Snapshot, span);
                                IOutliningRegionTag tag = new OutliningRegionTag();
                                TagSpan<IOutliningRegionTag> tagSpan = new TagSpan<IOutliningRegionTag>(snapshotSpan, tag);
                                outliningRegions.Add(tagSpan);
                                break;

                            default:
                                continue;
                            }
                        }
                    }
                }
            }

            this._outliningRegions = outliningRegions;
            OnTagsChanged(new SnapshotSpanEventArgs(new SnapshotSpan(e.Snapshot, new Span(0, e.Snapshot.Length))));
        }
    }
}
