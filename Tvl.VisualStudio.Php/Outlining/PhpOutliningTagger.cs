namespace Tvl.VisualStudio.Language.Php.Outlining
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using Antlr.Runtime.Tree;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;
    using Tvl.VisualStudio.Language.Parsing;

    using CharStreamConstants = Antlr.Runtime.CharStreamConstants;
    using IAstRuleReturnScope = Antlr.Runtime.IAstRuleReturnScope;

    internal sealed class PhpOutliningTagger : ITagger<IOutliningRegionTag>
    {
        private readonly ITextBuffer _textBuffer;
        private readonly PhpOutliningBackgroundParser _backgroundParser;
        private readonly PhpOutliningTaggerProvider _provider;

        private List<ITagSpan<IOutliningRegionTag>> _outliningRegions;

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public PhpOutliningTagger(ITextBuffer textBuffer, PhpOutliningTaggerProvider provider)
        {
            Contract.Requires<ArgumentNullException>(textBuffer != null, "textBuffer");
            Contract.Requires<ArgumentNullException>(provider != null, "provider");

            this._textBuffer = textBuffer;
            this._backgroundParser = PhpOutliningBackgroundParser.CreateParser(textBuffer, provider.OutputWindowService, provider.TextDocumentFactoryService);
            this._provider = provider;

            this.BackgroundParser.ParseComplete += HandleBackgroundParseComplete;
            this.BackgroundParser.RequestParse(false);
        }

        private ITextBuffer TextBuffer
        {
            get
            {
                return _textBuffer;
            }
        }

        private PhpOutliningBackgroundParser BackgroundParser
        {
            get
            {
                return _backgroundParser;
            }
        }

        public IEnumerable<ITagSpan<IOutliningRegionTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            return _outliningRegions ?? Enumerable.Empty<ITagSpan<IOutliningRegionTag>>();
        }

        private void OnTagsChanged(SnapshotSpanEventArgs e)
        {
            var t = TagsChanged;
            if (t != null)
                t(this, e);
        }

        private void HandleBackgroundParseComplete(object sender, ParseResultEventArgs e)
        {
            PhpOutliningParseResultEventArgs antlrParseResultArgs = e as PhpOutliningParseResultEventArgs;
            if (antlrParseResultArgs == null)
                return;

            UpdateTags(antlrParseResultArgs);
        }

        private void UpdateTags(PhpOutliningParseResultEventArgs antlrParseResultArgs)
        {
            List<ITagSpan<IOutliningRegionTag>> outliningRegions = new List<ITagSpan<IOutliningRegionTag>>();

            if (antlrParseResultArgs != null)
            {
                ITextSnapshot snapshot = antlrParseResultArgs.Snapshot;

                foreach (CommonTree child in antlrParseResultArgs.OutliningTrees)
                {
                    if (child.Type == CharStreamConstants.EndOfFile)
                        continue;

                    var startToken = antlrParseResultArgs.Tokens[child.TokenStartIndex];
                    var stopToken = antlrParseResultArgs.Tokens[child.TokenStopIndex];
                    Span span = new Span(startToken.StartIndex, stopToken.StopIndex - startToken.StartIndex + 1);
                    if (snapshot.GetLineNumberFromPosition(span.Start) == snapshot.GetLineNumberFromPosition(span.End))
                        continue;

                    SnapshotSpan snapshotSpan = new SnapshotSpan(antlrParseResultArgs.Snapshot, span);
                    IOutliningRegionTag tag = new OutliningRegionTag("...", string.Empty);
                    TagSpan<IOutliningRegionTag> tagSpan = new TagSpan<IOutliningRegionTag>(snapshotSpan, tag);
                    outliningRegions.Add(tagSpan);
                }

                //IAstRuleReturnScope resultArgs = antlrParseResultArgs.Result as IAstRuleReturnScope;
                //var result = resultArgs.Tree as CommonTree;
                //if (result != null)
                //{
                //    ITextSnapshot snapshot = antlrParseResultArgs.Snapshot;

                //    foreach (CommonTree child in result.Children)
                //    {
                //        if (child.Type == CharStreamConstants.EndOfFile)
                //            continue;

                //        var startToken = antlrParseResultArgs.Tokens[child.TokenStartIndex];
                //        var stopToken = antlrParseResultArgs.Tokens[child.TokenStopIndex];
                //        Span span = new Span(startToken.StartIndex, stopToken.StopIndex - startToken.StartIndex + 1);
                //        if (snapshot.GetLineNumberFromPosition(span.Start) == snapshot.GetLineNumberFromPosition(span.End))
                //            continue;

                //        SnapshotSpan snapshotSpan = new SnapshotSpan(antlrParseResultArgs.Snapshot, span);
                //        IOutliningRegionTag tag = new OutliningRegionTag("...", string.Empty);
                //        TagSpan<IOutliningRegionTag> tagSpan = new TagSpan<IOutliningRegionTag>(snapshotSpan, tag);
                //        outliningRegions.Add(tagSpan);
                //    }
                //}
            }

            this._outliningRegions = outliningRegions;
            OnTagsChanged(new SnapshotSpanEventArgs(new SnapshotSpan(antlrParseResultArgs.Snapshot, new Span(0, antlrParseResultArgs.Snapshot.Length))));
        }
    }
}
