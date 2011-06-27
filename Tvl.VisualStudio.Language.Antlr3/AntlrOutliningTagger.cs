namespace Tvl.VisualStudio.Language.Antlr3
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using Antlr.Runtime.Tree;
    using IAstRuleReturnScope = Antlr.Runtime.IAstRuleReturnScope;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;
    using Tvl.VisualStudio.Language.Parsing;

    internal sealed class AntlrOutliningTagger : ITagger<IOutliningRegionTag>
    {
        private List<ITagSpan<IOutliningRegionTag>> _outliningRegions;
        private readonly AntlrOutliningTaggerProvider _provider;

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public AntlrOutliningTagger(ITextBuffer textBuffer, AntlrBackgroundParser backgroundParser, AntlrOutliningTaggerProvider provider)
        {
            Contract.Requires<ArgumentNullException>(textBuffer != null, "textBuffer");
            Contract.Requires<ArgumentNullException>(backgroundParser != null, "backgroundParser");
            Contract.Requires<ArgumentNullException>(provider != null, "provider");

            this.TextBuffer = textBuffer;
            this.BackgroundParser = backgroundParser;
            this._provider = provider;

            this.BackgroundParser.ParseComplete += OnBackgroundParseComplete;
        }

        private ITextBuffer TextBuffer
        {
            get;
            set;
        }

        private AntlrBackgroundParser BackgroundParser
        {
            get;
            set;
        }

        public IEnumerable<ITagSpan<IOutliningRegionTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (_outliningRegions == null)
            {
                var previousParseResult = BackgroundParser.PreviousParseResult;
                if (previousParseResult != null)
                    UpdateTags(previousParseResult);
            }

            return _outliningRegions ?? Enumerable.Empty<ITagSpan<IOutliningRegionTag>>();
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
            if (antlrParseResultArgs == null)
                return;

            UpdateTags(antlrParseResultArgs);
        }

        private void UpdateTags(AntlrParseResultEventArgs antlrParseResultArgs)
        {
            List<ITagSpan<IOutliningRegionTag>> outliningRegions = new List<ITagSpan<IOutliningRegionTag>>();
            if (antlrParseResultArgs != null)
            {
                IAstRuleReturnScope resultArgs = antlrParseResultArgs.Result as IAstRuleReturnScope;
                var result = resultArgs.Tree as CommonTree;
                if (result != null)
                {
                    ITextSnapshot snapshot = antlrParseResultArgs.Snapshot;

                    foreach (CommonTree child in result.Children)
                    {
                        if (child == null || string.IsNullOrEmpty(child.Text))
                            continue;

                        if (child.Text == "rule" && child.ChildCount > 0 || child.Text.StartsWith("tokens"))
                        {
                            string blockHint;
                            if (child.Text == "rule")
                            {
                                string ruleName = child.Children[0].Text;
                                // don't try to outline the artificial tokens rule
                                if (ruleName == "Tokens")
                                    continue;

                                blockHint = child.Children[0].Text + "...";
                            }
                            else
                            {
                                // this is the special tokens{} block of a combined grammar
                                blockHint = "Tokens...";
                            }

                            var startToken = antlrParseResultArgs.Tokens[child.TokenStartIndex];
                            var stopToken = antlrParseResultArgs.Tokens[child.TokenStopIndex];
                            Span span = new Span(startToken.StartIndex, stopToken.StopIndex - startToken.StartIndex + 1);
                            if (snapshot.GetLineNumberFromPosition(span.Start) == snapshot.GetLineNumberFromPosition(span.End))
                                continue;

                            SnapshotSpan snapshotSpan = new SnapshotSpan(antlrParseResultArgs.Snapshot, span);
                            IOutliningRegionTag tag = new OutliningRegionTag(blockHint, snapshotSpan.GetText());
                            TagSpan<IOutliningRegionTag> tagSpan = new TagSpan<IOutliningRegionTag>(snapshotSpan, tag);
                            outliningRegions.Add(tagSpan);
                        }
                    }
                }
            }

            this._outliningRegions = outliningRegions;
            OnTagsChanged(new SnapshotSpanEventArgs(new SnapshotSpan(antlrParseResultArgs.Snapshot, new Span(0, antlrParseResultArgs.Snapshot.Length))));
        }
    }
}
