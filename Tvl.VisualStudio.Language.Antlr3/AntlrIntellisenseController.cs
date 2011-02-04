namespace Tvl.VisualStudio.Language.Antlr3
{
    using System.Collections.Generic;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Tagging;
    using Tvl.VisualStudio.Language.Intellisense;
    using VSOBJGOTOSRCTYPE = Microsoft.VisualStudio.Shell.Interop.VSOBJGOTOSRCTYPE;

    internal class AntlrIntellisenseController : IntellisenseController
    {
        public AntlrIntellisenseController(ITextView textView, AntlrIntellisenseControllerProvider provider, AntlrBackgroundParser backgroundParser)
            : base(textView, provider)
        {
            BackgroundParser = backgroundParser;
            ClassificationTagAggregator = provider.AggregatorFactory.CreateTagAggregator<ClassificationTag>(textView.TextBuffer);
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
    }
}
