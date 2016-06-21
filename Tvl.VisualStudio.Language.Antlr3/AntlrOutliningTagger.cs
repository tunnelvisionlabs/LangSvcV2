namespace Tvl.VisualStudio.Language.Antlr3
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using Antlr.Runtime.Tree;
    using global::Antlr3.Grammars;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;
    using Tvl.VisualStudio.Language.Parsing;

    using IAstRuleReturnScope = Antlr.Runtime.IAstRuleReturnScope;
    using TokenChannels = Antlr.Runtime.TokenChannels;

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

            this.BackgroundParser.ParseComplete += HandleBackgroundParseComplete;
            this.BackgroundParser.RequestParse(false);
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

        private void HandleBackgroundParseComplete(object sender, ParseResultEventArgs e)
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
                ITextSnapshot snapshot = antlrParseResultArgs.Snapshot;

                IAstRuleReturnScope resultArgs = antlrParseResultArgs.Result as IAstRuleReturnScope;
                var result = resultArgs.Tree as CommonTree;
                if (result != null)
                {

                    foreach (CommonTree child in result.Children)
                    {
                        if (child == null || string.IsNullOrEmpty(child.Text))
                            continue;

                        if (child.Text == "rule" && child.ChildCount > 0 || child.Text.StartsWith("tokens") || child.Text.StartsWith("options"))
                        {
                            string blockHint = "...";
                            if (child.Text == "rule")
                            {
                                string ruleName = child.Children[0].Text;
                                // don't try to outline the artificial tokens rule
                                if (ruleName == "Tokens")
                                    continue;

                                blockHint = child.Children[0].Text + "...";
                            }
                            else if (child.Text.StartsWith("tokens"))
                            {
                                // this is the special tokens{} block of a combined grammar
                                blockHint = "tokens {...}";
                            }
                            else if (child.Text.StartsWith("options"))
                            {
                                // this is the special options{} block of a grammar
                                blockHint = "options {...}";
                            }

                            var startToken = antlrParseResultArgs.Tokens[child.TokenStartIndex];
                            var stopToken = antlrParseResultArgs.Tokens[child.TokenStopIndex];

                            if (startToken.Type == ANTLRParser.DOC_COMMENT)
                            {
                                for (int index = child.TokenStartIndex; index <= child.TokenStopIndex; index++)
                                {
                                    startToken = antlrParseResultArgs.Tokens[index];
                                    if (startToken.Type != ANTLRParser.DOC_COMMENT && startToken.Channel != TokenChannels.Hidden)
                                        break;
                                }
                            }

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

                foreach (var token in antlrParseResultArgs.Tokens)
                {
                    switch (token.Type)
                    {
                    case ANTLRParser.DOC_COMMENT:
                    case ANTLRParser.ML_COMMENT:
                        Span commentSpan = Span.FromBounds(token.StartIndex, token.StopIndex + 1);
                        if (snapshot.GetLineNumberFromPosition(commentSpan.Start) != snapshot.GetLineNumberFromPosition(commentSpan.End))
                        {
                            SnapshotSpan commentSnapshotSpan = new SnapshotSpan(antlrParseResultArgs.Snapshot, commentSpan);
                            IOutliningRegionTag commentTag = new OutliningRegionTag(string.Format("/*{0} ... */", token.Type == ANTLRParser.DOC_COMMENT ? "*" : string.Empty), commentSnapshotSpan.GetText());
                            TagSpan<IOutliningRegionTag> commentTagSpan = new TagSpan<IOutliningRegionTag>(commentSnapshotSpan, commentTag);
                            outliningRegions.Add(commentTagSpan);
                        }
                        break;

                    default:
                        continue;
                    }
                }
            }

            this._outliningRegions = outliningRegions;
            OnTagsChanged(new SnapshotSpanEventArgs(new SnapshotSpan(antlrParseResultArgs.Snapshot, new Span(0, antlrParseResultArgs.Snapshot.Length))));
        }
    }
}
