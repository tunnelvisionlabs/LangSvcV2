namespace Tvl.VisualStudio.Language.Alloy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Tvl.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Operations;
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

        public ICompletionTargetMapService CompletionTargetMapService
        {
            get
            {
                return _provider.CompletionTargetMapService;
            }
        }

        public void AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets)
        {
            if (session == null || completionSets == null)
                return;

            ITrackingPoint triggerPoint = session.GetTriggerPoint(TextBuffer);
            if (triggerPoint != null)
            {

                ICompletionTarget completionTarget = CompletionTargetMapService.GetCompletionTargetForTextView(session.TextView);
                CompletionInfo completionInfo = completionTarget.CompletionInfo;
                ITextSnapshot snapshot = triggerPoint.TextBuffer.CurrentSnapshot;
                SnapshotPoint point = triggerPoint.GetPoint(snapshot);
                ITrackingPoint point2 = triggerPoint;
                int num4 = 0;
                bool flag = false;
                bool flag2 = true;
                switch (completionInfo.InvocationType)
                {
                case IntellisenseInvocationType.Default:
                    flag2 = completionInfo.InfoType == CompletionInfoType.GlobalInfo;
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
                if (flag2)
                {
                    ITextBuffer textBuffer = TextBuffer;
                    ITextStructureNavigator navigator = TextStructureNavigatorSelectorService.CreateTextStructureNavigator(textBuffer, textBuffer.ContentType);
                    SnapshotPoint currentPosition = new SnapshotPoint(snapshot, point2.GetPosition(snapshot));
                    extentOfWord = navigator.GetExtentOfWord(currentPosition);
                }
                else
                {
                    SnapshotSpan span = new SnapshotSpan(point, 0);
                    extentOfWord = new TextExtent(span, false);
                }

                if (completionInfo.InvocationType == IntellisenseInvocationType.BackspaceDeleteOrBackTab && extentOfWord.Span.Length > 0)
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

                int length = triggerPoint.GetPosition(snapshot) - extentOfWord.Span.Start.Position;
                completionInfo.TextSoFarTrackingSpan = snapshot.CreateTrackingSpan(extentOfWord.Span.Start, length, SpanTrackingMode.EdgeInclusive, TrackingFidelityMode.Forward);
                completionInfo.ApplicableTo = snapshot.CreateTrackingSpan(extentOfWord.Span, SpanTrackingMode.EdgeInclusive, TrackingFidelityMode.Forward);
                if (flag)
                {
                    completionInfo.ApplicableTo = snapshot.CreateTrackingSpan(point.Position - completionInfo.TextSoFar.Length, completionInfo.TextSoFar.Length, SpanTrackingMode.EdgeInclusive, TrackingFidelityMode.Forward);
                }

                completionSets.Add(new KeywordCompletionSet(completionInfo.ApplicableTo, _provider.GlyphService));
            }
        }

        public void Dispose()
        {
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
