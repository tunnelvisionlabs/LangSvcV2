namespace Tvl.VisualStudio.Language.Antlr3
{
    using System.Collections.Generic;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Tagging;
    using Tvl.VisualStudio.Language.Intellisense;
    using Tvl.VisualStudio.Shell;

    using AntlrIntellisenseOptions = Tvl.VisualStudio.Language.Antlr3.OptionsPages.AntlrIntellisenseOptions;
    using IVsTextView = Microsoft.VisualStudio.TextManager.Interop.IVsTextView;
    using VSOBJGOTOSRCTYPE = Microsoft.VisualStudio.Shell.Interop.VSOBJGOTOSRCTYPE;

    internal class AntlrIntellisenseController : IntellisenseController
    {
        private readonly AntlrIntellisenseOptions _intellisenseOptions;

        public AntlrIntellisenseController(ITextView textView, AntlrIntellisenseControllerProvider provider, AntlrBackgroundParser backgroundParser)
            : base(textView, provider)
        {
            BackgroundParser = backgroundParser;
            ClassificationTagAggregator = provider.AggregatorFactory.CreateTagAggregator<ClassificationTag>(textView.TextBuffer);

            var shell = Provider.GlobalServiceProvider.GetShell();
            var package = shell.LoadPackage<AntlrLanguagePackage>();
            _intellisenseOptions = package.IntellisenseOptions;
        }

        public new AntlrIntellisenseControllerProvider Provider
        {
            get
            {
                return (AntlrIntellisenseControllerProvider)base.Provider;
            }
        }

        public override bool SupportsCompletion
        {
            get
            {
                return true;
            }
        }

        public override bool SupportsGotoDefinition
        {
            get
            {
                return true;
            }
        }

        public AntlrBackgroundParser BackgroundParser
        {
            get;
            private set;
        }

        public ITagAggregator<ClassificationTag> ClassificationTagAggregator
        {
            get;
            private set;
        }

        public override bool IsCommitChar(char c)
        {
            switch (c)
            {
            case ' ':
                return _intellisenseOptions.CommitOnSpace;

            default:
                return _intellisenseOptions.CommitCharacters.IndexOf(c) >= 0;
            }
        }

        public override bool CommitCompletion()
        {
            // return true to suppress the character the user just typed
            if (!base.CommitCompletion())
                return false;

            if (CompletionInfo.CommitChar == ' ')
                return false;

            if (CompletionInfo.CommitChar == '\t')
                return true;

            if (CompletionInfo.CommitChar == '\n')
                return !_intellisenseOptions.NewLineAfterEnterCompletion;

            if (CompletionInfo.CommitChar == null)
                return true;

            return _intellisenseOptions.CommitCharacters.IndexOf(CompletionInfo.CommitChar.Value) < 0;
        }

        public override IEnumerable<INavigateToTarget> GoToSourceImpl(VSOBJGOTOSRCTYPE gotoSourceType, ITrackingPoint triggerPoint)
        {
            if (triggerPoint == null)
                return new INavigateToTarget[0];

            ITextSnapshot currentSnapshot = triggerPoint.TextBuffer.CurrentSnapshot;
            SnapshotPoint point = triggerPoint.GetPoint(currentSnapshot);

            foreach (var span in this.ClassificationTagAggregator.GetTags(new SnapshotSpan(point, point)))
            {
                if (!span.Tag.ClassificationType.IsOfType(AntlrClassificationTypeNames.LexerRule)
                    && !span.Tag.ClassificationType.IsOfType(AntlrClassificationTypeNames.ParserRule))
                {
                    continue;
                }

                NormalizedSnapshotSpanCollection spans = span.Span.GetSpans(currentSnapshot);
                if (spans.Count == 1)
                {
                    SnapshotSpan span2 = spans[0];
                    SnapshotSpan span3 = span.Span.GetSpans(span.Span.AnchorBuffer)[0];
                    if (span2.Length == span3.Length)
                    {
                        SnapshotSpan span4 = spans[0];
                        if (span4.Contains(point))
                        {
                            string ruleName = span2.GetText();
                            var rules = BackgroundParser.RuleSpans;
                            KeyValuePair<ITrackingSpan, ITrackingPoint> value;
                            if (rules != null && rules.TryGetValue(ruleName, out value))
                                return new INavigateToTarget[] { new SnapshotSpanNavigateToTarget(TextView, new SnapshotSpan(value.Value.GetPoint(currentSnapshot), value.Value.GetPoint(currentSnapshot))) };
                        }
                    }
                }
            }

            return new INavigateToTarget[0];
        }

        protected override IntellisenseCommandFilter CreateIntellisenseCommandFilter(IVsTextView textViewAdapter)
        {
            return new AntlrIntellisenseCommandFilter(textViewAdapter, this);
        }
    }
}
