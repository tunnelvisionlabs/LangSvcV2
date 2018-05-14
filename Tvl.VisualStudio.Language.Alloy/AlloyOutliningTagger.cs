namespace Tvl.VisualStudio.Language.Alloy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using JetBrains.Annotations;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;
    using Tvl.VisualStudio.Language.Parsing;

    using IAstRuleReturnScope = Antlr.Runtime.IAstRuleReturnScope;

    internal sealed class AlloyOutliningTagger : ITagger<IOutliningRegionTag>
    {
        private List<ITagSpan<IOutliningRegionTag>> _outliningRegions;
        private readonly AlloyOutliningTaggerProvider _provider;

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public AlloyOutliningTagger([NotNull] ITextBuffer textBuffer, [NotNull] AlloyBackgroundParser backgroundParser, [NotNull] AlloyOutliningTaggerProvider provider)
        {
            Requires.NotNull(textBuffer, nameof(textBuffer));
            Requires.NotNull(backgroundParser, nameof(backgroundParser));
            Requires.NotNull(provider, nameof(provider));

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

        private AlloyBackgroundParser BackgroundParser
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
            List<ITagSpan<IOutliningRegionTag>> outliningRegions = null;
            IAstRuleReturnScope parseResult = antlrParseResultArgs.Result as IAstRuleReturnScope;
            if (parseResult != null)
                outliningRegions = AlloyOutliningTaggerWalker.ExtractOutliningRegions(parseResult, antlrParseResultArgs.Tokens, _provider, antlrParseResultArgs.Snapshot);

            this._outliningRegions = outliningRegions ?? new List<ITagSpan<IOutliningRegionTag>>();
            OnTagsChanged(new SnapshotSpanEventArgs(new SnapshotSpan(antlrParseResultArgs.Snapshot, new Span(0, antlrParseResultArgs.Snapshot.Length))));
        }
    }
}
