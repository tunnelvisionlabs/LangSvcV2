namespace Tvl.VisualStudio.Language.Alloy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Operations;
    using Tvl.VisualStudio.Language.Intellisense;
    using ImageSource = System.Windows.Media.ImageSource;

    // see VBCompletionProvider
    internal class AlloyCompletionSource : ICompletionSource
    {
        private readonly ITextBuffer _textBuffer;
        private readonly AlloyCompletionSourceProvider _provider;

        public AlloyCompletionSource(ITextBuffer textBuffer, AlloyCompletionSourceProvider provider)
        {
            this._textBuffer = textBuffer;
            this._provider = provider;
        }

        public ITextBuffer TextBuffer
        {
            get
            {
                return _textBuffer;
            }
        }

        public ITextStructureNavigatorSelectorService TextStructureNavigatorSelectorService
        {
            get
            {
                return _provider.TextStructureNavigatorSelectorService;
            }
        }

        public void AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets)
        {
            if (session == null || completionSets == null)
                return;

            ITrackingPoint triggerPoint = session.GetTriggerPoint(TextBuffer);
            if (triggerPoint != null)
            {
                IntellisenseController controller = GetControllerForView(session.TextView);
                CompletionInfo completionInfo = controller.CompletionInfo;
                ITextSnapshot snapshot = triggerPoint.TextBuffer.CurrentSnapshot;
                SnapshotPoint point = triggerPoint.GetPoint(snapshot);
                ITrackingPoint point2 = triggerPoint;
                bool flag = false;
                bool extend = true;

                IntellisenseInvocationType invocationType = completionInfo.InvocationType;
                CompletionInfoType infoType = completionInfo.InfoType;

                switch (invocationType)
                {
                case IntellisenseInvocationType.Default:
                    extend = infoType == CompletionInfoType.GlobalInfo;
                    break;

                case IntellisenseInvocationType.BackspaceDeleteOrBackTab:
                case IntellisenseInvocationType.IdentifierChar:
                case IntellisenseInvocationType.Sharp:
                case IntellisenseInvocationType.Space:
                case IntellisenseInvocationType.ShowMemberList:
                    break;

                default:
                    flag = true;
                    break;
                }

                TextExtent extentOfWord;
                if (extend)
                {
                    ITextBuffer textBuffer = TextBuffer;
                    ITextStructureNavigator navigator = TextStructureNavigatorSelectorService.CreateTextStructureNavigator(textBuffer, textBuffer.ContentType);
                    SnapshotPoint currentPosition = new SnapshotPoint(snapshot, point2.GetPosition(snapshot));
                    extentOfWord = navigator.GetExtentOfWord(currentPosition);
                    if (extentOfWord.Span.Start == point)
                    {
                        TextExtent extentOfPreviousWord = navigator.GetExtentOfWord(currentPosition - 1);
                        if (extentOfPreviousWord.IsSignificant && extentOfPreviousWord.Span.End == point)
                            extentOfWord = extentOfPreviousWord;
                    }
                }
                else
                {
                    SnapshotSpan span = new SnapshotSpan(point, 0);
                    extentOfWord = new TextExtent(span, false);
                }

                if (invocationType == IntellisenseInvocationType.BackspaceDeleteOrBackTab && extentOfWord.Span.Length > 0)
                {
                    string str3 = snapshot.GetText(extentOfWord.Span);
                    if (!string.IsNullOrWhiteSpace(str3))
                    {
                        while ("!^()=<>\\:;.,+-*/{}\" '&%@?".IndexOf(str3[0]) > 0)
                        {
                            SnapshotSpan span2 = extentOfWord.Span;
                            SnapshotSpan span3 = new SnapshotSpan(snapshot, span2.Start + 1, span2.Length - 1);
                            extentOfWord = new TextExtent(span3, false);
                            str3 = snapshot.GetText(extentOfWord.Span);
                            if (string.IsNullOrEmpty(str3))
                                break;
                        }
                    }
                    else
                    {
                        SnapshotSpan span4 = new SnapshotSpan(snapshot, extentOfWord.Span.End, 0);
                        extentOfWord = new TextExtent(span4, false);
                        completionInfo.InvocationType = IntellisenseInvocationType.Default;
                    }
                }

                ITrackingSpan applicableTo = snapshot.CreateTrackingSpan(extentOfWord.Span, SpanTrackingMode.EdgeInclusive, TrackingFidelityMode.Forward);
                if (flag)
                {
                    SnapshotSpan textSoFarSpan = new SnapshotSpan(snapshot, extentOfWord.Span.Start, triggerPoint.GetPoint(snapshot));
                    string textSoFar = textSoFarSpan.GetText();
                    applicableTo = snapshot.CreateTrackingSpan(point.Position - textSoFar.Length, textSoFar.Length, SpanTrackingMode.EdgeInclusive, TrackingFidelityMode.Forward);
                }

                KeywordCompletionSet keywordCompletionSet = new KeywordCompletionSet(applicableTo, _provider.GlyphService);

                //int length = triggerPoint.GetPosition(snapshot) - extentOfWord.Span.Start.Position;
                //completionInfo.TextSoFarTrackingSpan = snapshot.CreateTrackingSpan(extentOfWord.Span.Start, length, SpanTrackingMode.EdgeInclusive, TrackingFidelityMode.Forward);

                completionSets.Add(keywordCompletionSet);
            }
        }

        public void Dispose()
        {
        }

        private static IntellisenseController GetControllerForView(ITextView view)
        {
            object controllerList;
            if (!view.Properties.TryGetProperty(typeof(IntellisenseController), out controllerList))
                return null;

            IEnumerable<IntellisenseController> controllers = controllerList as IEnumerable<IntellisenseController>;
            if (controllers == null)
                return null;

            return controllers.OfType<AlloyIntellisenseController>().SingleOrDefault();
        }

        private class KeywordCompletionSet : CompletionSet
        {
            private static readonly List<Completion> _completions = new List<Completion>();

            public KeywordCompletionSet(ITrackingSpan applicableTo, IGlyphService glyphService)
                : base("AlloyKeywords", "AlloyKeywords", applicableTo, GetOrCreateCompletions(glyphService), new List<Completion>())
            {
            }

            private static List<Completion> GetOrCreateCompletions(IGlyphService glyphService)
            {
                if (_completions.Count == 0)
                {
                    _completions.AddRange(AlloyClassifier.Keywords.OrderBy(i => i, StringComparer.CurrentCultureIgnoreCase).Select(i => CreateKeywordCompletion(i, glyphService)));
                }

                return _completions;
            }

            private static Completion CreateKeywordCompletion(string keyword, IGlyphService glyphService)
            {
                string displayText = keyword;
                string insertionText = keyword;
                string description = null;
                ImageSource iconSource = glyphService.GetGlyph(StandardGlyphGroup.GlyphKeyword, StandardGlyphItem.GlyphItemPublic);
                string iconAutomationText = new IconDescription(StandardGlyphGroup.GlyphKeyword, StandardGlyphItem.GlyphItemPublic).ToString();
                return new Completion(displayText, insertionText, description, iconSource, iconAutomationText);
            }
        }
    }
}
